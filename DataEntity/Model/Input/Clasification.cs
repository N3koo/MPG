using FluentNHibernate.Mapping;

using SAPServices;

using System;

namespace DataEntity.Model.Input {

    /// <summary>
    /// Saves the clasificiations for the materials
    /// </summary>
    public class Clasification {

        public virtual string MaterialID { set; get; }
        public virtual string Class { set; get; }
        public virtual string Param { set; get; }
        public virtual string ParamDescr { set; get; }
        public virtual string Value { set; get; }
        public virtual string ValueDescr { set; get; }
        public virtual int MESStatus { set; get; }
        public virtual int? MPGStatus { set; get; }
        public virtual string MPGErrorMessage { set; get; }
        public virtual DateTime? MPGRowUpdated { set; get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Clasification() {

        }

        /// <summary>
        /// Constructor for SAP data
        /// </summary>
        /// <param name="data">Data from the SAP</param>
        public Clasification(ZCLASIFICATION data) {
            MaterialID = data.MATERIALID;
            Class = data.CLASS;
            Param = data.PARAM;
            ParamDescr = data.PARAMDESCR;
            Value = data.VALUE;
            ValueDescr = data.VALUEDESCR;
        }

        /// <summary>
        /// Used to override the equals function
        /// </summary>
        /// <param name="obj">Reference to the object</param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            if (obj is not Clasification other) return false;
            if (ReferenceEquals(this, other)) return true;

            return MaterialID == other.MaterialID && Param == other.Param && Value == other.Value;
        }

        /// <summary>
        /// Used to override the function
        /// </summary>
        /// <returns>Hashcode for the needed fields</returns>
        public override int GetHashCode() {
            return HashCode.Combine(MaterialID, Param, Value);
        }

        /// <summary>
        /// Used to set details
        /// </summary>
        /// <param name="data">Element with the details</param>
        public virtual void SetDetails(ZCLASIFICATION data) {
            Class = data.CLASS;
            ParamDescr = data.PARAMDESCR;
        }

        /// <summary>
        /// Setting the details
        /// </summary>
        /// <param name="clasification"></param>
        public virtual void SetDetails(Clasification clasification) {
            Class = clasification.Class;
            ParamDescr = clasification.ParamDescr;
        }
    }

    /// <summary>
    /// Mapping the clasification class
    /// </summary>
    public class ClasificationMap : ClassMap<Clasification> {

        /// <summary>
        /// Default constructor
        /// </summary>
        public ClasificationMap() {
            Table("MES2MPG_MaterialDataClasification");

            _ = CompositeId().KeyProperty(x => x.MaterialID).KeyProperty(x => x.Param).KeyProperty(x => x.Value);

            _ = Map(x => x.Class).Nullable();
            _ = Map(x => x.ParamDescr).Nullable();
            _ = Map(x => x.ValueDescr).Nullable();
            _ = Map(x => x.MESStatus).Nullable();
            _ = Map(x => x.MPGStatus).Nullable();
            _ = Map(x => x.MPGErrorMessage).Nullable();
            _ = Map(x => x.MPGRowUpdated).Nullable();
        }
    }
}
