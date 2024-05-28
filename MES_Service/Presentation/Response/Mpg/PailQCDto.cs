namespace MpgWebService.Presentation.Response.Mpg {

    public record PailQCDto {

        public string POID { init; get; }
        public decimal Priority { init; get; }
        public string PailNumber { init; get; }
        public decimal MixingTime { init; get; }
        public decimal PailWeight { init; get; }

    }
}
