namespace MpgWebService.Presentation.Request {
    public record UsedMaterial {
        public string Item { init; get; }
        public decimal ItemQty { init; get; }
        public string ItemUom { init; get; }
    }
}
