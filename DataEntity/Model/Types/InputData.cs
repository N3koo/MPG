using DataEntity.Model.Input;
using DataEntity.Model.Output;
using DataEntity.Config;

using NHibernate;

using System.Collections.Generic;
using System.Linq;
using DataEntity.Properties;

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
        /// Sends the command to the MPG database
        /// </summary>
        public List<ProductionOrderPailStatus> ExportData(int priority, bool[] qc) {
            List<ProductionOrderPailStatus> pails = new();

            using ISession sqlite = MpgDb.Instance.GetSession();
            using ITransaction transaction = sqlite.BeginTransaction();

            int size = (int)Order.PlannedQtyBUC;
            Order.Priority = priority.ToString(); //TODO: Remove this
            Order.Status = Settings.Default.CMD_PRLS;
            Enumerable.Range(1, size).ToList().ForEach(data => {
                var local = ProductionOrderPailStatus.CreatePail(this, data, qc);
                pails.Add(local);
                _ = sqlite.Save(local);
            });

            DataUOMS.ForEach(item => sqlite.Save(item));
            OrderBOM.ForEach(item => sqlite.Save(item));
            OrderFinalItem.ForEach(item => sqlite.Save(item));
            LotDetails.ForEach(item => sqlite.Save(item));
            _ = sqlite.Save(LotHeader);
            _ = sqlite.Save(Order);
            transaction.Commit();

            return pails;
        }
    }
}
