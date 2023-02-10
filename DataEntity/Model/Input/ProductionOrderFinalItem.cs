using FluentNHibernate.Mapping;

using SAPServices;

namespace DataEntity.Model.Input {

    public class ProductionOrderFinalItem {
        public virtual string POID { set; get; }
        public virtual string ItemPosition { set; get; }
        public virtual string Item { set; get; }
        public virtual decimal ItemQty { set; get; }
        public virtual string ItemQtyUOM { set; get; }
        public virtual string ItemStorageLoc { set; get; }
        public virtual string ItemProposedLot { set; get; }

        public ProductionOrderFinalItem() {

        }

        public ProductionOrderFinalItem(ZMPGPOPF data) {
            POID = data.POID;
            ItemPosition = data.ITEMPOSITION;
            Item = data.ITEM;
            ItemQty = data.ITEMQTY;
            ItemQtyUOM = data.ITEMQTYUOM;
            ItemStorageLoc = data.ITEMSTORAGELOC;
            ItemProposedLot = data.ITEMPROPOSEDLOT;
        }
    }

    public class ProductionOrderFinalItemMap : ClassMap<ProductionOrderFinalItem> {

        public ProductionOrderFinalItemMap() {
            Table("MES2MPG_ProductionOrderFinalItems");

            _ = Id(x => x.POID).Not.Nullable();
            _ = Map(x => x.ItemPosition).Not.Nullable();
            _ = Map(x => x.Item).Not.Nullable();
            _ = Map(x => x.ItemQty).Not.Nullable();
            _ = Map(x => x.ItemQtyUOM).Not.Nullable();
            _ = Map(x => x.ItemStorageLoc).Not.Nullable();
            _ = Map(x => x.ItemProposedLot).Not.Nullable();
        }
    }
}
