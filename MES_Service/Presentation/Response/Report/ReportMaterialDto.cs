using System.Text.Json.Serialization;

namespace MpgWebService.Presentation.Response.Report {

    public record ReportMaterialDto {

        [JsonPropertyName("Item")]
        public string Item { get; init; }

        [JsonPropertyName("Description")]
        public string Description { get; init; }

        [JsonPropertyName("NetQuantity")]
        public decimal NetQuantity { get; init; }

        [JsonPropertyName("BrutQuantity")]
        public decimal BrutQuantity { get; set; }

        [JsonPropertyName("ItemUom")]
        public string ItemUom { get; init; }
    }
}
