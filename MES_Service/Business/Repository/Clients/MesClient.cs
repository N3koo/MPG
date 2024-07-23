using DataEntity.Config;
using DataEntity.Model.Input;
using DataEntity.Model.Output;
using DataEntity.Model.Types;
using MpgWebService.Business.Data.Extension;
using MpgWebService.Business.Data.Utils;
using MpgWebService.Data.Extension;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Request.MPG;
using MpgWebService.Presentation.Response.Mpg;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Properties;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace MpgWebService.Repository.Clients {

    public class MesClient {

        private readonly static string CorrectionQuery = "SELECT stock.MPGHead, correction.RawMaterialID, correction.ItemQuantity, correection.ItemUOM " +
            "FROM MES2MPG_Correction correction INNER JOIN MES2MPG_StockVessel stock ON stock.MaterialID = correction.RawMaterialID " +
            "WHERE correction.CorrectionID = (:id)";

        private readonly static string LabelQuery = "SELECT a.PlantID, a.MaterialID, c.ItemQty, b.StartDate, a.PODescription, a.POID, b.PailNumber, a.KoberLot, b.MES_Sample_ID, b.Op_No, c.OpDescr" +
            "FROM MES2MPG_ProductionOrders a INNER JOIN MPG2MES_ProductionOrderPailStatus b ON a.POID = b.POID INNER JOIN MES2MPG_ProductionOrderFinalItems c ON a.POID = c.POID " +
            "WHERE b.PailNumber = (:pail) AND a.POID = '(:POID)'";

        public static readonly MesClient Client = new();

        public async Task<ServiceResponse> GetCommands(Period period) {
            try {
                using var session = MesDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var result = await session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= period.StartDate &&
                    p.PlannedEndDate <= period.EndDate && p.Status == "ELB").ToListAsync();
                var data = result.Select(p => p.AsProductionDto()).ToList();

                return ServiceResponse.CreateResponse(data, "Nu au fost gasite comenzi in perioada selectata");

            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMes(ex.Message);
            }
        }

        public async Task<ServiceResponse> SaveDosageMaterials(ServiceResponse response) {
            try {
                using var session = MesDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var materials = (List<ProductionOrderConsumption>)response.Data;
                foreach (var material in materials) {
                    var stock = await session.Query<StockVessel>().FirstAsync(p => p.VesselCod == material.Item);
                    stock.ItemQty -= material.ItemQty;

                    await session.UpdateAsync(stock);
                    await session.UpdateAsync(material);
                }
                await transaction.CommitAsync();

                return ServiceResponse.Ok("Materialel au fost salvate");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMes(ex.Message);
            }
        }

        public async Task<ServiceResponse> GetQc(string POID) {
            try {
                using var session = MesDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var header = await session.Query<ProductionOrderLotHeader>().FirstAsync(p => p.POID == POID);
                return ServiceResponse.CreateResponse(header?.PozQC, "Nu exista QC setat");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMes(ex.Message);
            }
        }

        public async Task<ServiceResponse> SaveCorrection(POConsumption materials) {
            try {
                using var session = MesDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var lastId = await session.Query<QualityCheck>().OrderBy(p => p.ID).FirstAsync();
                var correction = Utils.CreateCorrection(materials, lastId);

                foreach (var material in materials.Materials) {
                    var stock = await session.Query<StockVessel>().FirstAsync(p => p.VesselCod == material.Item);
                    stock.ItemQty -= material.ItemQty;
                    await session.UpdateAsync(stock);
                }

                await session.SaveAsync(correction);
                await transaction.CommitAsync();

                return ServiceResponse.Ok(correction);
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMes(ex.Message);
            }
        }

        public async Task<ServiceResponse> SetQcStatus(string POID, int pailNumber) {
            try {
                using var session = MesDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var details = Utils.GetOperations(session, POID);

                foreach (var detail in details) {

                    if (Utils.CheckOperation(session, POID, pailNumber, detail.OPNO)) {
                        continue;
                    }

                    var pail = await session.Query<ProductionOrderPailStatus>().FirstAsync(p => p.POID == POID && p.PailNumber == pailNumber.ToString());

                    pail.PailStatus = "PRLQ";
                    pail.Op_No = detail.OPNO;
                    pail.MESStatus = 0;
                    pail.MPGStatus = 1;

                    await session.UpdateAsync(pail);
                    await transaction.CommitAsync();

                    var label = await session.CreateSQLQuery(LabelQuery)
                        .SetResultTransformer(Transformers.AliasToBean<QcLabelDto>())
                        .SetInt32("pail", pailNumber)
                        .SetString("POID", POID).ListAsync<QcLabelDto>();

                    return ServiceResponse.CreateResponse(label[0], "Nu exista eticheta de QC disponibila");
                }

                return ServiceResponse.NotFound("Nu exista operatii de QC disponibile");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMes(ex.Message);
            }
        }

        public async Task<ServiceResponse> GetCorrections(string POID, int pailNumber, string opNo) {
            try {
                using var session = MesDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var qc = await session.Query<QualityCheck>().FirstOrDefaultAsync(p => p.POID == POID && p.OpNo == opNo
                && p.PailNumber == pailNumber && p.MPGStatus == null);

                if (qc == null) {
                    return new();
                }

                if (qc.QualityOK != 2) {
                    qc.MPGStatus = 1;
                    qc.MPGRowUpdated = DateTime.Now;

                    await session.UpdateAsync(qc);
                    await transaction.CommitAsync();

                    return new();
                }

                var result = await session.CreateSQLQuery(CorrectionQuery)
                    .SetResultTransformer(Transformers.AliasToBean<MaterialDto>())
                    .SetInt64("id", qc.ID).ListAsync<MaterialDto>();

                return ServiceResponse.CreateResponse(result.ToList(), $"Nu exista corectii disponibile pentru comanda {POID}");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMes(ex.Message);
            }
        }

        public async Task<ServiceResponse> ChangeStatus(string POID, string pailIndex, string status) {
            try {
                using var session = MesDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var pail = await session.Query<ProductionOrderPailStatus>().FirstAsync(p => p.POID == POID && p.PailNumber == pailIndex);
                pail.PailStatus = status;

                await session.UpdateAsync(pail);
                await transaction.CommitAsync();

                return ServiceResponse.Ok($"Statusul pentru galeata {pailIndex} din comanda {POID} a fost actualizat");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMes(ex.Message);
            }
        }

        public async Task<ServiceResponse> BlockCommand(string POID) {
            try {
                using var session = MesDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var po = await session.Query<ProductionOrder>().FirstOrDefaultAsync(p => p.POID == POID);

                if (po == null) {
                    return ServiceResponse.NotFound($"Comanda {POID} nu exista");
                }

                po.Status = "BLOC";
                po.Priority = "-1";
                po.MPGStatus = 1;
                po.MPGRowUpdated = DateTime.Now;

                await session.UpdateAsync(po);
                await transaction.CommitAsync();

                return ServiceResponse.Ok($"Comanda {POID} a fost inchisa");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMes(ex.Message);
            }
        }

        public async Task<ServiceResponse> GetCommandData(StartCommand command) {
            try {
                string poid = command.POID;
                InputData data = new();

                using var session = MesDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                data.Order = await session.Query<ProductionOrder>().FirstAsync(p => p.POID == poid);
                data.DataUOMS = await session.Query<MaterialDataUOMS>().Where(p => p.MaterialID == data.Order.MaterialID).ToListAsync();
                data.OrderBOM = await session.Query<ProductionOrderBom>().Where(p => p.POID == poid).ToListAsync();
                data.OrderFinalItem = await session.Query<ProductionOrderFinalItem>().Where(p => p.POID == poid).ToListAsync();
                data.LotDetails = await session.Query<ProductionOrderLotDetail>().Where(p => p.POID == poid).ToListAsync();
                data.LotHeader = await session.Query<ProductionOrderLotHeader>().FirstAsync(p => p.POID == poid);

                data.Order.MPGRowUpdated = DateTime.Now;
                data.Order.MPGStatus = 1;

                var pails = data.CreatePails(command);

                pails.ForEach(async pail => await session.SaveAsync(pail));
                await session.UpdateAsync(data.Order);
                await transaction.CommitAsync();

                var result = Tuple.Create(data, pails);
                return ServiceResponse.Ok(result);
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMes(ex.Message);
            }
        }

        public async Task<ServiceResponse> SendPartialProduction(string POID) {
            try {
                using var session = MesDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var trigger = SapTransfer.CreateRecord(POID);
                _ = await session.SaveAsync(trigger);
                await transaction.CommitAsync();

                return ServiceResponse.Ok($"Predare partiala pentru comanda {POID} a fost inceputa");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMes(ex.Message);
            }
        }

        public async Task<ServiceResponse> GetStockVessels() {
            try {
                using var session = MesDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var stocks = await session.Query<StockVessel>().ToListAsync();
                return ServiceResponse.CreateResponse(stocks, "Nu exista vase de stocare definite");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMes(ex.Message);
            }
        }

        public async Task<ServiceResponse> CloseCommand(string POID) {
            try {
                using var session = MesDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var po = await session.Query<ProductionOrder>().FirstAsync(p => p.POID == POID);
                po.Status = Settings.Default.CMD_DONE;
                po.Priority = "-1";

                await session.SaveAsync(SapTransfer.CreateRecord(POID));
                await session.UpdateAsync(po);
                await transaction.CommitAsync();

                return ServiceResponse.Ok($"Comanda {POID} a fost inchisa");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMes(ex.Message);
            }
        }

        public async Task<ServiceResponse> GetRiskPhrases() {
            try {
                using var session = MesDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var risks = await session.Query<RiskPhrase>().ToListAsync();
                return ServiceResponse.CreateResponse(risks, "Nu exista fraze de risk disponibile");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMes(ex.Message);
            }
        }

        public async Task<ServiceResponse> GetMaterials() {
            try {
                using var session = MesDb.Instance.GetSession();
                using var transacttion = session.BeginTransaction();

                var materials = await session.Query<MaterialData>().Where(p => p.MPGStatus == null).ToListAsync();
                var alternative = await session.Query<AlternativeName>().ToListAsync();
                var clasification = await session.Query<Classification>().ToListAsync();

                materials.ForEach(async item => {
                    item.MPGStatus = 1;
                    item.MPGRowUpdated = DateTime.Now;
                    await session.UpdateAsync(item);
                });

                await transacttion.CommitAsync();

                var result = Tuple.Create(alternative, materials, clasification);
                return ServiceResponse.Ok(result);
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMes(ex.Message);
            }
        }

        public async Task<ServiceResponse> GetCoefficients() {
            try {
                using var session = MesDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var stocks = await session.Query<StockVessel>().ToListAsync();
                var result = stocks.Select(x => CoefficientDto.FromStockVessel(x)).ToList();

                return ServiceResponse.Ok(result);
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMes(ex.Message);
            }
        }

        public async Task<ServiceResponse> ReserveQuantities(ReserveTank[] quantities) {
            try {
                using var session = MesDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                foreach (var quantity in quantities) {
                    var tank = await session.Query<TankManagement>().FirstAsync(p => p.Tank_Name == quantity.MpgHead);
                    tank.Tank_Value += quantity.Quantity;

                    await session.UpdateAsync(tank);
                }

                await transaction.CommitAsync();
                return ServiceResponse.Ok("Cantitatile au fost actualizate");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMes(ex.Message);
            }
        }
    }
}