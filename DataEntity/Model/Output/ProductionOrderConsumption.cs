using FluentNHibernate.Mapping;

using System;

namespace DataEntity.Model.Output {

    public class ProductionOrderConsumption {
        public virtual int ID { set; get; }
        public virtual DateTime CreationDate { set; get; }
        public virtual string POID { set; get; }
        public virtual string MaterialID { set; get; }
        public virtual string PailNumber { set; get; }
        public virtual string Item { set; get; }
        public virtual decimal ItemQty { set; get; }
        public virtual string ItemUom { set; get; }
        public virtual string ItemLot { set; get; }
        public virtual string ItemStorageLoc { set; get; }
        public virtual int MPGStatus { set; get; }
        public virtual int MESStatus { set; get; }
        public virtual string ErrorMessage { set; get; }
        public virtual DateTime MPGRowUpdated { set; get; }
    }

    public class ProductionOrderConsumptionMap : ClassMap<ProductionOrderConsumption> {
        public ProductionOrderConsumptionMap() {
            Table("MPG2MES_ProductionOrderConsumptions");

            _ = Id(x => x.ID).Not.Nullable();
            _ = Map(x => x.CreationDate).Not.Nullable();
            _ = Map(x => x.POID).Not.Nullable();
            _ = Map(x => x.MaterialID).Not.Nullable();
            _ = Map(x => x.PailNumber).Not.Nullable();
            _ = Map(x => x.Item).Not.Nullable();
            _ = Map(x => x.ItemQty).Not.Nullable();
            _ = Map(x => x.ItemUom).Not.Nullable();
            _ = Map(x => x.ItemStorageLoc).Not.Nullable();
            _ = Map(x => x.MPGStatus).Nullable();
            _ = Map(x => x.MESStatus).Nullable();
            _ = Map(x => x.ErrorMessage).Nullable();
            _ = Map(x => x.MPGRowUpdated).Nullable();
        }
    }
}
