using DataEntity.Model.Output;
using DataEntity.Model.Input;
using DataEntity.Model.Types;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System;

using SAPServices;

using NHibernate;
using MPG_Interface.Module.Interfaces;

namespace MPG_Interface.Module.Logic {

    /// <summary>
    /// Created for saving generic functions
    /// </summary>
    public class Functions {

        /// <summary>
        /// Private constructor. Never to be used
        /// </summary>
        private Functions() {

        }

        /// <summary>
        /// Generic function used to save or update a list of items in a session
        /// </summary>
        /// <typeparam name="T">Generic type of data</typeparam>
        /// <param name="list">List with elements</param>
        /// <param name="session">Session</param>
        public static void SaveOrUpdate<T>(List<T> list, ISession session) {
            list.ForEach(item => {
                session.SaveOrUpdate(item);
            });
        }

        /// <summary>
        /// Generic function used to save a list of items in a session
        /// </summary>
        /// <typeparam name="T">Generic type of data</typeparam>
        /// <param name="list">List with elements</param>
        /// <param name="session">Session</param>
        public static void Save<T>(List<T> list, ISession session) {
            list.ForEach(item => {
                _ = session.Save(item);
            });
        }

        /// <summary>
        /// Sets the date when an updates is made
        /// TODO: Maybe will be removed
        /// </summary>
        public static void SetUpdateDate() {
            Properties.Settings.Default.Update = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Used to create a new data collection
        /// </summary>
        /// <param name="order">Contains start data</param>
        /// <param name="session">Session</param>
        /// <returns>New elements with the data needed</returns>
        public static InputData CreateData(ProductionOrder order, ISession session) {
            InputData inputData = new() {
                Order = new ProductionOrder(order)
            };
            inputData.DataUOMS = session.Query<MaterialDataUOMS>().Where(p => p.MaterialID == inputData.Order.MaterialID).ToList();
            inputData.Order.Priority = null;
            inputData.OrderBOM = session.Query<ProductionOrderBom>().Where(p => p.POID == inputData.Order.POID).ToList();
            inputData.OrderFinalItem = session.Query<ProductionOrderFinalItem>().Where(p => p.POID == inputData.Order.POID).ToList();
            inputData.LotDetails = session.Query<ProductionOrderLotDetail>().Where(p => p.POID == inputData.Order.POID).ToList();
            inputData.LotHeader = session.Query<ProductionOrderLotHeader>().FirstOrDefault(p => p.POID == inputData.Order.POID);
            inputData.QualityCheck = inputData.SetQC();
            return inputData;
        }

        /// <summary>
        /// Used for decreasing the priority after a command is done
        /// </summary>
        /// <param name="list">Contains the current commands in production</param>
        /// <param name="POID">ID of the current command</param>
        /// <param name="session">Session</param>
        public static void DecreasePriority(List<ProductionOrder> list, string POID, NHibernate.ISession session) {
            /// ToDo: Check if the cod works fine
            list.ForEach(item => {
                if (item.POID != POID) {
                    ulong value = ulong.Parse(item.Priority, CultureInfo.InvariantCulture) - 1;
                    item.Priority = value == 0 ? "1" : value.ToString(CultureInfo.InvariantCulture);
                    session.Update(item);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PO"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static Z_MPGPREDARE CreatePredare(ProductionOrder PO, List<ProductionOrderPailStatus> details, NHibernate.ISession session) {
            ProductionOrderFinalItem position = session.Query<ProductionOrderFinalItem>().First(p => p.POID == PO.POID);
            string date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            var receipeHeader = new ZGOODSRECEIPTHEADER {
                POID = PO.POID,
                GOODSRECEIPTTYPE = Properties.Resources.Goods_Receipt_Type,
                POSTINGDATE = date,
                DOCDATE = date,
                PLANT = PO.PlantID,
                PROFITCENTER = PO.ProfitCenter,
                HEADERTEXT = $"{PO.POID}-{PO.KoberLot}"
            };

            var receipeItem = new ZGOODSRECEIPTITEMS() {
                POID = PO.POID,
                GOODSRECEIPTTYPE = Properties.Resources.Goods_Receipt_Type,
                ITEMPOSITION = position.ItemPosition,
                MATERIALID = PO.MaterialID,
                QUANTITYPRODUCED = details.Count,
                UOM = PO.PlannedQtyBUCUom,
                STORAGELOC = PO.StorageLoc,
                KOBERLOT = PO.KoberLot
            };

            return new Z_MPGPREDARE() {
                GOODSRECEIPTHEADER = new ZGOODSRECEIPTHEADER[1] { receipeHeader },
                GOODSRECEIPTITEMS = new ZGOODSRECEIPTITEMS[1] { receipeItem }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PO"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static Z_MPGCONSUM CreateConsumption(ProductionOrder PO, NHibernate.ISession session) {
            List<ProductionOrderBom> ldm = session.Query<ProductionOrderBom>().Where(p => p.POID == PO.POID).ToList();
            string date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            List<ZCONSUMPTIONITEMS> list = new();

            var headerBC = new ZCONSUMPTIONHEADER {
                POID = PO.POID,
                CONSUMPTIONTYPE = Properties.Resources.Consumption_Type,
                POSTINGDATE = date,
                DOCDATE = date,
                MATERIALID = PO.MaterialID,
                PLANT = PO.PlantID,
                PROFITCENTER = PO.ProfitCenter,
                HEADERTEXT = $"{PO.POID}-{PO.KoberLot}",
                KOBERLOT = PO.KoberLot,
                REZERVATIONNUMBER = PO.RezervationNumber
            };

            ldm.ForEach(item => {
                list.Add(new ZCONSUMPTIONITEMS {
                    POID = PO.POID,
                    CONSUMPTIONTYPE = Properties.Resources.Consumption_Type,
                    ITEMPOSITION = item.ItemPosition,
                    ROWMATERIALID = item.ItemStorageLoc,
                    QUANTITY = item.ItemQty,
                    UOM = item.ItemQtyUOM,
                    LOT = item.ItemProposedLot
                });
            });

            return new Z_MPGCONSUM {
                CONSUMPTIONHEADER = new ZCONSUMPTIONHEADER[1] { headerBC },
                CONSUMPTIONITEMS = list.ToArray()
            };
        }

        /// <summary>
        /// Factory that creates the nedded connection
        /// </summary>
        /// <returns>Reference to the new input</returns>
        public static IInput CreateInput() {
            return Properties.Settings.Default.Connection == "SAP" ? new SapInput() : new MesInput();
        }
    }
}
