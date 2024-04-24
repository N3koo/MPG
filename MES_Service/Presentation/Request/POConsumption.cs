using MpgWebService.Presentation.Response;

namespace MpgWebService.Presentation.Request {
    public record POConsumption {
        public string POID { init; get; }
        public string PailNumber { init; get; }
        public UsedMaterial[] Materials { init; get; }
    }
}
