﻿using FluentNHibernate.Mapping;

using System;

namespace DataEntity.Model.Input {

    public class Correction {

        public virtual int ID { set; get; }
        public virtual DateTime CreationDate { set; get; }
        public virtual string POID { set; get; }
        public virtual string MaterialID { set; get; }
        public virtual int PailNumber { set; get; }
        public virtual int CorrectionID { set; get; }
        public virtual string RawMaterialID { set; get; }
        public virtual decimal ItemQuantity { set; get; }
        public virtual string ItemUOM { set; get; }
        public virtual int MESStatus { set; get; }
        public virtual int? MPGStatus { set; get; }
        public virtual string MPGErrorMessage { set; get; }
        public virtual DateTime? MPGRowUpdated { set; get; }
    }

    public class CorrectionMap : ClassMap<Correction> {

        public CorrectionMap() {
            Table("MES2MPG_Correction");

            _ = Id(x => x.ID).Not.Nullable();

            _ = Map(x => x.CreationDate).Not.Nullable();
            _ = Map(x => x.POID).Not.Nullable();
            _ = Map(x => x.MaterialID).Not.Nullable();
            _ = Map(x => x.PailNumber).Not.Nullable();
            _ = Map(x => x.CorrectionID).Not.Nullable();
            _ = Map(x => x.RawMaterialID).Not.Nullable();
            _ = Map(x => x.ItemQuantity).Not.Nullable();
            _ = Map(x => x.ItemUOM).Not.Nullable();
            _ = Map(x => x.MESStatus).Nullable();
            _ = Map(x => x.MPGStatus).Nullable();
            _ = Map(x => x.MPGErrorMessage).Nullable();
            _ = Map(x => x.MPGRowUpdated).Nullable();
        }
    }
}
