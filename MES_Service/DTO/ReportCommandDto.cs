using DataEntity.Model.Input;
using DataEntity.Model.Output;
using System;
using System.Globalization;

namespace MES_Service.DTO {
    public record ReportCommandDto {

        public string POID { get; init; }

        public string POID_ID { get; init; }

        public string Name { get; init; }

        public string Product { get; init; }

        public string KoberLot { get; init; }

        public decimal Quantity { get; init; }

        public string UOM { get; init; }

        public string StartDate { get; init; }

        public string EndDate { get; init; }

        public string ExecuteDate { get; init; }

        public string Status { get; init; }

        public bool QC { get; init; }

        public static ReportCommandDto CreateFromPO(ProductionOrder item) {
            return new ReportCommandDto {
                POID = item.POID,
                POID_ID = "-1",
                KoberLot = item.KoberLot,
                Name = item.PODescription,
                Quantity = item.PlannedQtyBUC,
                UOM = item.PlannedQtyBUCUom,
                StartDate = item.PlannedStartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                EndDate = item.PlannedEndDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                ExecuteDate = null,
                Status = item.Status
            };
        }

        public static ReportCommandDto CreateFromPailStatus(ProductionOrderPailStatus pail, ProductionOrder order) {
            return new ReportCommandDto {
                POID = $"{pail.POID}_{pail.PailNumber}",
                POID_ID = pail.POID,
                Quantity = pail.GrossWeight,
                EndDate = order.PlannedEndDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                StartDate = order.PlannedStartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                ExecuteDate = pail.StartDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                Status = pail.PailStatus,
                QC = pail.QC,
                UOM = "KG",
            };
        }
    }
}
