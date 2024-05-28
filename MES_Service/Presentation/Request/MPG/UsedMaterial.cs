namespace MpgWebService.Presentation.Request.MPG
{
    public record UsedMaterial
    {
        public string Item { init; get; }
        public decimal ItemQty { init; get; }
        public string ItemUom { init; get; }
    }
}
