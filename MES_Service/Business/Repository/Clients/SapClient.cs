using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response;
using MpgWebService.Data.Extension;
using MpgWebService.Properties;

using DataEntity.Model.Output;
using DataEntity.Model.Input;
using DataEntity.Model.Types;
using DataEntity.Config;

using SAPServices;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using System.Linq;
using System.Text;
using System;

namespace MpgWebService.Repository.Clients {

    public class SapClient {

        private readonly Z_MPGClient sapClient;

        public static readonly SapClient Client = new();

        public SapClient() {
            sapClient = SapDb.GetClient();
        }

        public async Task<ProductionOrder> BlockCommand(string POID) {
            var po = InputDataCollection.GetCommand(POID);
            po.Status = Settings.Default.CMD_BLOCKED;
            var status = await SetCommandStatusAsync(POID, Settings.Default.CMD_BLOCKED); // TODO: Check how to treat this result

            return status ? po : null;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public async Task<ServiceResponse> SendPartialProductionAsync(Tuple<ProductionOrder, List<ProductionOrderPailStatus>, List<ProductionOrderBom>, string> tuple) {
            var response = ServiceResponse.CreateOkResponse("Materialele au fost transmise");
            StringBuilder builder = new();

            Z_MPGPREDARE rendition = tuple.Item1.CreatePredare(tuple.Item2.Count, tuple.Item4);
            var result = await sapClient.Z_MPGPREDAREAsync(rendition);
            result.Z_MPGPREDAREResponse.ERRORS.ToList().ForEach(item => {
                tuple.Item2.ForEach(pail => {
                    pail.Ticket = item.DOCNO;
                });
            });

            if (string.IsNullOrEmpty(tuple.Item2[0].Ticket)) {
                response = ServiceResponse.CreateErrorSap(result.Z_MPGPREDAREResponse.ERRORS[0].ERRORMESSAGE);
            }

            Z_MPGCONSUM consumption = tuple.Item1.CreateConsumption(tuple.Item3);
            var resultConsum = await sapClient.Z_MPGCONSUMAsync(consumption);

            resultConsum.Z_MPGCONSUMResponse.ERRORS.ToList().ForEach(item => {
                tuple.Item2.ForEach(pail => {
                    pail.Consumption = item.DOCNO;

                    if (string.IsNullOrEmpty(item.DOCNO)) {
                        builder.Append(item.ERRORMESSAGE);
                        builder.Append('\n');
                    }
                });
            });

            if (string.IsNullOrEmpty(tuple.Item2[0].Consumption)) {
                if (response.Status) {
                    response = ServiceResponse.CreateErrorSap(builder.ToString());
                } else {
                    response.AddError(builder.ToString());
                }
            }

            return response;
        }

        public async Task<List<ProductionOrder>> GetCommandsAsync(Period period) {
            InputDataCollection.Clear();
            var task = sapClient.Z_PRODORDERSAsync(new Z_PRODORDERS {
                PLANT = Settings.Default.Plant,
                START_DATE = period.StartDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                END_DATE = period.EndDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            });

            if (await Task.WhenAny(task, Task.Delay(10000)) == task) {
                task.Result.Z_PRODORDERSResponse.MPGPO.ToList().ForEach(item => {
                    InputDataCollection.CreateCollection(item.POID, task.Result.Z_PRODORDERSResponse);
                });
            }

            return InputDataCollection.GetCommands();
        }

        public List<ProductionOrderPailStatus> StartCommand(StartCommand details) {
            return InputDataCollection.ExportCommand(details.POID, details.Priority.Value, details.QC);
        }

        public async Task<Tuple<List<AlternativeName>, List<MaterialData>, List<Classification>>> GetInitialMaterialsAsync() {
            var result = await sapClient.Z_INITIALMPGDOWNLOADAsync(new Z_INITIALMPGDOWNLOAD {
                PLANT = Properties.Settings.Default.Plant
            });

            List<AlternativeName> names = result.Z_INITIALMPGDOWNLOADResponse.ALTERNATIVEDESCR.Select(p => new AlternativeName(p)).ToList();
            List<MaterialData> materials = result.Z_INITIALMPGDOWNLOADResponse.MATERIALDATA.Select(p => new MaterialData(p)).ToList();
            List<Classification> clasifications = result.Z_INITIALMPGDOWNLOADResponse.CLASIFICATIONS.Select(p => new Classification(p)).ToList();

            return Tuple.Create(names, materials, clasifications);
        }

        public async Task<Tuple<List<AlternativeName>, List<MaterialData>, List<Classification>>> CheckSapMaterialsAsync() {
            string date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var result = await sapClient.Z_MPGNEWMATERIALSAsync(new Z_MPGNEWMATERIALS {
                PLANT = Properties.Settings.Default.Plant,
                START_DATE = date,
                END_DATE = date
            });

            List<AlternativeName> names = result.Z_MPGNEWMATERIALSResponse.ALTERNATIVEDESCR.Select(p => new AlternativeName(p)).ToList();
            List<MaterialData> materials = result.Z_MPGNEWMATERIALSResponse.MATERIALDATA.Select(p => new MaterialData(p)).ToList();
            List<Classification> classifications = result.Z_MPGNEWMATERIALSResponse.CLASIFICATIONS.Select(p => new Classification(p)).ToList();

            return Tuple.Create(names, materials, classifications);
        }

        public async Task<List<RiskPhrase>> GetRiskPhrasesAsync() {
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

            return phrases;
        }

        public string GetQC(string POID) {
            return InputDataCollection.GetQC(POID);
        }
    }
}
