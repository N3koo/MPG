namespace MpgWebService.Presentation.Response {

    public record QcLabelDto {

        public string Plant { init; get; }
        public string MaterialID { init; get; }
        public string Quantity { init; get; }
        public string StartDate { init; get; }
        public string PODescription { init; get; }
        public string POID { init; get; }
        public string PailNumber { init; get; }
        public string KoberLot { init; get; }
        public string ControlLot {  init; get; }
        public string OpQM { init; get; }
        public string QmDesc { init; get; }

    }
}
