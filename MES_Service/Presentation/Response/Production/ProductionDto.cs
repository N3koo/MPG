using System.Text.Json.Serialization;

namespace MpgWebService.Presentation.Response.Production {

    public record ProductionDto {

        [JsonPropertyName("POID")]
        public string POID { get; init; }

        [JsonPropertyName("POID_ID")]
        public string POID_ID { get; init; }

        [JsonPropertyName("Name")]
        public string Name { get; init; }

        [JsonPropertyName("MaterialID")]
        public string MaterialID { get; init; }

        [JsonPropertyName("Quantity")]
        public decimal Quantity { get; init; }

        [JsonPropertyName("Unit")]
        public string Unit { get; init; }

        [JsonPropertyName("Date")]
        public string Date { get; init; }

        [JsonPropertyName("KoberLot")]
        public string KoberLot { get; init; }

        [JsonPropertyName("Status")]
        public string Status { get; init; }

        [JsonPropertyName("BonPredare")]
        public string BonPredare { get; init; }

        [JsonPropertyName("Consumption")]
        public string Consumption { get; init; }
    }
}
