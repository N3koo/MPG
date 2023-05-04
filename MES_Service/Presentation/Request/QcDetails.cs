namespace MpgWebService.Presentation.Request {

    public record QcDetails {
        public string POID { init; get; }
        public string OpNo { init; get; }
        public string PailNumber { init; get; }
    }
}
