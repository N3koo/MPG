using DataEntity.Model.Input;
using DataEntity.Model.Output;

using System.Globalization;

namespace MES_Service.DTO {

    public record ProductionDto {

        public string POID { get; init; }

        public string POID_ID { get; init; }

        public string Name { get; init; }

        public string MaterialID { get; init; }

        public decimal Quantity { get; init; }

        public string Unit { get; init; }

        public string Date { get; init; }

        public string KoberLot { get; init; }

        public string Status { get; init; }

        public string BonPredare { get; init; }

        public string Consumption { get; init; }

        public static ProductionDto CreateFromPO(ProductionOrder order) {
            return new ProductionDto {
                POID = order.POID,
                POID_ID = "-1",
                Name = order.PODescription,
                Quantity = order.PlannedQtyBUC,
                Status = order.Status,
                Unit = order.PlannedQtyBUCUom,
                KoberLot = order.KoberLot,
                MaterialID = order.MaterialID
            };
        }

        public static ProductionDto CreateFromPailStatus(ProductionOrderPailStatus pail) {
            return new ProductionDto {
                POID = $"{pail.POID}_{pail.PailNumber}",
                POID_ID = pail.POID,
                Quantity = pail.NetWeight,
                BonPredare = pail.Ticket,
                Consumption = pail.Consumption,
                Status = pail.PailStatus,
                Unit = "KG",
                Date = pail.StartDate.ToString(CultureInfo.InvariantCulture)
            };
        }
    }
}
