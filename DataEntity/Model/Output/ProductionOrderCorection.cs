using FluentNHibernate.Mapping;

using System;

namespace DataEntity.Model.Output {

    public class ProductionOrderCorection {
        public virtual int ID { set; get; }
        public virtual DateTime CreationDate { set; get; }
        public virtual string POID { set; get; }
        public virtual string MaterialID { set; get; }
        public virtual string PailNumber { set; get; }
        public virtual int CorrectionID { set; get; }
        public virtual string Item_1 { set; get; }
        public virtual double ItemQty_1 { set; get; }
        public virtual string ItemUom_1 { set; get; }
        public virtual string Item_2 { set; get; }
        public virtual double ItemQty_2 { set; get; }
        public virtual string ItemUom_2 { set; get; }
        public virtual string Item_3 { set; get; }
        public virtual double ItemQty_3 { set; get; }
        public virtual string ItemUom_3 { set; get; }
        public virtual string Item_4 { set; get; }
        public virtual double ItemQty_4 { set; get; }
        public virtual string ItemUom_4 { set; get; }
        public virtual string Item_5 { set; get; }
        public virtual double ItemQty_5 { set; get; }
        public virtual string ItemUom_5 { set; get; }
        public virtual string Item_6 { set; get; }
        public virtual double ItemQty_6 { set; get; }
        public virtual string ItemUom_6 { set; get; }
        public virtual string Item_7 { set; get; }
        public virtual double ItemQty_7 { set; get; }
        public virtual string ItemUom_7 { set; get; }
        public virtual string Item_8 { set; get; }
        public virtual double ItemQty_8 { set; get; }
        public virtual string ItemUom_8 { set; get; }
        public virtual string Item_9 { set; get; }
        public virtual double ItemQty_9 { set; get; }
        public virtual string ItemUom_9 { set; get; }
        public virtual string Item_10 { set; get; }
        public virtual double ItemQty_10 { set; get; }
        public virtual string ItemUom_10 { set; get; }
        public virtual int? MPGStatus { set; get; }
        public virtual int? MESStatus { set; get; }
        public virtual string ErrorMessage { set; get; }
        public virtual DateTime? MPGRowUpdated { set; get; }
    }

    public class ProductionOrderCorectionMap : ClassMap<ProductionOrderCorection> {

        public ProductionOrderCorectionMap() {
            Table("MPG2MES_ProductionOrderCorrections");

            _ = Id(x => x.ID).Not.Nullable();
            _ = Map(x => x.CreationDate).Not.Nullable();
            _ = Map(x => x.POID).Not.Nullable();
            _ = Map(x => x.MaterialID).Not.Nullable();
            _ = Map(x => x.PailNumber).Not.Nullable();
            _ = Map(x => x.CorrectionID).Not.Nullable();
            _ = Map(x => x.Item_1).Not.Nullable();
            _ = Map(x => x.ItemQty_1).Not.Nullable();
            _ = Map(x => x.ItemUom_1).Not.Nullable();
            _ = Map(x => x.Item_2).Not.Nullable();
            _ = Map(x => x.ItemQty_2).Not.Nullable();
            _ = Map(x => x.ItemUom_2).Not.Nullable();
            _ = Map(x => x.Item_3).Not.Nullable();
            _ = Map(x => x.ItemQty_3).Not.Nullable();
            _ = Map(x => x.ItemUom_3).Not.Nullable();
            _ = Map(x => x.Item_4).Not.Nullable();
            _ = Map(x => x.ItemQty_4).Not.Nullable();
            _ = Map(x => x.ItemUom_4).Not.Nullable();
            _ = Map(x => x.Item_5).Not.Nullable();
            _ = Map(x => x.ItemQty_5).Not.Nullable();
            _ = Map(x => x.ItemUom_5).Not.Nullable();
            _ = Map(x => x.Item_6).Not.Nullable();
            _ = Map(x => x.ItemQty_6).Not.Nullable();
            _ = Map(x => x.ItemUom_6).Not.Nullable();
            _ = Map(x => x.Item_7).Not.Nullable();
            _ = Map(x => x.ItemQty_7).Not.Nullable();
            _ = Map(x => x.ItemUom_7).Not.Nullable();
            _ = Map(x => x.Item_8).Not.Nullable();
            _ = Map(x => x.ItemQty_8).Not.Nullable();
            _ = Map(x => x.ItemUom_8).Not.Nullable();
            _ = Map(x => x.Item_9).Not.Nullable();
            _ = Map(x => x.ItemQty_9).Not.Nullable();
            _ = Map(x => x.ItemUom_9).Not.Nullable();
            _ = Map(x => x.Item_10).Not.Nullable();
            _ = Map(x => x.ItemQty_10).Not.Nullable();
            _ = Map(x => x.ItemUom_10).Not.Nullable();
            _ = Map(x => x.MPGStatus).Nullable();
            _ = Map(x => x.MESStatus).Nullable();
            _ = Map(x => x.ErrorMessage).Nullable();
            _ = Map(x => x.MPGRowUpdated).Nullable();
        }
    }
}
