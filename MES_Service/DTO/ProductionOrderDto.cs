using System;

namespace MES_Service.DTO {
    public record ProductionOrderDto {

        public string PODescription { get; init; }
        public string POID { get; init; }
        public string MaterialID { get; init; }
        public string PlantID { get; init; }
        public string Status { get; init; }
        public decimal PlannedQtyBUC { get; init; }
        public string PlannedQtyBUCUom { get; init; }
        public string KoberLot { get; init; }
        public string ProfitCenter { get; init; }
        public string Priority { get; init; }
        public DateTime PlannedStartDate { get; init; }
        public virtual string PlannedStartHour { get; init; }
        public virtual DateTime PlannedEndDate { get; init; }
        public virtual string PlannedEndHour { get; init; }
    }
}
