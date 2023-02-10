using FluentNHibernate.Mapping;

using SAPServices;

namespace DataEntity.Model.Input {

    public class ProductionOrderBom {
        public virtual string POID { set; get; }
        public virtual string MaterialID { set; get; }
        public virtual string BomID { set; get; }
        public virtual string BomAlternative { set; get; }
        public virtual string ItemPosition { set; get; }
        public virtual string ItemStorageLoc { set; get; }
        public virtual string ItemProposedLot { set; get; }
        public virtual string Item { set; get; }
        public virtual decimal ItemQty { set; get; }
        public virtual string ItemQtyUOM { set; get; }

        public ProductionOrderBom() {

        }

        public ProductionOrderBom(ZMPGPOLDM data) {
            POID = data.POID;
            MaterialID = data.MATERIALID;
            BomID = data.BOMID;
            BomAlternative = data.BOMATERNATIVE;
            ItemPosition = data.ITEMPOSITION;
            ItemStorageLoc = data.ITEMSTORAGELOC;
            ItemProposedLot = data.ITEMPROPOSEDLOT;
            Item = data.ITEM;
            ItemQty = data.ITEMQTY;
            ItemQtyUOM = data.ITEMQTYUOM;
        }

        public override bool Equals(object obj) {
            if (obj is not ProductionOrderBom other) return false;
            if (ReferenceEquals(this, other)) return true;

            return POID == other.POID && MaterialID == other.MaterialID && Item == other.Item;
        }

        public override int GetHashCode() {
            return System.HashCode.Combine(POID, MaterialID, Item);
        }
    }

    public class ProductionOrderBomMap : ClassMap<ProductionOrderBom> {
        public ProductionOrderBomMap() {
            Table("MES2MPG_ProductionOrderBOM");

            _ = CompositeId().KeyProperty(x => x.POID).KeyProperty(x => x.MaterialID).KeyProperty(x => x.Item);

            _ = Map(x => x.BomID).Not.Nullable();
            _ = Map(x => x.BomAlternative).Not.Nullable();
            _ = Map(x => x.ItemPosition).Not.Nullable();
            _ = Map(x => x.ItemQty).Not.Nullable();
            _ = Map(x => x.ItemQtyUOM).Not.Nullable();
            _ = Map(x => x.ItemStorageLoc).Not.Nullable();
            _ = Map(x => x.ItemProposedLot).Not.Nullable();
        }
    }
}
