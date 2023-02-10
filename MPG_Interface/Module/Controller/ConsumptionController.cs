using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

using NHibernate.Transform;
using NHibernate;

using DataEntity.Config;

namespace MPG_Interface.Module.Controller {

    public class JoinResult {
        public string Item { set; get; }
        public string Description { set; get; }
        public double NetQuantity { set; get; }
        public string ItemUom { set; get; }
    }

    public class ConsumptionController {

        private readonly string QueryCommand = "SELECT pc.item, md.Description, SUM(pc.itemQty) AS NetQuantity, pc.ItemUom " +
               "FROM MPG2MES_ProductionOrderConsumptions pc LEFT JOIN MES2MPG_MaterialData md ON pc.Item = md.MaterialID " +
               "WHERE pc.POID = ? GROUP BY pc.Item;";

        private readonly string QueryPail = "SELECT pc.item, md.Description, pc.itemQty AS NetQuantity, pc.ItemUom " +
                "FROM MPG2MES_ProductionOrderConsumptions pc LEFT JOIN MES2MPG_MaterialData md ON pc.Item = md.MaterialID " +
                "WHERE pc.POID = ? AND pc.PailNumber = ? GROUP BY pc.Item;";

        private readonly DataGrid _grid;

        public ConsumptionController(List<object> items) {
            _grid = items[0] as DataGrid;

            SetEvents();
        }

        private void SetEvents() {
            _grid.LoadingRow += (sender, args) => {
                args.Row.Header = args.Row.GetIndex() + 1;
            };

            _grid.FontSize = 14;
            _grid.FontWeight = FontWeights.DemiBold;
        }

        public void SetDataCommand(string poid) {
            using (ISession session = SqliteDB.Instance.GetSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    IList<JoinResult> result = session.CreateSQLQuery(QueryCommand)
                        .SetResultTransformer(Transformers.AliasToBean<JoinResult>())
                        .SetString(0, poid).List<JoinResult>();

                    _grid.ItemsSource = result;
                }
            }
        }

        public void SetDataPail(string poid, int pail) {
            using (ISession session = SqliteDB.Instance.GetSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    IList<JoinResult> result = session.CreateSQLQuery(QueryPail)
                        .SetResultTransformer(Transformers.AliasToBean<JoinResult>())
                        .SetString(0, poid)
                        .SetInt32(1, pail)
                        .List<JoinResult>();

                    _grid.ItemsSource = result;
                }
            }
        }
    }
}
