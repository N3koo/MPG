using FluentNHibernate.Mapping;

using SAPServices;

using System;

namespace DataEntity.Model.Input {

    public class StockVessel {
        public virtual string MaterialID { set; get; }
        public virtual string Plant { set; get; }
        public virtual string MpgCode { set; get; }
        public virtual string MpgHead { set; get; }
        public virtual string StorageLoc { set; get; }
        public virtual string VesselCod { set; get; }
        public virtual string VesselDesc { set; get; }
        public virtual decimal Capacity { set; get; }
        public virtual string Dimensions { set; get; }
        public virtual string MaterialDesc { set; get; }
        public virtual decimal MaterialDensity { set; get; }
        public virtual decimal ItemQty { set; get; }
        public virtual string ItemQtyUOM { set; get; }
        public virtual decimal MPGOffset { set; get; }
        public virtual decimal Trcoef { set; get; }
        public virtual int MESStatus { set; get; }
        public virtual int? MPGStatus { set; get; }
        public virtual DateTime? MPGRowUpdated { set; get; }
        public virtual string CanisterType { set; get; }
        public virtual int TotalVolume { set; get; }
        public virtual int UsefulVolume { set; get; }
        public virtual decimal MaxQuantity { set; get; }
        public virtual decimal RecomQuantity { set; get; }
        public virtual decimal PasteConc { set; get; }
        public virtual decimal ThermalDev { set; get; }
        public virtual int NrDropSteps { set; get; }
        public virtual decimal Reserve1 { set; get; }
        public virtual decimal Reserve2 { set; get; }

        public StockVessel() {

        }

        public StockVessel(ZCORESPMPG data) {
            Plant = data.PLANT;
            MpgCode = data.MPGCODE;
            MpgHead = data.MPGHEAD;
            StorageLoc = data.STORAGELOC;
            VesselCod = data.VESSELCOD;
            VesselDesc = data.VESSELDESCR;
            Capacity = data.CAPACITY;
            Dimensions = data.DIMENSIONS;
            MaterialID = data.MATERIALID;
            MaterialDesc = data.MATDESCR;
            ItemQty = data.MATERIALQTY;
            ItemQtyUOM = data.MATERIALQTYUOM;
        }

        public virtual void SetDetails(ZCORESPMPG data) {
            Plant = data.PLANT;
            MpgCode = data.MPGCODE;
            MpgHead = data.MPGHEAD;
            StorageLoc = data.STORAGELOC;
            VesselCod = data.VESSELCOD;
            VesselDesc = data.VESSELDESCR;
            Capacity = data.CAPACITY;
            Dimensions = data.DIMENSIONS;
            MaterialID = data.MATERIALID;
            MaterialDesc = data.MATDESCR;
            ItemQty = data.MATERIALQTY;
            ItemQtyUOM = data.MATERIALQTYUOM;
        }

        public virtual void SetDetails(StockVessel vessel) {
            Plant = vessel.Plant;
            MpgCode = vessel.MpgCode;
            MpgHead = vessel.MpgHead;
            StorageLoc = vessel.StorageLoc;
            VesselCod = vessel.VesselCod;
            VesselDesc = vessel.VesselDesc;
            Capacity = vessel.Capacity;
            Dimensions = vessel.Dimensions;
            MaterialDesc = vessel.MaterialDesc;
            ItemQty = vessel.ItemQty;
            ItemQtyUOM = vessel.ItemQtyUOM;
            MaterialDensity = vessel.MaterialDensity;
            Trcoef = vessel.Trcoef;
            MPGOffset = vessel.MPGOffset;
        }
    }

    public class StockVesselMap : ClassMap<StockVessel> {
        public StockVesselMap() {
            Table("MES2MPG_StockVessel");

            _ = Id(x => x.MaterialID).Not.Nullable();
            _ = Map(x => x.Plant).Not.Nullable();
            _ = Map(x => x.MpgCode).Not.Nullable();
            _ = Map(x => x.MpgHead).Not.Nullable();
            _ = Map(x => x.StorageLoc).Not.Nullable();
            _ = Map(x => x.VesselCod).Not.Nullable();
            _ = Map(x => x.VesselDesc).Not.Nullable();
            _ = Map(x => x.Capacity).Not.Nullable();
            _ = Map(x => x.Dimensions).Not.Nullable();
            _ = Map(x => x.MaterialDesc).Not.Nullable();
            _ = Map(x => x.MaterialDensity).Nullable();
            _ = Map(x => x.ItemQty).Not.Nullable();
            _ = Map(x => x.ItemQtyUOM).Not.Nullable();
            _ = Map(x => x.MPGOffset).Nullable();
            _ = Map(x => x.Trcoef).Nullable();
            _ = Map(x => x.MESStatus).Nullable();
            _ = Map(x => x.MPGStatus).Nullable();
            _ = Map(x => x.MPGRowUpdated).Nullable();
            _ = Map(x => x.CanisterType).Nullable();
            _ = Map(x => x.TotalVolume).Nullable();
            _ = Map(x => x.UsefulVolume).Nullable();
            _ = Map(x => x.MaxQuantity).Nullable();
            _ = Map(x => x.RecomQuantity).Nullable();
            _ = Map(x => x.PasteConc).Nullable();
            _ = Map(x => x.ThermalDev).Nullable();
            _ = Map(x => x.NrDropSteps).Nullable();
            _ = Map(x => x.Reserve1).Nullable();
            _ = Map(x => x.Reserve2).Nullable();
        }
    }
}
