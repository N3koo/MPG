using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System;

using DataEntity.Model.Output;
using DataEntity.Model.Input;
using DataEntity.Config;
using NHibernate;

namespace MPG_Interface.Module.Visual.ViewModel {

    public class Report {

        public string POID { set; get; }

        public string POID_ID { set; get; }

        public string Name { set; get; }

        public string Product { set; get; }

        public string KoberLot { set; get; }

        public string Quantity { set; get; }

        public string UOM { set; get; }

        public string StartDate { set; get; }

        public string EndDate { set; get; }

        public string ExecuteDate { set; get; }

        public string Status { set; get; }

        public string QC { set; get; }
    }

    public class ReportView {
        public ObservableCollection<Report> GetData(DateTime? startDate, DateTime? endDate) {
            Random ran = new();
            ObservableCollection<Report> data = new();
            using (ISession session = SqliteDB.Instance.GetSession()) {
                using (ITransaction transaction = session.BeginTransaction()) {
                    List<ProductionOrder> result = session.Query<ProductionOrder>().Where(p => p.PlannedStartDate >= startDate && p.PlannedEndDate <= endDate).ToList();

                    if (result.Count <= 0) {
                        Alerts.ShowMessage("Nu exista inregistrari pentru perioada selectata.");
                        return data;
                    }

                    result.ForEach(item => {
                        data.Add(new Report {
                            POID = item.POID,
                            POID_ID = "-1",
                            KoberLot = item.KoberLot,
                            Name = item.PODescription,
                            Product = item.MaterialID,
                            Quantity = item.PlannedQtyBUC.ToString(CultureInfo.InvariantCulture),
                            UOM = item.PlannedQtyBUCUom,
                            StartDate = item.PlannedStartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                            EndDate = item.PlannedEndDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                            ExecuteDate = null,
                            Status = item.Status,
                        });

                        session.Query<ProductionOrderPailStatus>().Where(p => p.POID == item.POID).ToList().ForEach(localPail => {
                            data.Add(new Report {
                                POID = $"{item.POID}_{localPail.PailNumber}",
                                POID_ID = item.POID,
                                Quantity = localPail.GrossWeight.ToString(CultureInfo.InvariantCulture),
                                EndDate = item.PlannedEndDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                                StartDate = item.PlannedStartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                                UOM = "KG",
                                ExecuteDate = localPail.StartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                                Status = localPail.PailStatus,
                                QC = ran.Next(0, 2) == 0 ? true.ToString() : false.ToString()
                            });
                        });

                    });
                }
            }

            return data;
        }
    }
}
