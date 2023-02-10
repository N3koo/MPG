using MPG_Interface.Module.Interfaces;
using MPG_Interface.Module.Visual;

using DataEntity.Model.Output;
using DataEntity.Model.Input;
using DataEntity.Model.Types;
using DataEntity.Config;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;
using System.Linq;
using System;

namespace MPG_Interface.Module.Logic {

    /// <summary>
    /// Used to define functions for MES server
    /// </summary>
    public class MesInput : IInput {

        /// <summary>
        /// List with the observers
        /// </summary>
        private readonly List<IObserver> _observers;

        /// <summary>
        /// Public constructor
        /// </summary>
        public MesInput() {
            _observers = new();
        }

        /// <summary>
        /// Checks if the commands exits
        /// </summary>
        /// <param name="poid">ID of the command</param>
        /// <returns>True if there is any command <br> False otherwise</returns>
        public bool CommandExists(string poid) {
            using (var session = SqliteDB.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    return session.Query<ProductionOrder>().Any(p => p.POID == poid);
                }
            }
        }

        /// <summary>
        /// Gets the commands between the given period
        /// </summary>
        /// <param name="startDate">Start date of the period</param>
        /// <param name="endDate">Start date of the period</param>
        /// <returns>Task that is necessary to be awaited</returns>
        public Task GetCommandsAsync(DateTime startDate, DateTime endDate) {
            InputDataCollection.Clear();

            using (var session = MesDb.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {

                    session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= startDate && p.PlannedEndDate <= endDate && p.Status == Properties.Resources.CMD_ELB)
                        .ToList().ForEach(item => {
                            InputDataCollection.AddElement(Functions.CreateData(item, session));
                        });
                }
            }

            using (var session = SqliteDB.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= startDate && p.PlannedEndDate <= endDate).ToList().ForEach(item => {
                        InputDataCollection.AddElement(new InputData() {
                            Order = item
                        });
                    });
                }
            }

            return Task.Delay(200);
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
        /// Used to download the initial materials
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public Task GetInitialMaterialsAsync() {
            List<AlternativeName> names;
            List<MaterialData> materials;
            List<Clasification> clasifications;

            using (var session = MesDb.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    names = session.Query<AlternativeName>().ToList();
                    materials = session.Query<MaterialData>().ToList();
                    clasifications = session.Query<Clasification>().ToList();
                }
            }

            using (var session = SqliteDB.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    Functions.Save(names, session);
                    Functions.Save(materials, session);
                    Functions.Save(clasifications, session);
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Downloads the riskphrases from MES
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public Task GetRiskPhrasesAsync() {
            List<RiskPhrase> phrases = new();

            using (var session = MesDb.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    phrases = session.Query<RiskPhrase>().ToList();
                }
            }

            using (var session = SqliteDB.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    Functions.SaveOrUpdate(phrases, session);
                    transaction.Commit();
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the vessel asignations
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public Task GetStockVesselAsync() {
            List<StockVessel> stocks;
            using (var session = MesDb.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    stocks = session.Query<StockVessel>().ToList();
                }
            }

            using (var session = SqliteDB.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    stocks.ForEach(item => {
                        session.SaveOrUpdate(item);
                    });
                    transaction.Commit();
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Used to update the materials
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public async Task UpdateMaterialsAsync() {
            if (!Alerts.ConfirmMessageThread("Doriti sa actualizati materialele?")) {
                return;
            }

            Alerts.ShowLoading();
            await CheckMesMaterialsAsync();
            await GetRiskPhrasesAsync();
            Functions.SetUpdateDate();
            Alerts.HideLoading();
        }

        /// <summary>
        /// Sends the command status to MES
        /// </summary>
        /// <param name="poid">ID of the command</param>
        /// <param name="status">Status that will be set</param>
        /// <returns>Task that is necessary to be awaited</returns>
        public Task<bool> SetCommandStatusAsync(string poid, string status) {
            using (var session = MesDb.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    var order = session.Query<ProductionOrder>().First(p => p.POID == poid);
                    order.MPGRowUpdated = DateTime.Now;
                    order.Status = status;
                    session.Update(order);
                    transaction.Commit();
                }
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// Adds an observer in the list
        /// </summary>
        /// <param name="observer">Reference to the observer</param>
        public void AddObserver(IObserver observer) {
            _observers.Add(observer);
        }

        /// <summary>
        /// Removes the observer from the list
        /// </summary>
        /// <param name="observer">Refrence to the observer</param>
        public void RemoveObserver(IObserver observer) {
            _ = _observers.Remove(observer);
        }

        /// <summary>
        /// Notify the observers with the modifcation made
        /// </summary>
        /// <param name="poid">Id of the command</param>
        /// <param name="status">New status for the command</param>
        public void Notify(string poid, string status) {
            _observers.ForEach(item => {
                item.Update(poid, status);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="POID"></param>
        /// <returns></returns>
        public Task GeneratePartialProduction(string POID) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="POID"></param>
        /// <returns></returns>
        public Task CloseCommnadProduction(string POID) {
            List<ProductionOrderConsumption> materials;
            using (var session = SqliteDB.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    materials = session.Query<ProductionOrderConsumption>().Where(p => p.POID == POID).ToList();
                }
            }

            using (var session = MesDb.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    materials.ForEach(item => { session.Save(item); });
                    transaction.Commit();
                }
            }

            return Task.Delay(200);
        }

        /// <summary>
        /// Used to update the materials
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        private Task CheckMesMaterialsAsync() {
            List<MaterialData> materials;
            List<AlternativeName> names = new();
            List<Clasification> clasifications = new();

            using (var session = MesDb.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    materials = session.Query<MaterialData>().Where(p => p.MPGRowUpdated >= DateTime.Now).ToList();
                    names = session.Query<AlternativeName>().Where(p => p.MPGRowUpdated >= DateTime.Now).ToList();
                    clasifications = session.Query<Clasification>().Where(p => p.MPGRowUpdated >= DateTime.Now).ToList();/**/
                }
            }

            using (var session = SqliteDB.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    Functions.SaveOrUpdate(materials, session);
                    Functions.SaveOrUpdate(names, session);
                    Functions.SaveOrUpdate(clasifications, session);

                    transaction.Commit();
                }
            }

            return Task.CompletedTask;
        }
    }
}
