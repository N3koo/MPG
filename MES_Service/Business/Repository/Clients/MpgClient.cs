using MpgWebService.Presentation.Request;
using MpgWebService.Properties;
using DataEntity.Model.Output;
using DataEntity.Model.Input;
using DataEntity.Model.Types;
using DataEntity.Config;

using System.Collections.Generic;
using System.Linq;
using System;
using MpgWebService.Business.Data.DTO;
using System.Diagnostics;

namespace MpgWebService.Repository.Clients {

    public class MpgClient {

        public readonly static MpgClient Client = new();

        public List<ProductionOrder> GetCommands(Period period) => CatchException(() => {
            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= period.StartDate && p.PlannedEndDate <= period.EndDate).ToList();
        });

        public ProductionOrder GetCommand(string POID) => CatchException(() => {
            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<ProductionOrder>().FirstOrDefault(p => p.POID == POID);
        });


        public bool CheckPriority(string priority) => CatchException(() => {
            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<ProductionOrder>().Any(p => p.Priority == priority);
        });

        public string GetQc(string POID) => CatchException(() => {
            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<ProductionOrderLotHeader>().FirstOrDefault(p => p.POID == POID)?.PozQC;
        });

        public int BlockCommand(string POID) => CatchException(() => {
            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var po = session.Query<ProductionOrder>().FirstOrDefault(p => p.POID == POID);
            if (po == null) {
                return 2;
            }

            var count = session.Query<ProductionOrderPailStatus>().Count(p => p.POID == POID && p.PailStatus == Resources.CMD_SEND);

            if (po.PlannedQtyBUC != count) {
                return 1;
            }

            po.Status = Resources.CMD_BLOCKED;

            session.Update(po);
            transaction.Commit();

            return 0;
        });

        public Response CreateCommand(ProductionOrder po) => CatchException(() => {
            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            session.Save(po);
            transaction.Commit();

            return Response.CreateOkResponse("Comanda a fost salvata");
        });

        public Response CloseCommand(string POID) => CatchException(() => {
            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var result = session.Query<ProductionOrder>().First(p => p.POID == POID);
            result.Status = Resources.CMD_DONE;

            session.Update(result);
            transaction.Commit();

            return Response.CreateOkResponse("Comanda a fost inchisa");
        });

        public Tuple<ProductionOrder, List<ProductionOrderPailStatus>, List<ProductionOrderBom>, string> PartialMaterials(string POID) => CatchException(() => {
            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var po = session.Query<ProductionOrder>().First(p => p.POID == POID);
            var pails = session.Query<ProductionOrderPailStatus>().Where(p => p.POID == POID && p.PailStatus == Resources.CMD_DONE).ToList();
            var position = session.Query<ProductionOrderFinalItem>().First(p => p.POID == POID).ItemPosition;
            var ldm = session.Query<ProductionOrderBom>().Where(p => p.POID == POID).ToList();
            return Tuple.Create(po, pails, ldm, position);
        });

        public Response UpdateTickets(Tuple<ProductionOrder, List<ProductionOrderPailStatus>, List<ProductionOrderBom>, string> tuple) => CatchException(() => {
            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            tuple.Item2.ForEach(pail => {
                session.Update(pail);
            });

            transaction.Commit();

            return Response.CreateOkResponse("Materialele au fost actulizate");
        });

        public Response StartCommand(Tuple<InputData, List<ProductionOrderPailStatus>> data) => CatchException(() => {
            var po = data.Item1;

            using var session = SqliteDB.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            session.Save(po.Order);
            po.DataUOMS.ForEach(item => session.Save(item));
            po.OrderBOM.ForEach(item => session.Save(item));
            po.OrderFinalItem.ForEach(item => session.Save(item));
            po.LotDetails.ForEach(item => session.Save(item));
            session.Save(po.LotHeader);

            data.Item2.ForEach(pail => {
                session.Save(pail);
            });

            transaction.Commit();

            return Response.CreateOkResponse("Comanda a fost creata");
        });

        public Response SaveOrUpdateMaterials(Tuple<List<AlternativeName>, List<MaterialData>, List<Clasification>> tuple) => CatchException(() => {
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

            return Response.CreateOkResponse("Materialele au fost actualizate");
        });

        public Response SaveOrUpdateRiskPhrases(List<RiskPhrase> phrases) => CatchException(() => {
            using var session = SqliteDB.Instance.GetSession();
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

            return Response.CreateOkResponse("Frazele de risc au fost actualizate");
        });

        private T CatchException<T>(Func<T> function) {
            /*try {
                return function();
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }

            return default;/**/
            return function();
        }
    }
}
