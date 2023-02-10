using FluentNHibernate.Mapping;

using SAPServices;

namespace DataEntity.Model.Input {

    public class ProductionOrderLotDetail {
        public virtual string POID { set; get; }
        public virtual string InspectionLotID { set; get; }
        public virtual string KoberLot { set; get; }
        public virtual string OpNo { set; get; }
        public virtual string OpDescr { set; get; }
        public virtual string OpResource { set; get; }
        public virtual string OpResourceDescr { set; get; }
        public virtual string OpCtrlKey { set; get; }
        public virtual string CaractPos { set; get; }
        public virtual string CaractText { set; get; }
        public virtual string CaractCode { set; get; }
        public virtual string CaractType { set; get; }
        public virtual string UOM { set; get; }
        public virtual decimal TargetValue { set; get; }
        public virtual decimal UpperLimit { set; get; }
        public virtual decimal LowerLimit { set; get; }

        public ProductionOrderLotDetail() {

        }

        public ProductionOrderLotDetail(ZMPGPOLOTDET data) {
            POID = data.POID;
            InspectionLotID = data.INSPECTIONLOTID;
            KoberLot = data.KOBERLOT;
            OpNo = data.OPNO;
            OpDescr = data.OP_DESCR;
            OpResource = data.OPRESOURCE;
            OpResourceDescr = data.OPRESOURCEDESCR;
            OpCtrlKey = data.OPCTRLKEY;
            CaractPos = data.CARACTPOS;
            CaractText = data.CARACTTEXT;
            CaractCode = data.CARACTCODE;
            CaractType = data.CARACTTYPE;
            UOM = data.UOM;
            TargetValue = data.TARGETVALUE;
            UpperLimit = data.UPPERLIMIT;
            LowerLimit = data.LOWERLIMIT;
        }

        public override bool Equals(object obj) {
            if (obj is not ProductionOrderLotDetail other) return false;
            if (ReferenceEquals(this, other)) return true;

            return POID == other.POID && OpDescr == other.OpDescr && OpNo == other.OpNo && CaractPos == other.CaractPos;
        }

        public override int GetHashCode() {
            return System.HashCode.Combine(POID, OpDescr, OpNo, CaractPos);
        }
    }

    public class ProductionOrderLotDetailMap : ClassMap<ProductionOrderLotDetail> {

        public ProductionOrderLotDetailMap() {
            Table("MES2MPG_ProductionOrderLotDetails");
            _ = CompositeId().KeyProperty(x => x.POID)
                .KeyProperty(x => x.OpDescr)
                .KeyProperty(x => x.OpNo)
                .KeyProperty(x => x.CaractPos);

            _ = Map(x => x.InspectionLotID).Not.Nullable();
            _ = Map(x => x.KoberLot).Not.Nullable();
            _ = Map(x => x.OpResource).Not.Nullable();
            _ = Map(x => x.OpResourceDescr).Not.Nullable();
            _ = Map(x => x.OpCtrlKey).Not.Nullable();
            _ = Map(x => x.CaractText).Not.Nullable();
            _ = Map(x => x.CaractCode).Not.Nullable();
            _ = Map(x => x.CaractType).Not.Nullable();
            _ = Map(x => x.UOM).Not.Nullable();
            _ = Map(x => x.TargetValue).Not.Nullable();
            _ = Map(x => x.UpperLimit).Not.Nullable();
            _ = Map(x => x.LowerLimit).Not.Nullable();
        }
    }
}
