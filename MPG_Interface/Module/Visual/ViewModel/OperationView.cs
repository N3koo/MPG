using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System;

using DataEntity.Model.Input;
using DataEntity.Model.Output;
using DataEntity.Config;

namespace MPG_Interface.Module.Visual.ViewModel {

    /// <summary>
    /// Used to define the class for the report window
    /// </summary>
    public class Operation {
        public string POID { set; get; }

        public string POID_ID { set; get; }

        public string Name { set; get; }

        public string MaterialID { set; get; }

        public decimal Quantity { set; get; }

        public string Unit { set; get; }

        public string Date { set; get; }

        public string KoberLot { set; get; }

        public string Status { set; get; }

        public string BonPredare { set; get; }

        public string Consumption { set; get; }
    }

    public class OperationView {
        public ObservableCollection<Operation> Data { get; set; }

        public OperationView() {
            Data = new ObservableCollection<Operation>();
        }

        public ObservableCollection<Operation> GetData(DateTime? start, DateTime? end) {
            using (NHibernate.ISession session = SqliteDB.Instance.GetSession()) {
                using (NHibernate.ITransaction transaction = session.BeginTransaction()) {
                    List<ProductionOrder> result = session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= start && p.PlannedEndDate <= end).ToList();

                    if (result.Count == 0) {
                        return Data;
                    }

                    Data.Clear();
                    result.ForEach(item => {
                        Data.Add(new Operation {
                            POID = item.POID,
                            POID_ID = "-1",
                            Name = item.PODescription,
                            Quantity = item.PlannedQtyBUC,
                            Status = item.Status,
                            Unit = item.PlannedQtyBUCUom,
                            KoberLot = item.KoberLot,
                            MaterialID = item.MaterialID
                        });

                        List<ProductionOrderPailStatus> details = session.Query<ProductionOrderPailStatus>().Where(p => p.POID == item.POID).ToList();

                        details.ForEach(detail => {
                            Data.Add(new Operation {
                                POID = $"{detail.POID}_{detail.PailNumber}",
                                POID_ID = detail.POID,
                                Quantity = detail.NetWeight,
                                BonPredare = detail.Ticket,
                                Consumption = detail.Consumption,
                                Status = detail.PailStatus,
                                Unit = "KG",
                                Date = detail.StartDate.ToString(CultureInfo.InvariantCulture)
                            });
                        });
                    });
                }
            }

            return Data;
        }
    }
}
