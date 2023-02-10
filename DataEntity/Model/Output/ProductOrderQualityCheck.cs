using FluentNHibernate.Mapping;

using System;

namespace DataEntity.Model.Output {

    public class ProductOrderQualityCheck {
        public virtual int ID { set; get; }
        public virtual DateTime CreationDate { set; get; }
        public virtual string Plant { set; get; }
        public virtual string POID { set; get; }
        public virtual string MaterialID { set; get; }
        public virtual string LotNumber { set; get; }
        public virtual string PailNumber { set; get; }
        public virtual string InspectionLotID { set; get; }
        public virtual string OpNo { set; get; }
        public virtual string OpDesc { set; get; }
        public virtual int StatusQuality { set; get; }
        public virtual string StatusMessage { set; get; }
        public virtual int QualityOK { set; get; }
        public virtual int MPGStatus { set; get; }
        public virtual int MESStatus { set; get; }
        public virtual string ErrorMessage { set; get; }
        public virtual DateTime MPGRowUpdated { set; get; }
    }

    public class ProductOrderQualityCheckMap : ClassMap<ProductOrderQualityCheck> {

        public ProductOrderQualityCheckMap() {
            Table("MPG2MES_ProductionOrderQualityCheck");

            _ = Id(x => x.ID).Not.Nullable();
            _ = Map(x => x.CreationDate).Not.Nullable();
            _ = Map(x => x.Plant).Not.Nullable();
            _ = Map(x => x.POID).Not.Nullable();
            _ = Map(x => x.MaterialID).Not.Nullable();
            _ = Map(x => x.LotNumber).Not.Nullable();
            _ = Map(x => x.PailNumber).Not.Nullable();
            _ = Map(x => x.InspectionLotID).Not.Nullable();
            _ = Map(x => x.OpNo).Not.Nullable();
            _ = Map(x => x.OpDesc).Not.Nullable();
            _ = Map(x => x.StatusQuality).Not.Nullable();
            _ = Map(x => x.StatusMessage).Nullable();
            _ = Map(x => x.QualityOK).Not.Nullable();
            _ = Map(x => x.MPGStatus).Nullable();
            _ = Map(x => x.MESStatus).Nullable();
            _ = Map(x => x.ErrorMessage).Nullable();
            _ = Map(x => x.MPGRowUpdated).Nullable();
        }
    }
}
