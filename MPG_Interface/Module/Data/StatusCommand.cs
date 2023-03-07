namespace MPG_Interface.Module.Data {

    public record StatusCommand {
        public string POID { init; get; }

        public string POID_ID { init; get; }

        public string Name { init; get; }

        public string MaterialID { init; get; }

        public decimal Quantity { init; get; }

        public string Unit { init; get; }

        public string Date { init; get; }

        public string KoberLot { init; get; }

        public string Status { init; get; }

        public string BonPredare { init; get; }

        public string Consumption { init; get; }
    }
}
