using DataEntity.Model.Input;
using DataEntity.Model.Types;
using DataEntity.Config;

using System.Collections.Generic;
using System.Linq;
using System;

namespace MpgWebService.Repository.Clients {

    public class MesClient {

        public List<ProductionOrder> GetCommands(DateTime start, DateTime end) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= start &&
                p.PlannedEndDate <= end && p.Status == "ELB").ToList();
        }

        public bool BlockCommand(string POID) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var po = session.Query<ProductionOrder>().First(p => p.POID == POID);
            po.Status = "BLOC";

            session.Update(po);
            transaction.Commit();

            return true;
        }

        public InputData GetCommandData(string POID) {
            InputData data = new();

            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            data.Order = session.Query<ProductionOrder>().First(p => p.POID == POID);
            data.DataUOMS = session.Query<MaterialDataUOMS>().Where(p => p.MaterialID == data.Order.MaterialID).ToList();
            data.OrderBOM = session.Query<ProductionOrderBom>().Where(p => p.POID == POID).ToList();
            data.OrderFinalItem = session.Query<ProductionOrderFinalItem>().Where(p => p.POID == POID).ToList();
            data.LotHeader = session.Query<ProductionOrderLotHeader>().First(p => p.POID == POID);

            return data;
        }

    }
}
