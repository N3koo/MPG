using DataEntity.Config;
using DataEntity.Model.Input;
using DataEntity.Model.Output;
using MpgWebService.Business.Data.JoinData;
using MpgWebService.Business.Data.Utils;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Request.MPG;
using MpgWebService.Presentation.Response.Mpg;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Properties;
using NHibernate.Linq;
using NHibernate.Transform;
using NHibernate.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            "ORDER BY orders.Priority";

        public readonly static MpgClient Client = new();

        public virtual async Task<ServiceResponse<IList<ProductionOrder>>> GetCommands(Period period) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var result = await session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= period.StartDate && p.PlannedEndDate <= period.EndDate).ToListAsync();

                return ServiceResponse<IList<ProductionOrder>>.CreateResponse(result, "Nu exista inregistrari in perioada selectata");
            } catch (Exception ex) {
                return ServiceResponse<IList<ProductionOrder>>.CreateErrorMpg(ex.Message);
            }
        }

        public virtual async Task<ServiceResponse<bool>> SaveCorrection(POConsumption materials, ProductionOrderCorection correction) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                Utils.UpdateMaterials(session, materials);
                await session.SaveAsync(correction);
                await transaction.CommitAsync();

                return ServiceResponse<bool>.Ok(true);
            } catch (Exception ex) {
                return ServiceResponse<bool>.CreateErrorMpg(ex.Message);
            }
        }

        public virtual async Task<ServiceResponse<PailDto>> GetAvailablePail(string POID) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var pail = await session.Query<ProductionOrderPailStatus>().FirstOrDefaultAsync(p => p.POID == POID && p.PailStatus == "ELB");
                var order = await session.Query<ProductionOrder>().FirstAsync(p => p.POID == POID);
                var techDetails = await session.Query<ProductionOrderTechDetail>().Where(p => p.POID == POID && p.OP_DESCR == "MIXARE_1").ToListAsync();

                var result = PailDto.FromPailStatus(pail, order, techDetails);
                return ServiceResponse<PailDto>.CreateResponse(result, "Nu exista galeti disponibile");
            } catch (Exception ex) {
                return ServiceResponse<PailDto>.CreateErrorMpg(ex.Message);
            }
        }

        public virtual async Task<ServiceResponse<PailQCDto>> GetFirstPail() {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var result = await session.CreateSQLQuery(GET_FIRST_PAIL)
                    .SetResultTransformer(Transformers.AliasToBean<PailQCDto>())
                    .ListAsync<PailQCDto>();

                return ServiceResponse<PailQCDto>.CreateResponse(result.First(), "Nu exista comenzi active");
            } catch (Exception ex) {
                return ServiceResponse<PailQCDto>.CreateErrorMpg(ex.Message);
            }
        }

        public virtual async Task<ServiceResponse<IList<ProductionOrderConsumption>>> SaveDosageMaterials(POConsumption consumption) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var items = consumption.Materials.Select(p => p.Item).ToList();
                var data = await session.QueryOver<ProductionOrderBom>()
                    .WhereRestrictionOn(p => p.Item)
                    .IsIn(items)
                    .And(p => p.POID == consumption.POID)
                    .ListAsync<ProductionOrderBom>();

                var result = Utils.CreateConsumption(consumption, data.ToList());

                result.ForEach(async item => await session.SaveAsync(item));
                await transaction.CommitAsync();

                return ServiceResponse<IList<ProductionOrderConsumption>>.Ok(result);
            } catch (Exception ex) {
                return ServiceResponse<IList<ProductionOrderConsumption>>.CreateErrorMpg(ex.Message);
            }
        }

        public virtual async Task<ServiceResponse<ProductionOrder>> GetCommand(string POID) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var result = await session.Query<ProductionOrder>().FirstOrDefaultAsync(p => p.POID == POID);
                return ServiceResponse<ProductionOrder>.CreateResponse(result, $"Comanda nu exista {POID}");
            } catch (Exception ex) {
                return ServiceResponse<ProductionOrder>.CreateErrorMpg(ex.Message);
            }
        }

        public virtual async Task<ServiceResponse<bool>> CheckPriority(string priority) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var result = await session.Query<ProductionOrder>().AnyAsync(p => p.Priority == priority);
                return ServiceResponse<bool>.CreateResponse(!result, "Nu exista o comanda ce are prioritatea data");
            } catch (Exception ex) {
                return ServiceResponse<bool>.CreateErrorMpg(ex.Message);
            }
        }

        public virtual async Task<ServiceResponse<string>> GetQc(string POID) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var result = await session.Query<ProductionOrderLotHeader>().FirstOrDefaultAsync(p => p.POID == POID);
                return ServiceResponse<string>.CreateResponse(result?.PozQC, "Nu exista comanda");
            } catch (Exception ex) {
                return ServiceResponse<string>.CreateErrorMpg(ex.Message);
            }
        }

        public virtual async Task<ServiceResponse<IList<MaterialDto>>> GetMaterials(string POID) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var quantity = (await session.Query<ProductionOrder>().FirstAsync(p => p.POID == POID)).PlannedQtyBUC;
                var materials = await session.CreateSQLQuery(DOSAGE_MATERIALS)
                    .SetResultTransformer(Transformers.AliasToBean<MaterialDto>())
                    .SetString(0, POID).ListAsync<MaterialDto>();

                materials.Where(p => p.ItemQty != 1).ToList().ForEach(item => item.ItemQty /= quantity);
                return ServiceResponse<IList<MaterialDto>>.CreateResponse(materials.ToList(), "Nu exista materiale pentru comanda selectata");
            } catch (Exception ex) {
                return ServiceResponse<IList<MaterialDto>>.CreateErrorMpg(ex.Message);
            }
        }

        public virtual async Task<ServiceResponse<LabelDto>> GetLabelData(string POID) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var result = new LabelDto();
                var production = await session.Query<ProductionOrder>().FirstAsync(p => p.POID == POID);
                var material = await session.Query<MaterialData>().FirstAsync(p => p.MaterialID == production.MaterialID);
                var pail = await session.Query<ProductionOrderPailStatus>().FirstAsync(p => p.POID == POID);
                var details = await session.Query<ProductionOrderLotDetail>().FirstAsync(p => p.POID == POID);

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

                return ServiceResponse<LabelDto>.CreateResponse(result, "Nu exista eticheta pentru comanda selectata");
            } catch (Exception ex) {
                return ServiceResponse<LabelDto>.CreateErrorMpg(ex.Message);
            }
        }

        public virtual async Task<ServiceResponse<bool>> BlockCommand(string POID) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var po = await session.Query<ProductionOrder>().FirstOrDefaultAsync(p => p.POID == POID);
                if (po == null) {
                    return ServiceResponse<bool>.CreateErrorMpg($"Comanda {POID} nu exista");
                }

                var count = await session.Query<ProductionOrderPailStatus>().CountAsync(p => p.POID == POID && p.PailStatus == Settings.Default.CMD_SEND);

                if (po.PlannedQtyBUC != count) {
                    return ServiceResponse<bool>.CreateErrorMpg($"Comanda {POID} a fost inceputa");
                }

                po.Status = Settings.Default.CMD_BLOCKED;
                po.Priority = "-1";
                po.MPGStatus = 1;
                po.MPGRowUpdated = DateTime.Now;

                await session.UpdateAsync(po);
                await transaction.CommitAsync();

                return ServiceResponse<bool>.Ok(true);
            } catch (Exception ex) {
                return ServiceResponse<bool>.CreateErrorMpg(ex.Message);
            }

        }

        public virtual async Task<ServiceResponse<bool>> SetQC(QcLabelDto label) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var pail = await session.Query<ProductionOrderPailStatus>().FirstAsync(p => p.POID == label.POID && p.PailNumber == label.PailNumber);

                pail.PailStatus = "PRLQ";
                pail.Op_No = label.OpQM;

                await session.UpdateAsync(pail);
                await transaction.CommitAsync();

                return ServiceResponse<bool>.Ok(true);
            } catch (Exception ex) {
                return ServiceResponse<bool>.CreateErrorMpg(ex.Message);
            }
        }

        public virtual async Task<ServiceResponse<bool>> CloseCommand(string POID) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var result = await session.Query<ProductionOrder>().FirstAsync(p => p.POID == POID);
                result.Status = Settings.Default.CMD_DONE;
                result.Priority = "-1";

                await session.UpdateAsync(result);
                await transaction.CommitAsync();

                return ServiceResponse<bool>.Ok(true);
            } catch (Exception ex) {
                return ServiceResponse<bool>.CreateErrorMpg(ex.Message);
            }
        }

        public virtual async Task<ServiceResponse<PartialMaterials>> GetPartialMaterials(string POID) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var po = await session.Query<ProductionOrder>().FirstAsync(p => p.POID == POID);
                var pails = await session.Query<ProductionOrderPailStatus>().Where(p => p.POID == POID && p.PailStatus == Settings.Default.CMD_DONE).ToListAsync();
                var position = await session.Query<ProductionOrderFinalItem>().FirstAsync(p => p.POID == POID);
                var ldm = await session.Query<ProductionOrderBom>().Where(p => p.POID == POID).ToListAsync();

                var result = new PartialMaterials() {
                    Materials = ldm,
                    Order = po,
                    Pails = pails,
                    Position = position.ItemPosition
                };

                return ServiceResponse<PartialMaterials>.Ok(result);
            } catch (Exception ex) {
                return ServiceResponse<PartialMaterials>.CreateErrorMpg(ex.Message);
            }
        }

        public virtual async Task<ServiceResponse<bool>> UpdateTickets(List<ProductionOrderPailStatus> pails) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                pails.ForEach(async pail => {
                    await session.UpdateAsync(pail);
                });

                await transaction.CommitAsync();

                return ServiceResponse<bool>.Ok(true);
            } catch (Exception ex) {
                return ServiceResponse<bool>.CreateErrorMpg(ex.Message);
            }
        }

        public virtual async Task<ServiceResponse<bool>> StartCommand(CommandData command) {
            try {
                var po = command.Data;
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                await session.SaveAsync(po.Order);
                po.DataUOMS.ForEach(async item => await session.SaveAsync(item));
                po.OrderBOM.ForEach(async item => await session.SaveAsync(item));
                po.OrderFinalItem.ForEach(async item => await session.SaveAsync(item));
                po.LotDetails.ForEach(async item => await session.SaveAsync(item));
                await session.SaveAsync(po.LotHeader);

                command.Pails.ForEach(async pail => {
                    await session.SaveAsync(pail);
                });

                await transaction.CommitAsync();

                return ServiceResponse<bool>.Ok(true);
            } catch (Exception ex) {
                return ServiceResponse<bool>.CreateErrorMpg(ex.Message);
            }
        }

        public virtual async Task<ServiceResponse<bool>> SaveOrUpdateMaterials(UpdateData data) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                data.Names.ForEach(async name => {
                    var localName = await session.Query<AlternativeName>().FirstOrDefaultAsync(p => p.MaterialID == name.MaterialID && p.Language == name.Language);

                    if (localName == null) {
                        _ = await session.SaveAsync(name);
                    } else {
                        localName.SetDetails(name);
                        await session.SaveAsync(localName);
                    }
                });

                data.Materials.ForEach(async material => {
                    var localMaterial = await session.Query<MaterialData>().FirstOrDefaultAsync(p => p.MaterialID == material.MaterialID);

                    if (localMaterial == null) {
                        _ = await session.SaveAsync(material);
                    } else {
                        localMaterial.SetDetails(material);
                        await session.SaveAsync(localMaterial);
                    }
                });


                data.Classifications.ForEach(async classification => {
                    var localClasification = await session.Query<Classification>().FirstOrDefaultAsync(p => p.MaterialID == classification.MaterialID &&
                            p.Param == classification.Param && p.Value == classification.Value);

                    if (localClasification == null) {
                        await session.SaveAsync(classification);
                    } else {
                        localClasification.SetDetails(classification);
                        await session.UpdateAsync(localClasification);
                    }
                });

                await transaction.CommitAsync();

                return ServiceResponse<bool>.Ok(true);
            } catch (Exception ex) {
                return ServiceResponse<bool>.CreateErrorMpg(ex.Message);
            }
        }

        public virtual async Task<ServiceResponse<bool>> SaveOrUpdateRiskPhrases(IList<RiskPhrase> phrases) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                foreach (var item in phrases) {
                    var phrase = await session.Query<RiskPhrase>().FirstOrDefaultAsync(p => p.Material == item.Material);

                    if (phrase == null) {
                        await session.SaveAsync(item);
                    } else {
                        phrase.Update(item);
                        await session.UpdateAsync(phrase);
                    }
                }

                await transaction.CommitAsync();
                return ServiceResponse<bool>.Ok(true);
            } catch (Exception ex) {
                return ServiceResponse<bool>.CreateErrorMpg(ex.Message);
            }
        }

        public virtual async Task<ServiceResponse<bool>> SaveOrUpdateStockVesel(IList<StockVessel> vessels) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                foreach (var item in vessels) {
                    var vessel = await session.Query<StockVessel>().FirstOrDefaultAsync(p => p.MaterialID == item.MaterialID);

                    if (vessel == null) {
                        await session.SaveAsync(item);
                    } else {
                        vessel.SetDetails(item);
                        await session.UpdateAsync(vessel);
                    }
                }

                await transaction.CommitAsync();
                return ServiceResponse<bool>.Ok(true);
            } catch (Exception ex) {
                return ServiceResponse<bool>.CreateErrorMpg(ex.Message);
            }
        }

        public virtual async Task<ServiceResponse<bool>> ChangeStatus(string POID, string pailIndex, string status) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var pail = await session.Query<ProductionOrderPailStatus>().FirstAsync(p => p.POID == POID && p.PailNumber == pailIndex);
                pail.PailStatus = status;

                await session.UpdateAsync(pail);
                await transaction.CommitAsync();
                return ServiceResponse<bool>.Ok(true);
            } catch (Exception ex) {
                return ServiceResponse<bool>.CreateErrorMpg(ex.Message);
            }
        }
    }
}
