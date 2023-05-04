namespace MpgWebService.Business.Data.DTO {
    public record QcLabel {
        public string Plant { get; init; }
        public string MaterialID { get; init; }
        public string Quantity { get; init; }
        public string PODescription { get; init; }
        public string POID { get; init; }
        public string PailNumber { get; init; }
        public string KoberLot { get; init; }
        public string ControlLot { get; init; }
        public string OpQM { get; init; }
    }
}