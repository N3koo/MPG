using System.Text.Json.Serialization;

namespace MpgWebService.DTO {
    public record ReportCommand {

        [JsonPropertyName("POID")]
        public string POID { get; init; }

        [JsonPropertyName("POID_ID")]
        public string POID_ID { get; init; }

        [JsonPropertyName("Name")]
        public string Name { get; init; }

        [JsonPropertyName("Product")]
        public string Product { get; init; }

        [JsonPropertyName("KoberLot")]
        public string KoberLot { get; init; }

        [JsonPropertyName("Quantity")]
        public decimal Quantity { get; init; }

        [JsonPropertyName("UOM")]
        public string UOM { get; init; }

        [JsonPropertyName("StartDate")]
        public string StartDate { get; init; }

        [JsonPropertyName("EndDate")]
        public string EndDate { get; init; }

        [JsonPropertyName("ExecuteDate")]
        public string ExecuteDate { get; init; }

        [JsonPropertyName("Status")]
        public string Status { get; init; }

        [JsonPropertyName("QC")]
        public bool QC { get; init; }
    }
}
