namespace MpgWebService.Presentation.Request.MPG {

    public record QcDetails {

        public string POID { init; get; }
        public string OpNo { init; get; }
        public int PailNumber { init; get; }

    }
}
