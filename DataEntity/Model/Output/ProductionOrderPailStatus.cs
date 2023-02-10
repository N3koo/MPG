using FluentNHibernate.Mapping;

using System;

namespace DataEntity.Model.Output {

    public class ProductionOrderPailStatus {
        public virtual int ID { set; get; }
        public virtual DateTime CreationDate { set; get; }
        public virtual string POID { set; get; }
        public virtual string PailNumber { set; get; }
        public virtual DateTime StartDate { set; get; }
        public virtual DateTime EndDate { set; get; }
        public virtual decimal NetWeight { set; get; }
        public virtual decimal GrossWeight { set; get; }
        public virtual string PailStatus { set; get; }
        public virtual bool QC { set; get; }
        public virtual string Op_No { set; get; }
        public virtual string MES_Sample_ID { set; get; }
        public virtual string Timeout { set; get; }
        public virtual string Ticket { set; get; }
        public virtual string Consumption { set; get; }
        public virtual int MPGStatus { set; get; }
        public virtual int MESStatus { set; get; }
        public virtual string ErrorMessage { set; get; }
        public virtual DateTime MPGRowUpdated { set; get; }
    }

    public class ProductionOrderPailStatusMap : ClassMap<ProductionOrderPailStatus> {

        public ProductionOrderPailStatusMap() {
            Table("MPG2MES_ProductionOrderPailStatus");

            _ = Id(x => x.ID).Not.Nullable();

            _ = Map(x => x.CreationDate).Not.Nullable();
            _ = Map(x => x.POID).Not.Nullable();
            _ = Map(x => x.PailNumber).Not.Nullable();
            _ = Map(x => x.NetWeight).Not.Nullable();
            _ = Map(x => x.GrossWeight).Not.Nullable();
            _ = Map(x => x.StartDate).Not.Nullable();
            _ = Map(x => x.EndDate).Not.Nullable();
            _ = Map(x => x.PailStatus).Not.Nullable();
            _ = Map(x => x.QC).Not.Nullable();
            _ = Map(x => x.Timeout).Not.Nullable();
            _ = Map(x => x.Op_No).Nullable();
            _ = Map(x => x.MES_Sample_ID).Nullable();
            _ = Map(x => x.Ticket).Nullable();
            _ = Map(x => x.Consumption).Nullable();
            _ = Map(x => x.MPGStatus).Nullable();
            _ = Map(x => x.MESStatus).Nullable();
            _ = Map(x => x.ErrorMessage).Nullable();
            _ = Map(x => x.MPGRowUpdated).Nullable();
        }
    }
}
