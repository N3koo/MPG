﻿using DataEntity.Config;
using DataEntity.Model.Input;
using DataEntity.Model.Output;
using DataEntity.Model.Types;
using MpgWebService.Business.Data.JoinData;
using MpgWebService.Data.Extension;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Properties;
using SAPServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpgWebService.Repository.Clients {

    public class SapClient {

        private readonly Z_MPGClient sapClient;

        public static readonly SapClient Client = new();

        public SapClient() {
            sapClient = SapDb.GetClient();
        }

        public async Task<ServiceResponse<bool>> BlockCommand(string POID) {
            var po = InputDataCollection.GetCommand(POID);
            po.Status = Settings.Default.CMD_BLOCKED;
            var status = await SetCommandStatusAsync(POID, Settings.Default.CMD_BLOCKED); // TODO: Check how to treat this result

            return ServiceResponse<bool>.CreateResponse(status, $"Nu a fost gasita comanda {POID}");
        }

        public async Task<bool> SetCommandStatusAsync(string POID, string status) {
            bool resultStatus = true;

            var result = await sapClient.Z_UPDTSTATUSPOAsync(new Z_UPDTSTATUSPO {
                STATUSPO = new ZSTATUSPO[] {
                    new() {
                        POID = POID,
                        PLANT = "1000",
                        STATUSCODE = status,
                        ORDERMESSAGE = string.Empty
                    }
                }
            });

            result.Z_UPDTSTATUSPOResponse.ERRORS.ToList().ForEach(item => {
                if (item.ERRORCODE == 0) {
                    resultStatus = false;
                }
            });

            return resultStatus;
        }

        public async Task<ServiceResponse<bool>> SendPartialProductionAsync(PartialMaterials data) {
            var response = ServiceResponse<bool>.Ok(true);
            StringBuilder builder = new();

            Z_MPGPREDARE rendition = data.Order.CreatePredare(data.Pails.Count, data.Position);
            var result = await sapClient.Z_MPGPREDAREAsync(rendition);
            result.Z_MPGPREDAREResponse.ERRORS.ToList().ForEach(item => {
                data.Pails.ForEach(pail => {
                    pail.Ticket = item.DOCNO;
                });
            });

            if (string.IsNullOrEmpty(data.Pails[0].Ticket)) {
                response = ServiceResponse<bool>.CreateErrorSap(result.Z_MPGPREDAREResponse.ERRORS[0].ERRORMESSAGE);
            }

            Z_MPGCONSUM consumption = data.Order.CreateConsumption(data.Materials);
            var resultConsum = await sapClient.Z_MPGCONSUMAsync(consumption);

            resultConsum.Z_MPGCONSUMResponse.ERRORS.ToList().ForEach(item => {
                data.Pails.ForEach(pail => {
                    pail.Consumption = item.DOCNO;

                    if (string.IsNullOrEmpty(item.DOCNO)) {
                        builder.Append(item.ERRORMESSAGE);
                        builder.Append('\n');
                    }
                });
            });

            if (string.IsNullOrEmpty(data.Pails[0].Consumption)) {
                if (response.Errors.Count != 0) {
                    response = ServiceResponse<bool>.CreateErrorSap(builder.ToString());
                } else {
                    response.AddError(ErrorType.MPG(builder.ToString()));
                }
            }

            return response;
        }

        public async Task<ServiceResponse<List<ProductionOrder>>> GetCommandsAsync(Period period) {
            InputDataCollection.Clear();
            var data = await sapClient.Z_PRODORDERSAsync(new Z_PRODORDERS {
                PLANT = Settings.Default.Plant,
                START_DATE = period.StartDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                END_DATE = period.EndDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            });

            foreach (var item in data.Z_PRODORDERSResponse.MPGPO) {
                InputDataCollection.CreateCollection(item.POID, data.Z_PRODORDERSResponse);
            }

            return ServiceResponse<List<ProductionOrder>>.Ok(InputDataCollection.GetCommands());
        }

        public List<ProductionOrderPailStatus> StartCommand(StartCommand details) {
            return InputDataCollection.ExportCommand(details.POID, details.Priority.Value, details.QC);
        }

        public async Task<ServiceResponse<UpdateData>> GetInitialMaterialsAsync() {
            var result = await sapClient.Z_INITIALMPGDOWNLOADAsync(new Z_INITIALMPGDOWNLOAD {
                PLANT = Properties.Settings.Default.Plant
            });

            List<AlternativeName> names = result.Z_INITIALMPGDOWNLOADResponse.ALTERNATIVEDESCR.Select(p => new AlternativeName(p)).ToList();
            List<MaterialData> materials = result.Z_INITIALMPGDOWNLOADResponse.MATERIALDATA.Select(p => new MaterialData(p)).ToList();
            List<Classification> clasifications = result.Z_INITIALMPGDOWNLOADResponse.CLASIFICATIONS.Select(p => new Classification(p)).ToList();

            var data = new UpdateData(names, materials, clasifications);
            return ServiceResponse<UpdateData>.Ok(data);
        }

        public async Task<ServiceResponse<UpdateData>> CheckSapMaterialsAsync() {
            var date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var result = await sapClient.Z_MPGNEWMATERIALSAsync(new Z_MPGNEWMATERIALS {
                PLANT = Properties.Settings.Default.Plant,
                START_DATE = date,
                END_DATE = date
            });

            List<AlternativeName> names = result.Z_MPGNEWMATERIALSResponse.ALTERNATIVEDESCR.Select(p => new AlternativeName(p)).ToList();
            List<MaterialData> materials = result.Z_MPGNEWMATERIALSResponse.MATERIALDATA.Select(p => new MaterialData(p)).ToList();
            List<Classification> classifications = result.Z_MPGNEWMATERIALSResponse.CLASIFICATIONS.Select(p => new Classification(p)).ToList();

            var data = new UpdateData(names, materials, classifications);
            return ServiceResponse<UpdateData>.Ok(data);
        }

        public async Task<ServiceResponse<List<RiskPhrase>>> GetRiskPhrasesAsync() {
            var phrases = new List<RiskPhrase>();

            var result = await sapClient.Z_MPGFRAZERISCAsync(new());
            Dictionary<string, Tuple<StringBuilder, StringBuilder, RiskPhrase>> dict = new();

            result.Z_MPGFRAZERISCResponse.FRAZEDERISC.ToList().ForEach(item => {
                if (dict.ContainsKey(item.SERIES)) {
                    dict[item.SERIES].Item1.Append(item.TEXT).Append(">?<");
                } else {
                    dict.Add(item.SERIES, Tuple.Create(
                        new StringBuilder(), new StringBuilder(), new RiskPhrase {
                            Risk_Fr = item.TEXT,
                            Instr = "",
                            Language = item.LANGUAGE,
                            Material = item.SERIES
                        }));
                }
            });

            result.Z_MPGFRAZERISCResponse.INSTRUCTIUNI.ToList().ForEach(item => {
                dict[item.SERIES].Item2.Append(item.TEXT).Append(">?<");
            });

            result.Z_MPGFRAZERISCResponse.PICTOGRAME.ToList().ForEach(item => {
                if (dict.ContainsKey(item.SERIES)) {
                    dict[item.SERIES].Item3.SetPics(item);
                } else {
                    RiskPhrase phrase = new() {
                        Material = item.SERIES,
                        Language = "",
                        Instr = "",
                        Risk_Fr = ""
                    };
                    phrase.SetPics(item);
                    dict.Add(item.SERIES, Tuple.Create(
                        new StringBuilder(), new StringBuilder(), phrase));
                }
            });

            foreach (var item in dict.Values) {
                item.Item3.Risk_Fr = item.Item1.ToString();
                item.Item3.Instr = item.Item2.ToString();
                phrases.Add(item.Item3);
            }

            return ServiceResponse<List<RiskPhrase>>.Ok(phrases);
        }

        public string GetQC(string POID) {
            return InputDataCollection.GetQC(POID);
        }
    }
}
