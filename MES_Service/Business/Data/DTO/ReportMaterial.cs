using System.Text.Json.Serialization;

namespace MpgWebService.Business.Data.DTO {

    public record ReportMaterial {

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
