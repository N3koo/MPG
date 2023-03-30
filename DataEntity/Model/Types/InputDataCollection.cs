using DataEntity.Model.Input;
using DataEntity.Model.Output;
using SAPServices;

using System.Collections.Generic;
using System.Linq;

namespace DataEntity.Model.Types {

    /// <summary>
    /// 
    /// </summary>
    public class InputDataCollection {

        private static readonly List<InputData> _list = new();

        private InputDataCollection() {

        }

        public static List<InputData> GetList() {
            return _list;
        }

        public static void AddElement(InputData element) {
            _list.Add(element);
        }

        public static InputData GetElement(int index) {
            return _list[index];
        }

        public static InputData GetElement(string poid) {
            return _list.First(p => p.Order.POID == poid);
        }

        public static List<ProductionOrder> GetCommands() {
            return _list.Select(p => p.Order).ToList();
        }

        public static List<ProductionOrderPailStatus> ExportCommand(string poid, int prioriry, bool[] qc) {
            return _list.First(p => p.Order.POID == poid).ExportData(prioriry, qc);
        }

        public static ProductionOrder GetCommand(string poid) {
            return _list.Count == 0 ? null : _list.First(p => p.Order.POID == poid)?.Order;
        }

        public static bool CheckPriority(string poid, string value) {
            if (_list.Count == 0) {
                return false;
            }

            _list.Find(p => p.Order.POID == poid).Order.Priority = value;
            return true;
        }

        public static void Clear() {
            _list.Clear();
        }

        public static string GetQC(string poid) {
            return _list.FirstOrDefault(p => p.Order.POID == poid)?.LotHeader.PozQC;
        }


        public static void SetStatus(string poid, string status) {
            if (_list != null && _list.Count != 0) {
                _list.First(p => p.Order.POID == poid).Order.Status = status;
            }
        }

        public static List<ProductionOrder> GetCommandsByStatus(string status) {
            return string.IsNullOrEmpty(status) ? _list.Select(p => p.Order).ToList() : _list.Where(p => p.Order.Status == status).Select(p => p.Order).ToList();
        }

        /// <summary>
        /// Creates a new collection with the details
        /// </summary>
        /// <param name="poid">Command id</param>
        /// <param name="response">List with the data received from the SAP</param>
        public static void CreateCollection(string poid, Z_PRODORDERSResponse response) {
            InputData item = new();

            response.MPGPO.Where(p => p.POID == poid)
                .ToList().ForEach(data => item.Order = new ProductionOrder(data));

            response.MPGPOALTUOMS.Where(p => p.MATERIALID == item.Order.MaterialID)
                .ToList().ForEach(data => item.DataUOMS.Add(new MaterialDataUOMS(data)));

            response.MPGPOLDM.Where(p => p.POID == poid)
                .ToList().ForEach(data => item.OrderBOM.Add(new ProductionOrderBom(data)));

            response.MPGPOPF.Where(p => p.POID == poid)
                .ToList().ForEach(data => item.OrderFinalItem.Add(new ProductionOrderFinalItem(data)));

            response.MPGPOLOTDET.Where(p => p.POID == poid)
                .ToList().ForEach(data => item.LotDetails.Add(new ProductionOrderLotDetail(data)));

            response.MPGPOLOTHEADER.Where(p => p.POID == poid).ToList().ForEach(data => {
                item.LotHeader = new(data);
            });

            _list.Add(item);
        }
    }
}
