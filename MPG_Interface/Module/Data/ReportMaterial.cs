namespace MPG_Interface.Module.Data {

    public record ReportMaterial {

        public string Item { init; get; }

        public string Description { init; get; }

        public double NetQuantity { init; get; }

        public double BrutQuantity { init; get; }

        public string ItemUom { init; get; }

    }
}
