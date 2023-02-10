using FluentNHibernate.Mapping;

using System;

namespace DataEntity.Model.Output {

    public class ProductionOrderStatus {
        public virtual int ID { set; get; }
        public virtual DateTime CreationDate { set; get; }
        public virtual string Plant { set; get; }
        public virtual string POID { set; get; }
        public virtual string MaterialID { set; get; }
        public virtual string LotNumber { set; get; }
        public virtual DateTime StartDate { set; get; }
        public virtual string TimeStatus { set; get; }
        public virtual string OrderStatus { set; get; }
        public virtual int MPGStatus { set; get; }
        public virtual int MESStatus { set; get; }
        public virtual string ErrorMessage { set; get; }
        public virtual DateTime MPGRowUpdated { set; get; }
    }

    public class ProductionOrderStatusMap : ClassMap<ProductionOrderStatus> {

        public ProductionOrderStatusMap() {
            Table("MPG2MES_ProductionOrderStatus");

            _ = Id(x => x.ID).Not.Nullable();
            _ = Map(x => x.CreationDate).Not.Nullable();
            _ = Map(x => x.Plant).Not.Nullable();
            _ = Map(x => x.POID).Not.Nullable();
            _ = Map(x => x.MaterialID).Not.Nullable();
            _ = Map(x => x.LotNumber).Not.Nullable();
            _ = Map(x => x.StartDate).Not.Nullable();
            _ = Map(x => x.TimeStatus).Not.Nullable();
            _ = Map(x => x.OrderStatus).Not.Nullable();
            _ = Map(x => x.MPGStatus).Nullable();
            _ = Map(x => x.MESStatus).Nullable();
            _ = Map(x => x.ErrorMessage).Nullable();
            _ = Map(x => x.MPGRowUpdated).Nullable();
        }
    }
}
