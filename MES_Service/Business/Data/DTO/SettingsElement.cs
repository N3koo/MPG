using System.Text.Json.Serialization;

namespace MpgWebService.DTO {

    public record SettingsElement {

        [JsonPropertyName("Name")]
        public string Name { set; get; }

        [JsonPropertyName("DefaultValue")]
        public string DefaultValue { set; get; }
    }
}
