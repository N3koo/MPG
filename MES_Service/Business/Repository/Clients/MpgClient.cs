﻿using DataEntity.Config;
using DataEntity.Model.Input;
using DataEntity.Model.Output;
using DataEntity.Model.Types;
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
using System.Data.Entity;
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
            "ORDER BY oroders.Priority";

        public readonly static MpgClient Client = new();

        public async Task<ServiceResponse> GetCommands(Period period) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var result = await session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= period.StartDate && p.PlannedEndDate <= period.EndDate).ToListAsync();

                return ServiceResponse.CreateResponse(result, "Nu exista inregistrari in perioada selectata");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse> SaveCorrection(POConsumption materials, ServiceResponse response) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var correction = (ProductionOrderCorection)response.Data;

                Utils.UpdateMaterials(session, materials);
                await session.SaveAsync(correction);
                await transaction.CommitAsync();

                return ServiceResponse.Ok("Corectiile au fost salvate");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse> GetAvailablePail(string POID) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var pail = await session.Query<ProductionOrderPailStatus>().FirstOrDefaultAsync(p => p.POID == POID && p.PailStatus == "ELB");
                var order = await session.Query<ProductionOrder>().FirstAsync(p => p.POID == POID);
                var techDetails = await session.Query<ProductionOrderTechDetail>().Where(p => p.POID == POID && p.OP_DESCR == "MIXARE_1").ToListAsync();

                var result = PailDto.FromPailStatus(pail, order, techDetails);
                return ServiceResponse.CreateResponse(result, "Nu exista galeti disponibile");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse> GetFirstPail() {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var result = await session.CreateSQLQuery(GET_FIRST_PAIL)
                    .SetResultTransformer(Transformers.AliasToBean<PailQCDto>())
                    .ListAsync<PailQCDto>();

                return ServiceResponse.CreateResponse(result.First(), "Nu exista comenzi active");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse> SaveDosageMaterials(POConsumption consumption) {
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

                return ServiceResponse.Ok(result);
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse> GetCommand(string POID) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var result = await session.Query<ProductionOrder>().FirstOrDefaultAsync(p => p.POID == POID);
                return ServiceResponse.CreateResponse(result, "Comanda nu exista");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse> CheckPriority(string priority) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var result = await session.Query<ProductionOrder>().AnyAsync(p => p.Priority == priority);
                return ServiceResponse.CreateResponse(!result, "Nu exista o comanda ce are prioritatea data");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse> GetQc(string POID) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var result = await session.Query<ProductionOrderLotHeader>().FirstOrDefaultAsync(p => p.POID == POID);
                return ServiceResponse.CreateResponse(result?.PozQC, "Nu exista comanda");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse> GetMaterials(string POID) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var quantity = (await session.Query<ProductionOrder>().FirstAsync(p => p.POID == POID)).PlannedQtyBUC;
                var materials = await session.CreateSQLQuery(DOSAGE_MATERIALS)
                    .SetResultTransformer(Transformers.AliasToBean<MaterialDto>())
                    .SetString(0, POID).ListAsync<MaterialDto>();

                materials.Where(p => p.ItemQty != 1).ToList().ForEach(item => item.ItemQty /= quantity);
                return ServiceResponse.CreateResponse(materials, "Nu exista materiale pentru comanda selectata");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse> GetLabelData(string POID) {
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

                return ServiceResponse.CreateResponse(result, "Nu exista eticheta pentru comanda selectata");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse> BlockCommand(string POID) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var po = await session.Query<ProductionOrder>().FirstOrDefaultAsync(p => p.POID == POID);
                if (po == null) {
                    return ServiceResponse.Ok(2);
                }

                var count = await session.Query<ProductionOrderPailStatus>().CountAsync(p => p.POID == POID && p.PailStatus == Settings.Default.CMD_SEND);

                if (po.PlannedQtyBUC != count) {
                    return ServiceResponse.Ok(1);
                }

                po.Status = Settings.Default.CMD_BLOCKED;
                po.Priority = "-1";
                po.MPGStatus = 1;
                po.MPGRowUpdated = DateTime.Now;

                await session.UpdateAsync(po);
                await transaction.CommitAsync();

                return ServiceResponse.Ok(0);
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }

        }

        public async Task<ServiceResponse> SetQC(ServiceResponse response) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var label = (QcLabelDto)response.Data;
                var pail = await session.Query<ProductionOrderPailStatus>().FirstAsync(p => p.POID == label.POID && p.PailNumber == label.PailNumber);

                pail.PailStatus = "PRLQ";
                pail.Op_No = label.OpQM;

                await session.UpdateAsync(pail);
                await transaction.CommitAsync();

                return ServiceResponse.Ok("Statusul a fost actualizat");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse> CreateCommand(ProductionOrder po) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                po.Priority = "-1";

                await session.SaveAsync(po);
                await transaction.CommitAsync();

                return ServiceResponse.Ok("Comanda a fost salvata");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse> CloseCommand(string POID) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var result = await session.Query<ProductionOrder>().FirstAsync(p => p.POID == POID);
                result.Status = Settings.Default.CMD_DONE;
                result.Priority = "-1";

                await session.UpdateAsync(result);
                await transaction.CommitAsync();

                return ServiceResponse.Ok("Comanda a fost inchisa");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse> PartialMaterials(string POID) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var po = await session.Query<ProductionOrder>().FirstAsync(p => p.POID == POID);
                var pails = await session.Query<ProductionOrderPailStatus>().Where(p => p.POID == POID && p.PailStatus == Settings.Default.CMD_DONE).ToListAsync();
                var position = await session.Query<ProductionOrderFinalItem>().FirstAsync(p => p.POID == POID);
                var ldm = await session.Query<ProductionOrderBom>().Where(p => p.POID == POID).ToListAsync();

                var result = Tuple.Create(po, pails, ldm, position.ItemPosition);
                return ServiceResponse.Ok(result);
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse> UpdateTickets(ServiceResponse response) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var tuple = (Tuple<ProductionOrder, List<ProductionOrderPailStatus>, List<ProductionOrderBom>, string>)response.Data;
                tuple.Item2.ForEach(async pail => {
                    await session.UpdateAsync(pail);
                });

                await transaction.CommitAsync();

                return ServiceResponse.Ok("Materialele au fost actulizate");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse> StartCommand(ServiceResponse response) {
            try {
                var data = (Tuple<InputData, List<ProductionOrderPailStatus>>)response.Data;
                var po = data.Item1;

                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                await session.SaveAsync(po.Order);
                po.DataUOMS.ForEach(async item => await session.SaveAsync(item));
                po.OrderBOM.ForEach(async item => await session.SaveAsync(item));
                po.OrderFinalItem.ForEach(async item => await session.SaveAsync(item));
                po.LotDetails.ForEach(async item => await session.SaveAsync(item));
                await session.SaveAsync(po.LotHeader);

                data.Item2.ForEach(async pail => {
                    await session.SaveAsync(pail);
                });

                await transaction.CommitAsync();

                return ServiceResponse.Ok("Comanda a fost inceputa");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse> SaveOrUpdateMaterials(ServiceResponse response) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var tuple = (Tuple<List<AlternativeName>, List<MaterialData>, List<Classification>>)response.Data;

                tuple.Item1.ForEach(async name => {
                    var localName = await session.Query<AlternativeName>().FirstOrDefaultAsync(p => p.MaterialID == name.MaterialID && p.Language == name.Language);

                    if (localName == null) {
                        _ = await session.SaveAsync(name);
                    } else {
                        localName.SetDetails(name);
                        await session.SaveAsync(localName);
                    }
                });

                tuple.Item2.ForEach(async material => {
                    var localMaterial = await session.Query<MaterialData>().FirstOrDefaultAsync(p => p.MaterialID == material.MaterialID);

                    if (localMaterial == null) {
                        _ = await session.SaveAsync(material);
                    } else {
                        localMaterial.SetDetails(material);
                        await session.SaveAsync(localMaterial);
                    }
                });


                tuple.Item3.ForEach(async classification => {
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

                return ServiceResponse.Ok("Materialele au fost actualizate");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse> SaveOrUpdateRiskPhrases(ServiceResponse response) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var phrases = (List<RiskPhrase>)response?.Data;
                phrases.ForEach(async item => {
                    var phrase = await session.Query<RiskPhrase>().FirstOrDefaultAsync(p => p.Material == item.Material);

                    if (phrase == null) {
                        await session.SaveAsync(item);
                    } else {
                        phrase.Update(item);
                        await session.UpdateAsync(phrase);
                    }
                });

                await transaction.CommitAsync();
                return ServiceResponse.Ok("Frazele de risc au fost actualizate");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse> SaveOrUpdateStockVesel(object data) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var vessels = (List<StockVessel>)data;
                vessels.ForEach(async vessel => {
                    var local = await session.Query<StockVessel>().FirstOrDefaultAsync(p => p.MaterialID == vessel.MaterialID);

                    if (local == null) {
                        await session.SaveAsync(vessel);
                    } else {
                        local.SetDetails(vessel);
                        await session.UpdateAsync(local);
                    }
                });

                await transaction.CommitAsync();
                return ServiceResponse.Ok("Vasele de stocare au fost actualizate");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }

        public async Task<ServiceResponse> ChangeStatus(string POID, string pailIndex, string status) {
            try {
                using var session = MpgDb.Instance.GetSession();
                using var transaction = session.BeginTransaction();

                var pail = await session.Query<ProductionOrderPailStatus>().FirstAsync(p => p.POID == POID && p.PailNumber == pailIndex);
                pail.PailStatus = status;

                await session.UpdateAsync(pail);
                await transaction.CommitAsync();
                return ServiceResponse.Ok("Statul galetii a fost actualizat");
            } catch (Exception ex) {
                return ServiceResponse.CreateErrorMpg(ex.Message);
            }
        }
    }
}
