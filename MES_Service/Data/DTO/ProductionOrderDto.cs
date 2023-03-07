using System.Text.Json.Serialization;
using System;

namespace MpgWebService.DTO {

    public record ProductionOrderDto {

        [JsonPropertyName("PODescription")]
        public string PODescription { get; init; }

        [JsonPropertyName("POID")]
        public string POID { get; init; }

        [JsonPropertyName("MaterialID")]
        public string MaterialID { get; init; }

        [JsonPropertyName("PlantID")]
        public string PlantID { get; init; }

        [JsonPropertyName("Status")]
        public string Status { get; init; }

        [JsonPropertyName("PlannedQtyBUC")]
        public decimal PlannedQtyBUC { get; init; }

        [JsonPropertyName("PlannedQtyBUCUom")]
        public string PlannedQtyBUCUom { get; init; }

        [JsonPropertyName("KoberLot")]
        public string KoberLot { get; init; }

        [JsonPropertyName("ProfitCenter")]
        public string ProfitCenter { get; init; }

        [JsonPropertyName("Priority")]
        public string Priority { get; init; }

        [JsonPropertyName("PlannedStartDate")]
        public DateTime PlannedStartDate { get; init; }

        [JsonPropertyName("PlannedStartHour")]
        public string PlannedStartHour { get; init; }

        [JsonPropertyName("PlannedEndDate")]
        public DateTime PlannedEndDate { get; init; }

        [JsonPropertyName("PlannedEndHour")]
        public string PlannedEndHour { get; init; }
    }
}
