using DataEntity.Model.Input;
using DataEntity.Model.Output;
using DataEntity.Config;

using NHibernate;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System;

namespace DataEntity.Model.Types {

    public class InputData {

        /// <summary>
        /// Object that saves the orders details
        /// </summary>
        public ProductionOrder Order { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public List<MaterialDataUOMS> DataUOMS { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public List<ProductionOrderBom> OrderBOM { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public List<ProductionOrderFinalItem> OrderFinalItem { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public List<ProductionOrderLotDetail> LotDetails { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public ProductionOrderLotHeader LotHeader { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public bool[] QualityCheck { set; get; }

        /// <summary>
        /// Constructor that creates the objects
        /// </summary>
        public InputData() {
            Order = new ProductionOrder();
            DataUOMS = new List<MaterialDataUOMS>();
            OrderBOM = new List<ProductionOrderBom>();
            OrderFinalItem = new List<ProductionOrderFinalItem>();
            LotDetails = new List<ProductionOrderLotDetail>();
            LotHeader = new ProductionOrderLotHeader();
        }

        /// <summary>
        /// Creates the commnand 
        /// TODO: Delete qc from here
        /// </summary>
        public short ExportData() {
            List<ProductionOrderPailStatus> list = new();

            if (PriorityEmpty()) {
                return -1;
            }

            using (ISession sqlite = SqliteDB.Instance.GetSession()) {
                using (ITransaction transaction = sqlite.BeginTransaction()) {

                    if (CommandExists(sqlite)) {
                        return -2;
                    }

                    int size = (int)Order.PlannedQtyBUC;
                    Order.Status = Properties.Resources.CMD_PRLS;
                    Enumerable.Range(1, size).ToList().ForEach(data => {
                        var local = new ProductionOrderPailStatus() {
                            CreationDate = DateTime.Now,
                            PailNumber = $"{data}",
                            POID = Order.POID,
                            PailStatus = Properties.Resources.CMD_ELB,
                            NetWeight = OrderFinalItem[0].ItemQty / Order.PlannedQtyBUC,
                            GrossWeight = 0,
                            QC = false,
                            Timeout = "10",
                            StartDate = DateTime.Now,
                            EndDate = DateTime.Now,
                            MPGRowUpdated = DateTime.Now
                        };

                        list.Add(local);
                        _ = sqlite.Save(local);
                    });

                    DataUOMS.ForEach(item => sqlite.Save(item));
                    OrderBOM.ForEach(item => sqlite.Save(item));
                    OrderFinalItem.ForEach(item => sqlite.Save(item));
                    LotDetails.ForEach(item => sqlite.Save(item));
                    _ = sqlite.Save(LotHeader);
                    _ = sqlite.Save(Order);
                    transaction.Commit();
                }

            }

            using (var session = MesDb.Instance.GetSession()) {
                using (var transaction = session.BeginTransaction()) {
                    list.ForEach(item => {
                        session.Save(item);
                    });
                    transaction.Commit();
                }
            }

            return 0;
        }

        /// <summary>
        /// Used to block a command and save it in the database
        /// </summary>
        public void BlockCommand() {
            using (ISession session = SqliteDB.Instance.GetSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    Order.Status = Properties.Resources.CMD_BLOCK;
                    Order.Priority = "-1";
                    _ = session.Save(Order);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// TODO: TO be implemented 
        /// Should do sommething
        /// </summary>
        /// <returns></returns>
        public bool[] SetQC() {
            bool[] local = new bool[decimal.ToInt32(Order.PlannedQtyBUC)];
            string poz = LotHeader.PozQC;

            if (string.IsNullOrEmpty(poz)) {
                return local;
            }

            if (poz.Contains(";")) {
                poz.Split(";").ToList().ForEach(item => local[int.Parse(item, CultureInfo.InvariantCulture)] = true);
            } else if (poz.Contains("-")) {
                string[] splits = poz.Split("-");

                int min = int.Parse(splits[0], CultureInfo.InvariantCulture);
                int max = int.Parse(splits[1], CultureInfo.InvariantCulture);
                Enumerable.Range(min, max + 1).ToList().ForEach(data => local[data] = true);
            }

            return local;
        }

        /// <summary>
        /// Checks if the priority is set
        /// </summary>
        /// <returns>False if the priority is null or empty
        /// <br>True otherwise</br></returns>
        private bool PriorityEmpty() {
            return string.IsNullOrEmpty(Order.Priority);
        }

        /// <summary>
        /// Checks if there is any command in production
        /// </summary>
        /// <param name="session">Reference to the current session</param>
        /// <returns>True if there is a command in the production  <br/> False otherwise</returns>
        private bool CommandExists(ISession session) {
            return session.Query<ProductionOrder>().Any(p => p.POID == Order.POID);
        }
    }
}
