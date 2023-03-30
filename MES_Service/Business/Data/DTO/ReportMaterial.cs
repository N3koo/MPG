using System.Text.Json.Serialization;

namespace MpgWebService.DTO {

    public record ReportMaterial {

        [JsonPropertyName("Item")]
        public string Item { get; init; }

        [JsonPropertyName("Description")]
        public string Description { get; init; }

        [JsonPropertyName("NetQuantity")]
        public double NetQuantity { get; init; }

        [JsonPropertyName("BrutQuantity")]
        public double BrutQuantity { get; set; }

        [JsonPropertyName("ItemUom")]
        public string ItemUom { get; init; }
    }
}
