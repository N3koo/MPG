using MpgWebService.Business.Data.Extension;
using MpgWebService.Presentation.Request;
using MpgWebService.Business.Data.DTO;
using MpgWebService.Properties;
using NHibernate.Transform;

using DataEntity.Model.Output;
using DataEntity.Model.Input;
using DataEntity.Model.Types;
using DataEntity.Config;

using System.Collections.Generic;
using System.Linq;
using System;

namespace MpgWebService.Repository.Clients {

    public class MesClient {

        private readonly static string CorrectionQuery = "SELECT a.* FROM MES2MPG_Correction a INNER JOIN MES2MPG_QualityCheck b ON a.CorrectionID = b.ID " +
            "WHERE b.ID = (:id) AND a.MPGStatus IS NULL";

        private readonly static string LabelQuery = "SELECT a.PlantID, a.MaterialID, c.ItemQty, a.PODescription, a.POID, b.PailNumber, a.KoberLot, b.MES_Sample_ID, b.Op_No " +
            "FROM MES2MPG_ProductionOrders a INNER JOIN MPG2MES_ProductionOrderPailStatus b ON a.POID = b.POID INNER JOIN MES2MPG_ProductionOrderFinalItems c ON a.POID = c.POID " +
            "WHERE b.PailNumber = (:pail) AND a.POID = '(:POID)'";

        public static readonly MesClient Client = new();

        public List<ProductionOrder> GetCommands(Period period) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= period.StartDate &&
                p.PlannedEndDate <= period.EndDate && p.Status == "ELB").ToList();
        }

        public void SaveDosageMaterials(List<ProductionOrderConsumption> consumption) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            consumption.ForEach(item => {
                session.Save(item);
            });

            transaction.Commit();
        }

        public string GetQc(string POID) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<ProductionOrderLotHeader>().First(p => p.POID == POID)?.PozQC;
        }

        public void SaveCorrection(ProductionOrderCorection correction) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            session.Save(correction);
            transaction.Commit();
        }

        public void ConsumeMaterials(List<POConsumption> materials) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            materials.ForEach(item => {
                session.Save(POConsumption.CreateConsumption(item));
            });

            transaction.Commit();
        }

        public QcLabel SetQcStatus(QcDetails details) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var pail = session.Query<ProductionOrderPailStatus>().First(p => p.POID == details.POID && p.PailNumber == details.PailNumber);
            pail.Op_No = details.OpNo;
            pail.PailStatus = Resources.CMD_QC;
            pail.MESStatus = 0;
            pail.MPGStatus = 1;

            session.Update(pail);
            transaction.Commit();

            var label = session.CreateSQLQuery(LabelQuery)
                .SetResultTransformer(Transformers.AliasToBean<QcLabel>())
                .SetString("pail", details.PailNumber)
                .SetString("POID", details.POID).List<QcLabel>();

            return label.Count == 0 ? null : label[0];
        }

        public List<Correction> GetCorrections(QcDetails details) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var qc = session.Query<QualityCheck>().FirstOrDefault(p => p.POID == details.POID && p.OpNo == details.OpNo
            && p.PailNumber == int.Parse(details.PailNumber) && p.MPGStatus == null);

            if (qc == null) {
                return new();
            }

            if (qc.QualityOK != 2) {
                qc.MPGStatus = 1;
                qc.MPGRowUpdated = DateTime.Now;

                session.Update(qc);
                transaction.Commit();

                return new();
            }

            var query = (List<Correction>)session.CreateSQLQuery(CorrectionQuery)
                .SetResultTransformer(Transformers.AliasToBean<Correction>())
                .SetInt64("id", qc.ID).List<Correction>();

            query.ForEach(item => {
                item.MPGStatus = 1;
                item.MPGRowUpdated = DateTime.Now;
                session.Update(item);
            });

            transaction.Commit();
            return query;
        }

        public void ChangeStatus(string POID, string pailIndex, string status) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var pail = session.Query<ProductionOrderPailStatus>().First(p => p.POID == POID && p.PailNumber == pailIndex);
            pail.PailStatus = status;

            session.Update(pail);
            transaction.Commit();
        }

        public ProductionOrder BlockCommand(string POID) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var po = session.Query<ProductionOrder>().FirstOrDefault(p => p.POID == POID);

            if (po == null) {
                return null;
            }

            po.Status = "BLOC";
            po.Priority = "-1";
            po.MPGStatus = 1;
            po.MPGRowUpdated = DateTime.Now;

            session.Update(po);
            transaction.Commit();

            return po;
        }

        public Tuple<InputData, List<ProductionOrderPailStatus>> GetCommandData(StartCommand command) {
            string poid = command.POID;
            InputData data = new();

            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            data.Order = session.Query<ProductionOrder>().First(p => p.POID == poid);
            data.DataUOMS = session.Query<MaterialDataUOMS>().Where(p => p.MaterialID == data.Order.MaterialID).ToList();
            data.OrderBOM = session.Query<ProductionOrderBom>().Where(p => p.POID == poid).ToList();
            data.OrderFinalItem = session.Query<ProductionOrderFinalItem>().Where(p => p.POID == poid).ToList();
            data.LotHeader = session.Query<ProductionOrderLotHeader>().First(p => p.POID == poid);

            data.Order.MPGRowUpdated = DateTime.Now;
            data.Order.MPGStatus = 1;

            var pails = data.CreatePails(command);

            pails.ForEach(pail => session.Save(pail));
            session.Update(data.Order);
            transaction.Commit();

            return Tuple.Create(data, pails);
        }

        public void SendPartialProduction(string POID) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var trigger = SapTransfer.CreateRecord(POID);
            _ = session.Save(trigger);
            transaction.Commit();
        }

        public List<StockVessel> GetStockVessels() {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<StockVessel>().ToList();
        }

        public void CloseCommand(string POID) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var po = session.Query<ProductionOrder>().First(p => p.POID == POID);
            po.Status = Resources.CMD_DONE;
            po.Priority = "-1";

            session.Save(SapTransfer.CreateRecord(POID));
            session.Update(po);
            transaction.Commit();
        }

        public List<RiskPhrase> GetRiskPhrases() {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<RiskPhrase>().ToList();
        }

        public Tuple<List<AlternativeName>, List<MaterialData>, List<Classification>> GetMaterials() {
            using var session = MesDb.Instance.GetSession();
            using var transacttion = session.BeginTransaction();

            var materials = session.Query<MaterialData>().Where(p => p.MPGStatus == null).ToList();
            var alternative = session.Query<AlternativeName>().ToList();
            var clasification = session.Query<Classification>().ToList();

            /*materials.ForEach(item => {
                item.MPGStatus = 1;
                item.MPGRowUpdated = DateTime.Now;
                session.Update(item);
            });/**/

            transacttion.Commit();

            return Tuple.Create(alternative, materials, clasification);
        }
    }
}
