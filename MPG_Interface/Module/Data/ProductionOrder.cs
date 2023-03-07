using System;

namespace MPG_Interface.Module.Data {

    public record ProductionOrder {

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

        public string PlannedStartHour { get; init; }

        public DateTime PlannedEndDate { get; init; }

        public string PlannedEndHour { get; init; }
    }
}
