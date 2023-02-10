using DataEntity.Model.Input;
using DataEntity.Model.Types;
using DataEntity.Config;

using System.Collections.Generic;
using System.Linq;
using System;

using Microsoft.AspNetCore.Mvc;

using MES_Service.Interface;
using NHibernate;

namespace MES_Service.Repository {

    public class MesCommandRepository : ICommandRepository {

        public IEnumerable<ProductionOrder> GetCommands(DateTime startDate, DateTime endDate) {
            List<ProductionOrder> orders;
            using (var session = MesDb.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    orders = session.Query<ProductionOrder>().Where(p => p.Status == "ELB").ToList();
                }
            }

            using (var session = SqliteDB.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    orders.AddRange(session.Query<ProductionOrder>().ToList());
                }
            }

            return orders;
        }

        public ProductionOrder GetCommands(string POID) {
            using (var session = SqliteDB.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    return session.Query<ProductionOrder>().SingleOrDefault(p => p.POID == POID);
                }
            }
        }

        public void StartCommand(string POID) {
            InputData data = new();

            using (ISession session = MesDb.Instance.GetSession()) {
                using ITransaction transaction = session.BeginTransaction();
                data.Order = session.Query<ProductionOrder>().First(p => p.POID == POID);
                data.DataUOMS = session.Query<MaterialDataUOMS>().Where(p => p.MaterialID == data.Order.MaterialID).ToList();
                data.OrderBOM = session.Query<ProductionOrderBom>().Where(p => p.POID == POID).ToList();
                data.OrderFinalItem = session.Query<ProductionOrderFinalItem>().Where(p => p.POID == POID).ToList();
                data.LotDetails = session.Query<ProductionOrderLotDetail>().Where(p => p.POID == POID).ToList();
                data.LotHeader = session.Query<ProductionOrderLotHeader>().First(p => p.POID == POID);
            }

            using (ISession session = MpgDb.Instance.GetSession()) {
                using ITransaction transaction = session.BeginTransaction();
                session.Save(data.Order);
                data.DataUOMS.ForEach(item => session.Save(item));
                data.OrderBOM.ForEach(item => session.Save(item));
                data.OrderFinalItem.ForEach(item => session.Save(item));
                data.LotDetails.ForEach(item => session.Save(item));
                session.Save(data.LotHeader);

                transaction.Commit();
            }
        }

        public bool CheckPriority(string Priority) {
            using ISession session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();
            return session.Query<ProductionOrder>().Any(p => p.Priority == Priority);
        }

        public string GetQC(string POID) {
            using ISession session = MesDb.Instance.GetSession();
            using ITransaction transaction = session.BeginTransaction();
            return session.Query<ProductionOrderLotHeader>().FirstOrDefault(p => p.POID == POID)?.PozQC;
        }

        public ActionResult<bool> BlockCommand(string POID) {
            throw new NotImplementedException();
        }

        public ActionResult<bool> CloseCommand(string POID) {
            throw new NotImplementedException();
        }

        public ActionResult<bool> PartialProduction(string pOID) {
            throw new NotImplementedException();
        }

        public ActionResult<bool> DownloadMaterials() {
            throw new NotImplementedException();
        }
    }
}
