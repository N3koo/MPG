namespace MpgWebService.Presentation.Request.MPG {
    public record ReserveTank {

        public string MpgHead { init; get; }        
        public decimal Quantity { init; get; }

    }
}
