using DataEntity.Model.Input;

namespace MpgWebService.Presentation.Response {
    public record Materials {
        public string POID { init; get; }
        public string MaterialID { init; get; }
        public string BomID { init; get; }
        public string BomAlternative { init; get; }
        public string ItemPosition { init; get; }
        public string ItemStorageLoc { init; get; }
        public string ItemProposedLot { init; get; }
        public string Item { init; get; }
        public decimal ItemQty { init; get; }
        public string ItemQtyUOM { init; get; }

        public static Materials FromBom(ProductionOrderBom bom) => new() {
            POID = bom.POID,
            MaterialID = bom.MaterialID,
            BomID = bom.BomID,
            BomAlternative = bom.BomAlternative,
            ItemPosition = bom.ItemPosition,
            ItemStorageLoc = bom.ItemStorageLoc,
            ItemProposedLot = bom.ItemProposedLot,
            Item = bom.Item,
            ItemQty = bom.ItemQty,
            ItemQtyUOM = bom.ItemQtyUOM
        };
    }
}
