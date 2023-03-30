using MpgWebService.Presentation.Request;

using DataEntity.Model.Input;
using DataEntity.Model.Types;
using DataEntity.Config;

using System.Collections.Generic;
using System.Linq;
using System;
using DataEntity.Model.Output;
using NHibernate;
using MpgWebService.Business.Data.Extension;

namespace MpgWebService.Repository.Clients {

    public class MesClient {

        public List<ProductionOrder> GetCommands(Period period) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= period.StartDate &&
                p.PlannedEndDate <= period.EndDate && p.Status == "ELB")
                .Skip((period.PageNumber - 1) * period.PageSize)
                .Take(period.PageSize).ToList();
        }

        public ProductionOrder BlockCommand(string POID) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var po = session.Query<ProductionOrder>().FirstOrDefault(p => p.POID == POID);

            if (po == null) {
                return null;
            }

            po.Status = "BLOC";

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

            var pails = data.CreatePails(session, command);
            transaction.Commit();

            return Tuple.Create(data, pails);
        }

        public bool SendPartialProduction(string POID) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var trigger = SapTransfer.CreateRecord(POID);
            var id = (int)session.Save(trigger);
            transaction.Commit();

            return id != 0;
        }



        public List<RiskPhrase> GetRiskPhrases() {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<RiskPhrase>().ToList();
        }

        public Tuple<List<AlternativeName>, List<MaterialData>, List<Clasification>> GetMaterials() {
            using var session = MesDb.Instance.GetSession();
            using var transacttion = session.BeginTransaction();

            var materials = session.Query<MaterialData>().Where(p => p.MESStatus == 0).ToList();
            var alternative = session.Query<AlternativeName>().Where(p => p.MESStatus == 0).ToList();
            var clasification = session.Query<Clasification>().Where(p => p.MESStatus == 0).ToList();

            materials.ForEach(item => {
                item.MESStatus = 1;
                item.MPGStatus = 1;
                item.MPGRowUpdated = DateTime.Now;
                session.Update(item);
            });

            alternative.ForEach(item => {
                item.MESStatus = 1;
                item.MPGStatus = 1;
                item.MPGRowUpdated = DateTime.Now;
                session.Update(item);
            });

            clasification.ForEach(item => {
                item.MESStatus = 1;
                item.MPGStatus = 1;
                item.MPGRowUpdated = DateTime.Now;
                session.Update(item);
            });

            return Tuple.Create(alternative, materials, clasification);
        }
    }
}
