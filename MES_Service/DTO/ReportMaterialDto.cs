namespace MES_Service.DTO {

    public record ReportMaterialDto {

        public string Item { get; init; }

        public string Description { get; init; }

        public double NetQuantity { get; init; }

        public string ItemUom { get; init; }
    }
}
