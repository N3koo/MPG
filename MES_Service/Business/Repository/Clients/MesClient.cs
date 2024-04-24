using MpgWebService.Business.Data.Extension;
using MpgWebService.Presentation.Response;
using MpgWebService.Presentation.Request;
using MpgWebService.Business.Data.Utils;
using MpgWebService.Properties;

using DataEntity.Model.Output;
using DataEntity.Model.Input;
using DataEntity.Model.Types;
using DataEntity.Config;

using System.Collections.Generic;
using System.Linq;
using System;

using NHibernate.Transform;

namespace MpgWebService.Repository.Clients {

    public class MesClient {

        private readonly static string CorrectionQuery = "SELECT stock.MPGHead, correction.RawMaterialID, correction.ItemQuantity, correection.ItemUOM " +
            "FROM MES2MPG_Correction correction INNER JOIN MES2MPG_StockVessel stock ON stock.MaterialID = correction.RawMaterialID " +
            "WHERE correction.CorrectionID = (:id)";

        private readonly static string LabelQuery = "SELECT a.PlantID, a.MaterialID, c.ItemQty, b.StartDate, a.PODescription, a.POID, b.PailNumber, a.KoberLot, b.MES_Sample_ID, b.Op_No, c.OpDescr" +
            "FROM MES2MPG_ProductionOrders a INNER JOIN MPG2MES_ProductionOrderPailStatus b ON a.POID = b.POID INNER JOIN MES2MPG_ProductionOrderFinalItems c ON a.POID = c.POID " +
            "WHERE b.PailNumber = (:pail) AND a.POID = '(:POID)'";

        public static readonly MesClient Client = new();

        public List<ProductionOrder> GetCommands(Period period) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= period.StartDate &&
                p.PlannedEndDate <= period.EndDate && p.Status == "ELB").ToList();
        }

        public void SaveDosageMaterials(List<ProductionOrderConsumption> materials) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            materials.ForEach(item => session.Save(item));
            transaction.Commit();
        }

        public string GetQc(string POID) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<ProductionOrderLotHeader>().First(p => p.POID == POID)?.PozQC;
        }

        public ProductionOrderCorection SaveCorrection(POConsumption materials) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var lastId = session.Query<QualityCheck>().OrderBy(p => p.ID).First();
            var correction = Utils.CreateCorrection(materials, lastId);

            session.Save(correction);
            transaction.Commit();

            return correction;
        }

        public QcLabelDto SetQcStatus(string POID, int pailNumber) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var details = Utils.GetOperations(session, POID);

            foreach(var detail in details) {
                if(Utils.CheckOperation(session, POID, pailNumber, detail.OPNO)) {
                    continue;
                }

                var pail = session.Query<ProductionOrderPailStatus>().First(p => p.POID == POID && p.PailNumber == pailNumber.ToString());

                pail.PailStatus = "PRLQ";
                pail.Op_No = detail.OPNO;
                pail.MESStatus = 0;
                pail.MPGStatus = 1;

                session.Update(pail);
                transaction.Commit();

                var label = session.CreateSQLQuery(LabelQuery)
                    .SetResultTransformer(Transformers.AliasToBean<QcLabelDto>())
                    .SetInt32("pail", pailNumber)
                    .SetString("POID", POID).List<QcLabelDto>();

                return label.Count == 0 ? null : label[0];
            }

            return null;
        }

        public List<MaterialDto> GetCorrections(string POID, int pailNumber, string opNo) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var qc = session.Query<QualityCheck>().FirstOrDefault(p => p.POID == POID && p.OpNo == opNo
            && p.PailNumber == pailNumber && p.MPGStatus == null);

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

            return session.CreateSQLQuery(CorrectionQuery)
                .SetResultTransformer(Transformers.AliasToBean<MaterialDto>())
                .SetInt64("id", qc.ID).List<MaterialDto>().ToList();
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
            data.LotDetails = session.Query<ProductionOrderLotDetail>().Where(p => p.POID == poid).ToList();
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
            po.Status = Settings.Default.CMD_DONE;
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
