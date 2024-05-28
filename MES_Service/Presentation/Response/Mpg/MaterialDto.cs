namespace MpgWebService.Presentation.Response.Mpg {

    public record MaterialDto {

        public string MpgHead { init; get; }
        public string Item { init; get; }
        public decimal ItemQty { set; get; }
        public string ItemQtyUOM { init; get; }

    }
}
