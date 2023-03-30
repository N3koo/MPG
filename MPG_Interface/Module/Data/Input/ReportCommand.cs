namespace MPG_Interface.Module.Data.Input {

    public record ReportCommand {

        public string POID { init; get; }

        public string POID_ID { init; get; }

        public string Name { init; get; }

        public string Product { init; get; }

        public string KoberLot { init; get; }

        public decimal Quantity { init; get; }

        public string UOM { init; get; }

        public string StartDate { init; get; }

        public string EndDate { init; get; }

        public string ExecuteDate { init; get; }

        public string Status { init; get; }

        public bool QC { init; get; }
    }
}
