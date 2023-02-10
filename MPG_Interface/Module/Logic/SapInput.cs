using MPG_Interface.Module.Interfaces;
using MPG_Interface.Module.Visual;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;
using System.Linq;
using System;

using DataEntity.Model.Types;
using DataEntity.Model.Input;
using DataEntity.Model.Output;
using System.Diagnostics;
using DataEntity.Config;

using SAPServices;

using log4net;

namespace MPG_Interface.Module.Logic {

    /// <summary>
    /// Implements the data manipulation for the SAP connection
    /// </summary>
    public class SapInput : IInput {

        /// <summary>
        /// Reference the SAP client
        /// </summary>
        private readonly Z_MPGClient _client;

        /// <summary>
        /// List with the observers
        /// </summary>
        private readonly List<IObserver> _observers;

        /// <summary>
        /// Logger that saves the errors
        /// </summary>
        private readonly ILog _logger;

        /// <summary>
        /// Constructor that sets the client and gets the available production orders
        /// </summary>
        public SapInput() {
            _client = SapDb.GetClient();
            _observers = new();
            _logger = LogManager.GetLogger("Errors");
        }

        /// <summary>
        /// Reads the data from the SAP and the output database and compose the available commands
        /// </summary>
        public async Task GetCommandsAsync(DateTime startDate, DateTime endDate) {
            InputDataCollection.Clear();
            var task = _client.Z_PRODORDERSAsync(new Z_PRODORDERS {
                PLANT = Properties.Settings.Default.Plant,
                START_DATE = startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                END_DATE = endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            });

            if (await Task.WhenAny(task, Task.Delay(10000)) == task) {
                List<string> orders = task.Result.Z_PRODORDERSResponse.MPGPO.Select(p => p.POID).ToList();
                orders.ForEach(item => {
                    InputDataCollection.CreateCollection(item, task.Result.Z_PRODORDERSResponse);
                });
            } else {
                _logger.Error("Timeout for getting the commands from SAP");
                Alerts.ShowMessage("Nu s-a putut realiza conexiuna la SAP sau timeout");
            }

            using (NHibernate.ISession session = SqliteDB.Instance.GetSession()) {
                using (NHibernate.ITransaction transaction = session.BeginTransaction()) {
                    session.Query<ProductionOrder>()
                        .Where(p => p.PlannedStartDate >= startDate && p.PlannedEndDate <= endDate).ToList()
                        .ForEach(item => {
                            InputDataCollection.AddElement(new InputData() {
                                Order = item
                            });
                        });
                }
            }
        }

        /// <summary>
        /// Called to download or update the materials
        /// </summary>
        public void GetMaterials() {
            new Thread(new ThreadStart(async () => {
                string date = Properties.Settings.Default.Update;

                if (string.IsNullOrEmpty(date)) {
                    if (!Alerts.ConfirmMessageThread("Doriti sa descarcati materialele?")) {
                        return;
                    }

                    Alerts.ShowLoading();
                    await GetInitialMaterialsAsync();
                    await GetRiskPhrasesAsync();
                    Alerts.HideLoading();
                } else {
                    DateTime local = DateTime.Parse(date, CultureInfo.InvariantCulture);
                    if (local.Date != DateTime.Now.Date) {
                        await UpdateMaterialsAsync();

                    }
                }

                Functions.SetUpdateDate();
            })).Start();
        }

        /// <summary>
        /// Used to update the materials
        /// </summary>
        public async Task UpdateMaterialsAsync() {
            if (!Alerts.ConfirmMessageThread("Doriti sa actualizati materialele?")) {
                return;
            }

            Alerts.ShowLoading();
            await CheckSapMaterialsAsync();
            await GetRiskPhrasesAsync();
            Functions.SetUpdateDate();
            Alerts.HideLoading();
        }

        /// <summary>
        /// Used to update the materials
        /// </summary>
        private async Task CheckSapMaterialsAsync() {
            string date = DateTime.Now.ToString("yyyy-mm-dd", CultureInfo.InvariantCulture);
            Z_MPGNEWMATERIALSResponse1 result = await _client.Z_MPGNEWMATERIALSAsync(new Z_MPGNEWMATERIALS() {
                PLANT = Properties.Settings.Default.Plant,
                START_DATE = date,
                END_DATE = date
            });

            using (NHibernate.ISession session = SqliteDB.Instance.GetSession()) {
                using (NHibernate.ITransaction transaction = session.BeginTransaction()) {

                    foreach (ZMATERIALDATA item in result.Z_MPGNEWMATERIALSResponse.MATERIALDATA) {
                        MaterialData exists = session.Query<MaterialData>().First(p => p.MaterialID == item.MATERIALID);

                        if (exists != null) {
                            exists.SetDetails(item);
                            session.Update(exists);
                        } else {
                            _ = session.Save(new MaterialData(item));
                        }
                    }

                    foreach (ZALTERNATIVEDESCRIPTION item in result.Z_MPGNEWMATERIALSResponse.ALTERNATIVEDESCR) {
                        AlternativeName exists = session.Query<AlternativeName>().First(p => p.MaterialID == item.MATERIALID);

                        if (exists != null) {
                            exists.SetDetails(item);
                            session.Update(exists);
                        } else {
                            _ = session.Save(new AlternativeName(item));
                        }
                    }

                    foreach (ZCLASIFICATION item in result.Z_MPGNEWMATERIALSResponse.CLASIFICATIONS) {
                        Clasification exists = session.Query<Clasification>().First(p => p.MaterialID == item.MATERIALID);

                        if (exists != null) {
                            exists.SetDetails(item);
                            session.Update(exists);
                        } else {
                            _ = session.Save(new Clasification(item));
                        }
                    }

                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Used to download the initial materials
        /// </summary>
        /// <returns>Task that is necessary to be awaited </returns>
        public async Task GetInitialMaterialsAsync() {
            var result = await _client.Z_INITIALMPGDOWNLOADAsync(new Z_INITIALMPGDOWNLOAD() {
                PLANT = Properties.Settings.Default.Plant
            });

            using (NHibernate.ISession session = SqliteDB.Instance.GetSession()) {
                using (NHibernate.ITransaction transaction = session.BeginTransaction()) {

                    foreach (ZALTERNATIVEDESCRIPTION item in result.Z_INITIALMPGDOWNLOADResponse.ALTERNATIVEDESCR) {
                        _ = session.Save(new AlternativeName(item));
                    }

                    foreach (ZMATERIALDATA item in result.Z_INITIALMPGDOWNLOADResponse.MATERIALDATA) {
                        _ = session.Save(new MaterialData(item));
                    }

                    foreach (ZCLASIFICATION item in result.Z_INITIALMPGDOWNLOADResponse.CLASIFICATIONS) {
                        _ = session.Save(new Clasification(item));
                    }

                    transaction.Commit();
                }
            }
        }



        /// <summary>
        /// Sends the command status to SAP
        /// </summary>
        /// <param name="poid">ID of the command</param>
        /// <param name="status">Status that will be set</param>
        /// <returns>Task that is necessary to be awaited</returns>
        public async Task<bool> SetCommandStatusAsync(string poid, string status) {
            bool resultStatus = true;
            var result = await _client.Z_UPDTSTATUSPOAsync(new Z_UPDTSTATUSPO {
                STATUSPO = new ZSTATUSPO[] {
                    new ZSTATUSPO {
                        POID = poid,
                        PLANT = Properties.Settings.Default.Plant,
                        STATUSCODE = status,
                        ORDERMESSAGE = string.Empty
                    }
                }
            });

            result.Z_UPDTSTATUSPOResponse.ERRORS.ToList().ForEach(item => {
                if (item.ERRORCODE == 0) {
                    resultStatus = false;
                    _logger.Error($"Error for the command {item.POID} with the code {item.ERRORCODE} and the message {item.ERRORMESSAGE}");
                }
            });

            return resultStatus;
        }

        /// <summary>
        /// Gets the vessel asignations
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public async Task GetStockVesselAsync() {
            var result = await _client.Z_MPGCORESPONDENTAAsync(new Z_MPGCORESPONDENTA() {
                PLANT = Properties.Settings.Default.Plant,
                MPG_CODE = Properties.Settings.Default.Code,
                DATA = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture)
            });

            using (NHibernate.ISession sqlite = SqliteDB.Instance.GetSession()) {
                using (NHibernate.ITransaction transaction = sqlite.BeginTransaction()) {
                    result.Z_MPGCORESPONDENTAResponse.CORRESPONDENCE.ToList().ForEach(item => {
                        StockVessel local = sqlite.Query<StockVessel>().First(p => p.MaterialID == item.MATERIALID);
                        if (local != null) {
                            local.SetDetails(item);
                            sqlite.Update(local);
                        } else {
                            _ = sqlite.Save(new StockVessel(item));
                        }
                    });

                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Downloads the riskphrases from the SAP
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public async Task GetRiskPhrasesAsync() {
            Z_MPGFRAZERISCResponse1 result = await _client.Z_MPGFRAZERISCAsync(new());

            Dictionary<string, RiskPhrase> dict = new();

            foreach (ZMESMPGFRAZERISC item in result.Z_MPGFRAZERISCResponse.FRAZEDERISC) {
                if (dict.ContainsKey(item.SERIES)) {
                    dict[item.SERIES].Risk_Fr += item.TEXT + ">?<";
                } else {
                    dict.Add(item.SERIES, new() { Risk_Fr = item.TEXT, Instr = "", Language = item.LANGUAGE, Material = item.SERIES });
                }
            }

            foreach (ZMESMPGPINSTRUCTIUNI item in result.Z_MPGFRAZERISCResponse.INSTRUCTIUNI) {
                dict[item.SERIES].Instr += item.TEXT + ">?<";
            }

            foreach (ZMESMPGPICTOGRAME item in result.Z_MPGFRAZERISCResponse.PICTOGRAME) {
                if (dict.ContainsKey(item.SERIES)) {
                    dict[item.SERIES].SetPics(item);
                } else {
                    RiskPhrase local = new() {
                        Material = item.SERIES,
                        Language = "",
                        Instr = "",
                        Risk_Fr = ""
                    };
                    local.SetPics(item);
                    dict.Add(item.SERIES, local);
                }
            }

            using (NHibernate.ISession session = SqliteDB.Instance.GetSession()) {
                using (NHibernate.ITransaction transaction = session.BeginTransaction()) {
                    foreach (string item in dict.Keys) {
                        RiskPhrase risk = session.Query<RiskPhrase>().FirstOrDefault(p => p.Material == item);
                        if (risk == null) {
                            _ = session.Save(dict[item]);
                        } else {
                            risk.Update(dict[item]);
                            session.Update(risk);
                        }
                    }
                    transaction.Commit();
                    Alerts.ShowMessage("Au fost actualizate materialele");
                }
            }

        }

        /// <summary>
        /// Checks if there is any command for the given ID
        /// </summary>
        /// <param name="poid">Command id</param>
        /// <returns>True if there is any command <br/> False otherwise</returns>
        public bool CommandExists(string poid) {
            using (NHibernate.ISession session = SqliteDB.Instance.GetSession()) {
                using (NHibernate.ITransaction transation = session.BeginTransaction()) {
                    return session.Query<ProductionOrder>().Where(p => p.POID == poid).Any();
                }
            }
        }

        /// <summary>
        /// Adds an observer in the list
        /// </summary>
        /// <param name="observer">Reference to the observer</param>
        public void AddObserver(IObserver observer) {
            if (!_observers.Contains(observer)) {
                _observers.Add(observer);
            }
        }

        /// <summary>
        /// Removes the observer from the list
        /// </summary>
        /// <param name="observer">Refrence to the observer</param>
        public void RemoveObserver(IObserver observer) {
            if (_observers.Contains(observer)) {
                _ = _observers.Remove(observer);
            }
        }

        /// <summary>
        /// Notify the observers with the modifcation made
        /// </summary>
        /// <param name="poid">Id of the command</param>
        /// <param name="status">New status for the command</param>
        public void Notify(string poid, string status) {
            _observers.ForEach(item => item.Update(poid, status));
        }

        /// <summary>
        /// Used to create production
        /// </summary>
        /// <param name="POID">ID number of the command</param>
        /// <returns></returns>
        public async Task GeneratePartialProduction(string POID) {
            using (NHibernate.ISession session = SqliteDB.Instance.GetSession()) {
                using (NHibernate.ITransaction transaction = session.BeginTransaction()) {
                    ProductionOrder PO = session.Query<ProductionOrder>().First(p => p.POID == POID);
                    List<ProductionOrderPailStatus> details = session.Query<ProductionOrderPailStatus>().Where(p => p.POID == PO.POID && p.PailStatus == "PRLT").ToList();

                    Z_MPGPREDARE predare = Functions.CreatePredare(PO, details, session);
                    Z_MPGPREDAREResponse1 result = await _client.Z_MPGPREDAREAsync(predare);

                    result.Z_MPGPREDAREResponse.ERRORS.ToList().ForEach(item => {
                        Debug.WriteLine($"{item.POID}_{item.DOCNO}_{item.ERRORCODE}_{item.ERRORMESSAGE}");
                        details.ForEach(local => {
                            local.Ticket = item.DOCNO;
                            session.Update(local);
                        });
                    });

                    Z_MPGCONSUM consum = Functions.CreateConsumption(PO, session);
                    Z_MPGCONSUMResponse1 resultConsum = await _client.Z_MPGCONSUMAsync(consum);

                    resultConsum.Z_MPGCONSUMResponse.ERRORS.ToList().ForEach(item => {
                        Debug.WriteLine($"{item.POID}_{item.DOCNO}_{item.ERRORCODE}_{item.ERRORMESSAGE}");
                        details.ForEach(local => {
                            local.Consumption = item.DOCNO;
                            session.Update(local);
                        });
                    });

                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="POID"></param>
        /// <returns></returns>
        public async Task CloseCommnadProduction(string POID) {
            await GeneratePartialProduction(POID);
            await SetCommandStatusAsync(POID, Properties.Resources.CMD_PRLT);

            using (var session = SqliteDB.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    var result = session.Query<ProductionOrder>().First(p => p.POID == POID);
                    result.Status = Properties.Resources.CMD_PRLT; // TODO: adaugat stare pentru comanda inchisa prematur
                    var details = session.Query<ProductionOrderPailStatus>().Where(p => p.POID == POID && p.PailStatus != "PRLT").ToList();
                    details.ForEach(item => {
                        item.PailStatus = Properties.Resources.CMD_PRLT; // TODO: adaugata stare pentru galeata neexecutata
                        session.Update(item);
                    });
                    session.Update(result);
                    transaction.Commit();
                }
            }
        }
    }
}
