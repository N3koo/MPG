using FluentNHibernate.Mapping;

using SAPServices;

namespace DataEntity.Model.Input {

    public class ProductionOrderLotHeader {
        public virtual string POID { set; get; }
        public virtual string InspectionLotID { set; get; }
        public virtual string MaterialID { set; get; }
        public virtual string KoberLot { set; get; }
        public virtual string SourceLot { set; get; }
        public virtual string ControlType { set; get; }
        public virtual string StartDate { set; get; }
        public virtual string FinishDate { set; get; }
        public virtual string FabrInstrGroup { set; get; }
        public virtual string FabrInstr { set; get; }
        public virtual string PozQC { set; get; }

        public ProductionOrderLotHeader() {

        }

        public ProductionOrderLotHeader(ZMPGPOLOTHEADER data) {
            POID = data.POID;
            InspectionLotID = data.INSPECTIONLOTID;
            MaterialID = data.MATERIALID;
            KoberLot = data.KOBERLOT;
            SourceLot = data.SOURCELOT;
            ControlType = data.CONTROLTYPE;
            StartDate = data.STARTDATE;
            FinishDate = data.FINISHDATE;
            FabrInstrGroup = data.FABRINSTRGROUP;
            FabrInstr = data.FABRINSTR;
            PozQC = data.POZQC;
        }
    }

    public class ProductionOrderLotHeaderMap : ClassMap<ProductionOrderLotHeader> {

        public ProductionOrderLotHeaderMap() {
            Table("MES2MPG_ProductionOrderLotHeader");

            _ = Id(x => x.POID).Not.Nullable();
            _ = Map(x => x.InspectionLotID).Not.Nullable();
            _ = Map(x => x.MaterialID).Not.Nullable();
            _ = Map(x => x.KoberLot).Not.Nullable();
            _ = Map(x => x.SourceLot).Not.Nullable();
            _ = Map(x => x.ControlType).Not.Nullable();
            _ = Map(x => x.StartDate).Not.Nullable();
            _ = Map(x => x.FinishDate).Not.Nullable();
            _ = Map(x => x.FabrInstr).Not.Nullable();
            _ = Map(x => x.FabrInstrGroup).Not.Nullable();
            _ = Map(x => x.PozQC).Nullable();
        }
    }
}
