using MPG_Interface.Module.Logic;

using DataEntity.Model.Output;
using DataEntity.Model.Input;
using DataEntity.Config;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

using log4net;
using System.Diagnostics;

namespace MPG_Interface.Module.Interfaces {

    /// <summary>
    /// Interface that defines the needed functions
    /// </summary>
    public interface IInput {

        /// <summary>
        /// Used to get the commands in a specific period
        /// </summary>
        /// <param name="startDate">Start date of the period</param>
        /// <param name="endDate">End date of the period</param>
        /// <returns>Task that is necessary to be awaited</returns>
        public Task GetCommandsAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Used to download or updates the materials
        /// </summary>
        public void GetMaterials();

        /// <summary>
        /// Used to update the materials
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public Task UpdateMaterialsAsync();

        /// <summary>
        /// Used to set the status the given command
        /// </summary>
        /// <param name="poid">ID of the command</param>
        /// <param name="status">Status which will be set</param>
        /// <returns>Task that is necessary to be awaited</returns>
        public Task<bool> SetCommandStatusAsync(string poid, string status);

        /// <summary>
        /// Used to download the assignation of colorants to the canisters
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public Task GetStockVesselAsync();

        /// <summary>
        /// Used to download the risk phrases
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public Task GetRiskPhrasesAsync();

        /// <summary>
        /// Used to create production consumption of the materials
        /// </summary>
        /// <param name="POID">ID of the current command</param>
        /// <returns>Task that is necessary to be awaited</returns>
        public Task GeneratePartialProduction(string POID);

        /// <summary>
        /// Closes the command for the given POID
        /// </summary>
        /// <param name="POID">Id of the command</param>
        /// <returns>Task that is necessary to be awaited</returns>
        public Task CloseCommnadProduction(string POID);

        /// <summary>
        /// Used to download all the materials for the first time
        /// </summary>
        /// <returns>Task that is necessary to be awaited</returns>
        public Task GetInitialMaterialsAsync();

        /// <summary>
        /// Checks for the given ID if there is any command in production
        /// </summary>
        /// <param name="poid">ID of the command</param>
        /// <returns>True if there is any command in production <br/>False otherwise</returns>
        public bool CommandExists(string poid);

        /// <summary>
        /// Adds a new observer
        /// </summary>
        /// <param name="observer">Observer</param>
        public void AddObserver(IObserver observer);

        /// <summary>
        /// Removes an observer
        /// </summary>
        /// <param name="observer">Observer</param>
        public void RemoveObserver(IObserver observer);

        /// <summary>
        /// Used to notify all the observers
        /// </summary>
        /// <param name="poid">ID of the command</param>
        /// <param name="status">Status of the command</param>
        public void Notify(string poid, string status);

        /// <summary>
        /// Used to check the status of the production and update the statuses
        /// </summary>
        public void VerifyProductionStatus() {
            try {
                List<ProductionOrderPailStatus> pails = new();

                using (NHibernate.ISession session = SqliteDB.Instance.GetSession()) {
                    using (NHibernate.ITransaction transaction = session.BeginTransaction()) {
                        List<ProductionOrder> orders = session.Query<ProductionOrder>().Where(p => p.Status != "PRLT" && p.Status != "BLOC").ToList();

                        var errors = session.Query<ProductionOrderPailStatus>().Where(p => p.PailStatus == "PRLI").GroupBy(p => p.POID).ToList();
                        if (errors.Count != 0) {
                            errors.ForEach(item => {
                                ProductionOrder local = orders.First(p => p.POID == item.Key);
                                if (local != null && local.Status != "PRLI") {
                                    local.Status = "PRLI";
                                    session.Update(local);
                                    Notify(item.Key, local.Status);
                                }
                            });
                        } else {
                            orders.ForEach(item => {
                                if (!session.Query<ProductionOrderPailStatus>().Any(p => p.POID == item.POID)) {
                                    return;
                                }

                                if (item.Status != "PRLS" && item.Status != "PRLT") {
                                    item.Status = "PRLS";
                                    session.Update(item);
                                    Notify(item.POID, item.Status);
                                }
                            });
                        }

                        orders.ForEach(item => {
                            int count = session.Query<ProductionOrderPailStatus>().Count(p => p.POID == item.POID && p.PailStatus == "PRLT");

                            if (count == item.PlannedQtyBUC) {
                                if (item.Status == "PRLT") {
                                    return;
                                }

                                item.Status = "PRLT";
                                item.Priority = "-1";
                                session.Update(item);
                                Functions.DecreasePriority(orders, item.POID, session);
                                Notify(item.POID, item.Status);
                            }
                        });

                        orders.ForEach(item => {
                            pails.AddRange(session.Query<ProductionOrderPailStatus>().Where(p => p.POID == item.POID).ToList());
                        });
                        transaction.Commit();
                    }
                }

                using (var session = MesDb.Instance.GetSession()) {
                    using (var transaction = session.BeginTransaction()) {
                        pails.ForEach(item => {
                            var pail = session.Query<ProductionOrderPailStatus>().First(p => p.POID == item.POID && p.PailNumber == item.PailNumber);
                            Debug.WriteLine(pail.POID + " " + pail.PailNumber);
                            pail.MPGRowUpdated = DateTime.Now;
                            pail.PailStatus = item.PailStatus;
                            session.Update(pail);
                        });
                        transaction.Commit();
                    }
                }
            } catch (Exception ex) {
                LogManager.GetLogger("Errors").Error(ex.Message);
            }
        }
    }
}
