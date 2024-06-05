using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Mpg;
using MpgWebService.Presentation.Request.MPG;
using MpgWebService.Presentation.Response;
using MpgWebService.Business.Data.Utils;
using MpgWebService.Data.Wrappers;
using MpgWebService.Properties;

using DataEntity.Model.Output;
using DataEntity.Model.Input;
using DataEntity.Model.Types;
using DataEntity.Config;

using System.Collections.Generic;
using System.Linq;
using System;

using NHibernate.Transform;
using NHibernate.Util;

namespace MpgWebService.Repository.Clients {

    public class MpgClient {

        private readonly string DOSAGE_MATERIALS = "SELECT stock.MpgHead, bom.Item, bom.ItemQty, bom.ItemQtyUOM " +
            "FROM MES2MPG_StockVessel stock FULL OUTER JOIN MES2MPG_ProductionOrderBOM bom " +
            "ON stock.MaterialID = bom.Item " +
            "WHERE bom.POID = ?";

        private readonly string GET_FIRST_PAIL = "SELECT pails.POID, orders.Priority, pails.PailNumber, pails.GrossWeight " +
            "FROM MPG2MES_ProductionOrderPailStatus pails INNER JOIN MES2MPG_ProductionOrders orders " +
            "ON orders.POID = pails.POID " +
            "WHERE orders.Status = 'PRLS' AND pails.QC = '1' AND pails.PailStatus = 'ELB' AND pails.PailNumber = 1 " +
            "ORDER BY oroders.Priority";

        public readonly static MpgClient Client = new();

        public ServiceResponse GetCommands(Period period) => CatchError(() => {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var result = session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= period.StartDate && p.PlannedEndDate <= period.EndDate).ToList();

            return ServiceResponse.CreateResponse(result, "");
        });

        public IResponse SaveCorrection(POConsumption materials, ProductionOrderCorection correction) => CatchError(() => {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            Utils.UpdateMaterials(session, materials);
            session.Save(correction);
            transaction.Commit();

            return Response<int>.Success("Materialele au fost salvate");
        });

        public IResponse GetAvailablePail(string POID) => CatchError(() => {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var pail = session.Query<ProductionOrderPailStatus>().FirstOrDefault(p => p.POID == POID && p.PailStatus == "ELB");
            var order = session.Query<ProductionOrder>().First(p => p.POID == POID);
            var techDetails = session.Query<ProductionOrderTechDetail>().Where(p => p.POID == POID && p.OP_DESCR == "MIXARE_1").ToList();

            var result = PailDto.FromPailStatus(pail, order, techDetails);
            return Response<PailDto>.CreateResponse(result);
        });

        public IResponse GetFirstPail() => CatchError(() => {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var result = session.CreateSQLQuery(GET_FIRST_PAIL)
                .SetResultTransformer(Transformers.AliasToBean<PailQCDto>())
                .List<PailQCDto>().First();

            return Response<PailQCDto>.CreateResponse(result);
        });

        public IResponse SaveDosageMaterials(POConsumption consumption) => CatchError(() => {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var items = consumption.Materials.Select(p => p.Item).ToList();
            var data = session.QueryOver<ProductionOrderBom>()
                .WhereRestrictionOn(p => p.Item)
                .IsIn(items)
                .And(p => p.POID == consumption.POID)
                .List<ProductionOrderBom>().ToList();

            var result = Utils.CreateConsumption(consumption, data);

            result.ForEach(item => session.Save(item));
            transaction.Commit();

            return Response<List<ProductionOrderConsumption>>.CreateResponse(result);
        });

        public IResponse GetCommand(string POID) => CatchError(() => {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var result = session.Query<ProductionOrder>().FirstOrDefault(p => p.POID == POID);
            return Response<ProductionOrder>.CreateResponse(result);
        });

        public IResponse CheckPriority(string priority) => CatchError(() => {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var result = !session.Query<ProductionOrder>().Any(p => p.Priority == priority);
            return Response<bool>.CreateResponse(result);
        });

        public IResponse GetQc(string POID) => CatchError(() => {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var result = session.Query<ProductionOrderLotHeader>().FirstOrDefault(p => p.POID == POID)?.PozQC;
            return Response<string>.CreateResponse(result);
        });

        public IResponse GetMaterials(string POID) => CatchError(() => {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var quantity = session.Query<ProductionOrder>().First(p => p.POID == POID).PlannedQtyBUC;
            var materials = session.CreateSQLQuery(DOSAGE_MATERIALS)
                .SetResultTransformer(Transformers.AliasToBean<MaterialDto>())
                .SetString(0, POID).List<MaterialDto>().Where(p => p.ItemQty != 1).ToList();

            materials.ForEach(item => item.ItemQty /= quantity);
            return Response<List<MaterialDto>>.CreateResponse(materials);
        });

        public IResponse GetLabelData(string POID) => CatchError(() => {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var result = new LabelDto();
            var production = session.Query<ProductionOrder>().First(p => p.POID == POID);
            var material = session.Query<MaterialData>().First(p => p.MaterialID == production.MaterialID);
            var pail = session.Query<ProductionOrderPailStatus>().First(p => p.POID == POID);
            var details = session.Query<ProductionOrderLotDetail>().First(p => p.POID == POID);

            result.SetMaterialDetails(material, pail);
            result.SetProductionDetails(production);
            result.SetLimits(details);

            result.RiskPhrases = session.Query<RiskPhrase>().FirstOrDefault(p => p.Material == material.MaterialID);

            result.ProductOrigin = session.Query<Classification>().FirstOrDefault(p => p.Param == "0000000028"
                && p.ParamDescr == "MEDIU DISPERSIE" && p.MaterialID == material.MaterialID);

            result.Diluent = session.Query<Classification>().FirstOrDefault(p => p.Param == "0000000025"
                && p.ParamDescr == "DILUANT RECOMANDAT" && p.MaterialID == material.MaterialID);

            result.Temperature = session.Query<Classification>().FirstOrDefault(p => p.Param == "0000000015"
                && p.ParamDescr == "CONDITII DE DEPOZITARE" && p.MaterialID == material.MaterialID);

            result.Hardener = session.Query<Classification>().FirstOrDefault(p => p.Param == "0000000029"
                && p.ParamDescr == "INTARITOR UNIVERSAL" && p.MaterialID == material.MaterialID);

            result.Winter = session.Query<Classification>().FirstOrDefault(p => p.Param == "0000000030"
                && p.ParamDescr == "INTARITOR IARNA" && p.MaterialID == material.MaterialID);

            return Response<LabelDto>.CreateResponse(result);
        });

        public IResponse BlockCommand(string POID) => CatchError(() => {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var po = session.Query<ProductionOrder>().FirstOrDefault(p => p.POID == POID);
            if (po == null) {
                return Response<int>.CreateResponse(2);
            }

            var count = session.Query<ProductionOrderPailStatus>().Count(p => p.POID == POID && p.PailStatus == Settings.Default.CMD_SEND);

            if (po.PlannedQtyBUC != count) {
                return Response<int>.CreateResponse(1);
            }

            po.Status = Settings.Default.CMD_BLOCKED;
            po.Priority = "-1";
            po.MPGStatus = 1;
            po.MPGRowUpdated = DateTime.Now;

            session.Update(po);
            transaction.Commit();

            return Response<int>.CreateResponse(0);
        });

        public void SetQC(QcLabelDto label) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var pail = session.Query < ProductionOrderPailStatus>().First(p => p.POID == label.POID && p.PailNumber == label.PailNumber);

            pail.PailStatus = "PRLQ";
            pail.Op_No = label.OpQM;


            session.Update(pail);
            transaction.Commit();
        }

        public ServiceResponse CreateCommand(ProductionOrder po) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            po.Priority = "-1";

            session.Save(po);
            transaction.Commit();

            return ServiceResponse.Ok("Comanda a fost salvata");
        }

        public ServiceResponse CloseCommand(string POID) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var result = session.Query<ProductionOrder>().First(p => p.POID == POID);
            result.Status = Settings.Default.CMD_DONE;
            result.Priority = "-1";

            session.Update(result);
            transaction.Commit();

            return ServiceResponse.CreateOkResponse("Comanda a fost inchisa");
        }

        public Tuple<ProductionOrder, List<ProductionOrderPailStatus>, List<ProductionOrderBom>, string> PartialMaterials(string POID) {
            using var session = MpgDb.Instance.GetSession();
            using var transaction = session.BeginTransaction();

            var po = session.Query<ProductionOrder>().First(p => p.POID == POID);
            var pails = session.Query<ProductionOrderPailStatus>().Where(p => p.POID == POID && p.PailStatus == Settings.Default.CMD_DONE).ToList();
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
