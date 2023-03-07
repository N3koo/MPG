using DataEntity.Model.Output;
using DataEntity.Model.Input;
using DataEntity.Model.Types;
using DataEntity.Config;

using System.Collections.Generic;
using System.Linq;
using System;

namespace MpgWebService.Repository.Clients {

    public class MpgClient {

        public readonly static MpgClient Client = new();

        public List<ProductionOrder> GetCommands(DateTime start, DateTime end) {
            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= start && p.PlannedEndDate <= end).ToList();
        }

        public ProductionOrder GetCommand(string POID) {
            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<ProductionOrder>().FirstOrDefault(p => p.POID == POID);
        }

        public bool CheckPriority(string priority) {
            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<ProductionOrder>().Any(p => p.Priority == priority);
        }

        public string GetQc(string POID) {
            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<ProductionOrderLotHeader>().FirstOrDefault(p => p.POID == POID)?.PozQC;
        }

        public bool BlockCommand(string POID) {
            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var po = session.Query<ProductionOrder>().First(p => p.POID == POID);
            var count = session.Query<ProductionOrderPailStatus>().Count(p => p.POID == POID && p.PailStatus == "ELB");

            if (po.PlannedQtyBUC != count) {
                return false;
            }

            po.Status = "BLOC";

            session.Update(po);
            transaction.Commit();

            return true;
        }

        public bool CloseCommand(string POID) {
            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var result = session.Query<ProductionOrder>().First(p => p.POID == POID);
            result.Status = "PRLT";

            session.Update(result);
            transaction.Commit();

            return true;
        }

        public Tuple<ProductionOrder, List<ProductionOrderPailStatus>, List<ProductionOrderBom>, string> PartialMaterials(string POID) {
            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var po = session.Query<ProductionOrder>().First(p => p.POID == POID);
            var pails = session.Query<ProductionOrderPailStatus>().Where(p => p.POID == POID && p.PailStatus == "PRLT").ToList();
            var position = session.Query<ProductionOrderFinalItem>().First(p => p.POID == POID).ItemPosition;
            var ldm = session.Query<ProductionOrderBom>().Where(p => p.POID == POID).ToList();
            return Tuple.Create(po, pails, ldm, position);
        }

        public void UpdateTickets(Tuple<ProductionOrder, List<ProductionOrderPailStatus>, List<ProductionOrderBom>, string> tuple) {
            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            tuple.Item2.ForEach(pail => {
                session.Update(pail);
            });

            transaction.Commit();
        }

        public void StartCommand(InputData data) {
            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            session.Save(data.Order);
            data.DataUOMS.ForEach(item => session.Save(item));
            data.OrderBOM.ForEach(item => session.Save(item));
            data.OrderFinalItem.ForEach(item => session.Save(item));
            data.LotDetails.ForEach(item => session.Save(item));
            session.Save(data.LotHeader);

            transaction.Commit();
        }

        public void SaveOrUpdateMaterials(Tuple<List<AlternativeName>, List<MaterialData>, List<Clasification>> tuple) {
            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            tuple.Item1.ForEach(name => {
                var localName = session.Query<AlternativeName>().FirstOrDefault(p => p.MaterialID == name.MaterialID);

                if (localName == null) {
                    _ = session.Save(name);
                } else {
                    localName.SetDetails(name);
                    session.Update(localName);
                }
            });

            tuple.Item2.ForEach(material => {
                var localMaterial = session.Query<MaterialData>().FirstOrDefault(p => p.MaterialID == material.MaterialID);

                if (localMaterial == null) {
                    _ = session.Save(material);
                } else {
                    localMaterial.SetDetails(material);
                    session.Update(localMaterial);
                }
            });


            tuple.Item3.ForEach(clasification => {
                var localClasification = session.Query<Clasification>().FirstOrDefault(p => p.MaterialID == clasification.MaterialID);

                if (localClasification == null) {
                    _ = session.Save(clasification);
                } else {
                    localClasification.SetDetails(clasification);
                    session.Update(localClasification);
                }
            });

            transaction.Commit();
        }

        public void SaveOrUpdateRiskPhrases(List<RiskPhrase> phrases) {
            using var session = MesDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            phrases.ForEach(item => {
                var phrase = session.Query<RiskPhrase>().FirstOrDefault(p => p.Material == item.Material);

                if (phrase == null) {
                    session.Save(item);
                } else {
                    phrase.Update(item);
                    session.Update(phrase);
                }
            });

            transaction.Commit();
        }
    }
}
