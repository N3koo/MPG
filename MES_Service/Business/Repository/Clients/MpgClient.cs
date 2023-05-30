using MpgWebService.Presentation.Request;
using MpgWebService.Properties;

using DataEntity.Model.Output;
using DataEntity.Model.Input;
using DataEntity.Model.Types;
using DataEntity.Config;

using System.Collections.Generic;
using System.Linq;
using System;
using MpgWebService.Presentation.Response;

namespace MpgWebService.Repository.Clients {

    public class MpgClient {

        public readonly static MpgClient Client = new();

        public List<ProductionOrder> GetCommands(Period period) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= period.StartDate && p.PlannedEndDate <= period.EndDate).ToList();
        }

        public void SaveCorrection(ProductionOrderCorection correction) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            session.Save(correction);
            transaction.Commit();
        }

        public ProductionOrderPailStatus GetAvailablePail() {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            throw new NotImplementedException();

        }

        public void ConsumeMaterials(List<POConsumption> materials) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            materials.ForEach(item => {
                session.Save(POConsumption.CreateConsumption(item));
            });

            transaction.Commit();
        }

        public List<LotDetails> GetOperations(string POID) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<ProductionOrderLotDetail>().Where(p => p.POID == POID).OrderBy(p => p.OpNo)
                .GroupBy(p => new { p.OpNo, p.OpDescr })
                .Select(p => new LotDetails { OPNO = p.Key.OpNo, OpDescr = p.Key.OpDescr }).ToList();
        }

        public void SaveDosageMaterials(List<ProductionOrderConsumption> consumption) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            consumption.ForEach(item => {
                session.Save(item);
            });

            transaction.Commit();
        }

        public ProductionOrder GetCommand(string POID) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<ProductionOrder>().FirstOrDefault(p => p.POID == POID);
        }


        public bool CheckPriority(string priority) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return !session.Query<ProductionOrder>().Any(p => p.Priority == priority);
        }

        public string GetQc(string POID) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            return session.Query<ProductionOrderLotHeader>().FirstOrDefault(p => p.POID == POID)?.PozQC;
        }

        public List<ProductionOrderBom> GetMaterials(string POID) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var quantity = session.Query<ProductionOrder>().First(p => p.POID == POID).PlannedQtyBUC;
            var materials = session.Query<ProductionOrderBom>().Where(p => p.POID == POID).ToList();
            materials.ForEach(item => item.ItemQty /= quantity);

            return materials;
        }


        public int BlockCommand(string POID) {
            using var session = MpgDb.Instance.GetSession();
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
            po.Priority = "-1";
            po.MPGStatus = 1;
            po.MPGRowUpdated = DateTime.Now;

            session.Update(po);
            transaction.Commit();

            return 0;
        }

        public ServiceResponse CreateCommand(ProductionOrder po) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            po.Priority = "-1";

            session.Save(po);
            transaction.Commit();

            return ServiceResponse.CreateOkResponse("Comanda a fost salvata");
        }

        public ServiceResponse CloseCommand(string POID) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var result = session.Query<ProductionOrder>().First(p => p.POID == POID);
            result.Status = Resources.CMD_DONE;
            result.Priority = "-1";

            session.Update(result);
            transaction.Commit();

            return ServiceResponse.CreateOkResponse("Comanda a fost inchisa");
        }

        public Tuple<ProductionOrder, List<ProductionOrderPailStatus>, List<ProductionOrderBom>, string> PartialMaterials(string POID) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var po = session.Query<ProductionOrder>().First(p => p.POID == POID);
            var pails = session.Query<ProductionOrderPailStatus>().Where(p => p.POID == POID && p.PailStatus == Resources.CMD_DONE).ToList();
            var position = session.Query<ProductionOrderFinalItem>().First(p => p.POID == POID).ItemPosition;
            var ldm = session.Query<ProductionOrderBom>().Where(p => p.POID == POID).ToList();
            return Tuple.Create(po, pails, ldm, position);
        }

        public ServiceResponse UpdateTickets(Tuple<ProductionOrder, List<ProductionOrderPailStatus>, List<ProductionOrderBom>, string> tuple) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            tuple.Item2.ForEach(pail => {
                session.Update(pail);
            });

            transaction.Commit();

            return ServiceResponse.CreateOkResponse("Materialele au fost actulizate");
        }

        public void StartCommand(Tuple<InputData, List<ProductionOrderPailStatus>> data) {
            var po = data.Item1;

            using var session = MpgDb.Instance.GetSession();
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
        }

        public ServiceResponse SaveOrUpdateMaterials(Tuple<List<AlternativeName>, List<MaterialData>, List<Classification>> tuple) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            tuple.Item1.ForEach(name => {
                var localName = session.Query<AlternativeName>().FirstOrDefault(p => p.MaterialID == name.MaterialID && p.Language == name.Language);

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


            tuple.Item3.ForEach(classification => {
                var localClasification = session.Query<Classification>().FirstOrDefault(p => p.MaterialID == classification.MaterialID &&
                        p.Param == classification.Param && p.Value == classification.Value);

                if (localClasification == null) {
                    _ = session.Save(classification);
                } else {
                    localClasification.SetDetails(classification);
                    session.Update(localClasification);
                }
            });

            transaction.Commit();

            return ServiceResponse.CreateOkResponse("Materialele au fost actualizate");
        }

        public void SaveOrUpdateRiskPhrases(List<RiskPhrase> phrases) {
            using var session = MpgDb.Instance.GetSession();
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

        public void SaveOrUpdateStockVesel(List<StockVessel> vessels) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            vessels.ForEach(vessel => {
                var local = session.Query<StockVessel>().FirstOrDefault(p => p.MaterialID == vessel.MaterialID);

                if (local == null) {
                    session.Save(vessel);
                } else {
                    local.SetDetails(vessel);
                    session.Update(local);
                }
            });

            transaction.Commit();
        }

        public void ChangeStatus(string POID, string pailIndex, string status) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var pail = session.Query<ProductionOrderPailStatus>().First(p => p.POID == POID && p.PailNumber == pailIndex);
            pail.PailStatus = status;

            session.Update(pail);
            transaction.Commit();
        }
    }
}
