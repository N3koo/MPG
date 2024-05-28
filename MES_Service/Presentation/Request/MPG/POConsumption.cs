namespace MpgWebService.Presentation.Request.MPG {

    public record POConsumption {

        public string POID { init; get; }
        public string PailNumber { init; get; }
        public UsedMaterial[] Materials { init; get; }

    }
}
