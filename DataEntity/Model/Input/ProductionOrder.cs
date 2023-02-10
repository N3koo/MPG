using FluentNHibernate.Mapping;

using SAPServices;

using System;
using System.Data.SqlTypes;

namespace DataEntity.Model.Input {

    public class ProductionOrder {
        public virtual string POID { set; get; }
        public virtual string POType { set; get; }
        public virtual string PODescription { set; get; }
        public virtual string MaterialID { set; get; }
        public virtual string PlantID { set; get; }
        public virtual decimal PlannedQtyBUC { set; get; }
        public virtual string PlannedQtyBUCUom { set; get; }
        public virtual DateTime PlannedStartDate { set; get; }
        public virtual string PlannedStartHour { set; get; }
        public virtual DateTime PlannedEndDate { set; get; }
        public virtual string PlannedEndHour { set; get; }
        public virtual string InspectionLotId { set; get; }
        public virtual string ProfitCenter { set; get; }
        public virtual string StorageLoc { set; get; }
        public virtual string RezervationNumber { set; get; }
        public virtual string KoberLot { set; get; }
        public virtual string BOMId { set; get; }
        public virtual string BOMAlternative { set; get; }
        public virtual string Priority { set; get; }
        public virtual string Status { set; get; }
        public virtual string TechCode { set; get; }
        public virtual string TechVersion { set; get; }
        public virtual int MESStatus { set; get; }
        public virtual int MPGStatus { set; get; }
        public virtual string MPGErrorMessage { set; get; }
        public virtual DateTime MPGRowUpdated { set; get; }

        public ProductionOrder() {

        }

        public ProductionOrder(ProductionOrder order) {
            POID = order.POID;
            POType = order.POType;
            PODescription = order.PODescription;
            MaterialID = order.MaterialID;
            PlantID = order.PlantID;
            PlannedQtyBUC = order.PlannedQtyBUC;
            PlannedQtyBUCUom = order.PlannedQtyBUCUom;
            PlannedStartDate = order.PlannedStartDate;
            PlannedStartHour = order.PlannedStartHour;
            PlannedEndDate = order.PlannedEndDate;
            PlannedEndHour = order.PlannedEndHour;
            InspectionLotId = order.InspectionLotId;
            ProfitCenter = order.ProfitCenter;
            StorageLoc = order.StorageLoc;
            KoberLot = order.KoberLot;
            RezervationNumber = order.RezervationNumber;
            BOMId = order.BOMId;
            BOMAlternative = order.BOMAlternative;
            Priority = order.Priority;
            Status = order.Status;
            TechCode = order.TechCode;
            TechVersion = order.TechVersion;
        }

        public ProductionOrder(ZMPGPO data) {
            POID = data.POID;
            POType = data.POTYPE;
            PODescription = data.PODESCRIPTION;
            MaterialID = data.MATERIALID;
            PlantID = data.PLANTID;
            PlannedQtyBUC = data.PLANNEDQTYBUC;
            PlannedQtyBUCUom = data.PLANNEDQTYBUCUOM;
            PlannedStartDate = DateTime.Parse(data.PLANNEDSTARTDATE);
            PlannedStartHour = data.PLANNEDSTARTHOUR;
            PlannedEndDate = DateTime.Parse(data.PLANNEDENDDATE);
            PlannedEndHour = data.PLANNEDENDHOUR;
            InspectionLotId = data.INSPECTIONLOTID;
            ProfitCenter = data.PROFITCENTER;
            StorageLoc = data.STORAGELOC;
            RezervationNumber = data.RESERVATIONNUMBER;
            KoberLot = data.KOBERLOT;
            BOMId = data.BOMID;
            BOMAlternative = data.BOMALTERNATIVE;
            Priority = data.PRIORITY;
            Status = data.STATUS;
            TechCode = data.TECH_CODE;
            TechVersion = data.TECH_VERSION;
        }
    }

    public class ProductionOrderMap : ClassMap<ProductionOrder> {

        public ProductionOrderMap() {
            Table("MES2MPG_ProductionOrders");

            _ = Id(x => x.POID).Not.Nullable();

            _ = Map(x => x.POType).Not.Nullable();
            _ = Map(x => x.PODescription).Not.Nullable();
            _ = Map(x => x.MaterialID).Not.Nullable();
            _ = Map(x => x.PlantID).Not.Nullable();
            _ = Map(x => x.PlannedQtyBUC).Not.Nullable();
            _ = Map(x => x.PlannedQtyBUCUom).Not.Nullable();
            _ = Map(x => x.PlannedStartDate).Not.Nullable();
            _ = Map(x => x.PlannedStartHour).Not.Nullable();
            _ = Map(x => x.PlannedEndDate).Not.Nullable();
            _ = Map(x => x.PlannedEndHour).Not.Nullable();
            _ = Map(x => x.InspectionLotId).Not.Nullable();
            _ = Map(x => x.ProfitCenter).Not.Nullable();
            _ = Map(x => x.RezervationNumber).Not.Nullable();
            _ = Map(x => x.StorageLoc).Not.Nullable();
            _ = Map(x => x.KoberLot).Not.Nullable();
            _ = Map(x => x.BOMId).Not.Nullable();
            _ = Map(x => x.BOMAlternative).Not.Nullable();
            _ = Map(x => x.Priority).Not.Nullable();
            _ = Map(x => x.Status).Not.Nullable();
            _ = Map(x => x.TechCode).Not.Nullable();
            _ = Map(x => x.TechVersion).Not.Nullable();
            _ = Map(x => x.MESStatus).Nullable();
            _ = Map(x => x.MPGStatus).Nullable();
            _ = Map(x => x.MPGErrorMessage).Nullable();
            _ = Map(x => x.MPGRowUpdated).Nullable();
        }
    }
}
