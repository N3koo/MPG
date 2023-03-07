using FluentNHibernate.Mapping;

using SAPServices;

using System;

namespace DataEntity.Model.Input {

    /// <summary>
    /// Used to store data about the materials
    /// </summary>
    public class MaterialData {

        public virtual int ID { set; get; }
        public virtual string MaterialID { set; get; }
        public virtual string Description { set; get; }
        public virtual decimal GrossWeight { set; get; }
        public virtual decimal NetWeight { set; get; }
        public virtual string WeightUnit { set; get; }
        public virtual string Volume { set; get; }
        public virtual string VolumeUnit { set; get; }
        public virtual string ShelfLife { set; get; }
        public virtual string Plant { set; get; }
        public virtual string BaseUOM { set; get; }
        public virtual string Type { set; get; }
        public virtual string TypeDescr { set; get; }
        public virtual string MatGroup { set; get; }
        public virtual string MatGroupDescr { set; get; }
        public virtual string EAN { set; get; }
        public virtual string Standard { set; get; }
        public virtual string BaseMaterial { set; get; }
        public virtual string MatSeries { set; get; }
        public virtual string Status { set; get; }
        public virtual string PropDep { set; get; }
        public virtual string WareLoc { set; get; }
        public virtual string ProfitCenter { set; get; }
        public virtual int MESStatus { set; get; }
        public virtual int? MPGStatus { set; get; }
        public virtual string MPGErrorMessage { set; get; }
        public virtual DateTime? MPGRowUpdated { set; get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MaterialData() {

        }

        /// <summary>
        /// Constructor with data from SAP
        /// </summary>
        /// <param name="data">Data from SAP</param>
        public MaterialData(ZMATERIALDATA data) {
            MaterialID = data.MATERIALID;
            Description = data.DESCRIPTION;
            GrossWeight = data.GROSSWEIGHT;
            NetWeight = data.NETWEIGHT;
            WeightUnit = data.WEIGHTUNIT;
            Volume = data.VOLUME.ToString();
            VolumeUnit = data.VOLUMEUNIT;
            ShelfLife = data.SHELFLIFE;
            Plant = data.PLANT;
            BaseUOM = data.BASEUOM;
            Type = data.TYPE;
            TypeDescr = data.TYPEDESCR;
            MatGroup = data.MATGROUP;
            MatGroupDescr = data.MATGROUPDESCR;
            EAN = data.EAN;
            Standard = data.STANDARD;
            BaseMaterial = data.BASEMATERIAL;
            MatSeries = data.MATSERIES;
            Status = data.STATUS;
            PropDep = data.PRODDEP;
            WareLoc = data.WARELOC;
            ProfitCenter = data.PROFITCENTER;
        }

        /// <summary>
        /// Setting the details from SAP
        /// </summary>
        /// <param name="data">Data from SAP</param>
        public virtual void SetDetails(ZMATERIALDATA data) {
            Description = data.DESCRIPTION;
            GrossWeight = data.GROSSWEIGHT;
            NetWeight = data.NETWEIGHT;
            WeightUnit = data.WEIGHTUNIT;
            Volume = data.VOLUME.ToString();
            VolumeUnit = data.VOLUMEUNIT;
            ShelfLife = data.SHELFLIFE;
            Plant = data.PLANT;
            BaseUOM = data.BASEUOM;
            Type = data.TYPE;
            TypeDescr = data.TYPEDESCR;
            MatGroup = data.MATGROUP;
            MatGroupDescr = data.MATGROUPDESCR;
            MatSeries = data.MATSERIES;
            EAN = data.EAN;
            Standard = data.STANDARD;
            BaseMaterial = data.BASEMATERIAL;
            Status = data.STATUS;
            PropDep = data.PRODDEP;
            WareLoc = data.WARELOC;
            ProfitCenter = data.PROFITCENTER;
            MPGRowUpdated = DateTime.Now;
        }

        public virtual void SetDetails(MaterialData data) {
            Description = data.Description;
            GrossWeight = data.GrossWeight;
            NetWeight = data.NetWeight;
            WeightUnit = data.WeightUnit;
            Volume = data.Volume.ToString();
            VolumeUnit = data.VolumeUnit;
            ShelfLife = data.ShelfLife;
            Plant = data.Plant;
            BaseUOM = data.BaseUOM;
            Type = data.Type;
            TypeDescr = data.TypeDescr;
            MatGroup = data.MatGroup;
            MatGroupDescr = data.MatGroupDescr;
            MatSeries = data.MatSeries;
            EAN = data.EAN;
            Standard = data.Standard;
            BaseMaterial = data.BaseMaterial;
            Status = data.Status;
            PropDep = data.PropDep;
            WareLoc = data.WareLoc;
            ProfitCenter = data.ProfitCenter;
            MPGRowUpdated = DateTime.Now;
        }
    }

    /// <summary>
    /// Used for mapping the material data
    /// </summary>
    public class MaterialDataMap : ClassMap<MaterialData> {

        /// <summary>
        /// Default constructor
        /// </summary>
        public MaterialDataMap() {
            Table("MES2MPG_MaterialData");

            _ = Id(x => x.ID).Not.Nullable();
            _ = Map(x => x.MaterialID).Not.Nullable();
            _ = Map(x => x.Description).Nullable();
            _ = Map(x => x.GrossWeight).Nullable();
            _ = Map(x => x.NetWeight).Nullable();
            _ = Map(x => x.WeightUnit).Nullable();
            _ = Map(x => x.Volume).Nullable();
            _ = Map(x => x.VolumeUnit).Nullable();
            _ = Map(x => x.ShelfLife).Nullable();
            _ = Map(x => x.Plant).Nullable();
            _ = Map(x => x.BaseUOM).Nullable();
            _ = Map(x => x.Type).Nullable();
            _ = Map(x => x.TypeDescr).Nullable();
            _ = Map(x => x.MatGroup).Nullable();
            _ = Map(x => x.MatGroupDescr).Nullable();
            _ = Map(x => x.MatSeries).Nullable();
            _ = Map(x => x.EAN).Nullable();
            _ = Map(x => x.Standard).Nullable();
            _ = Map(x => x.BaseMaterial).Nullable();
            _ = Map(x => x.Status).Nullable();
            _ = Map(x => x.PropDep).Nullable();
            _ = Map(x => x.WareLoc).Nullable();
            _ = Map(x => x.ProfitCenter).Nullable();
            _ = Map(x => x.MESStatus).Nullable();
            _ = Map(x => x.MPGStatus).Nullable();
            _ = Map(x => x.MPGErrorMessage).Nullable();
            _ = Map(x => x.MPGRowUpdated).Nullable();
        }
    }
}
