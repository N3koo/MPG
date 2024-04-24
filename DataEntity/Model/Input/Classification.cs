using FluentNHibernate.Mapping;

using SAPServices;

using System;

namespace DataEntity.Model.Input {

    /// <summary>
    /// Saves the classifications for the materials
    /// </summary>
    public class Classification {

        public virtual string MaterialID { set; get; }
        public virtual string Class { set; get; }
        public virtual string Param { set; get; }
        public virtual string ParamDescr { set; get; }
        public virtual string Value { set; get; }
        public virtual string ValueDescr { set; get; }


        /// <summary>
        /// Default constructor
        /// </summary>
        public Classification() {

        }

        /// <summary>
        /// Constructor for SAP data
        /// </summary>
        /// <param name="data">Data from the SAP</param>
        public Classification(ZCLASIFICATION data) {
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
            if (obj is not Classification other) return false;
            if (ReferenceEquals(this, other)) return true;

            return MaterialID == other.MaterialID && Param == other.Param && Value == other.Value;
        }

        /// <summary>
        /// Used to override the function
        /// </summary>
        /// <returns>Hash code for the needed fields</returns>
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
        public virtual void SetDetails(Classification clasification) {
            Class = clasification.Class;
            ParamDescr = clasification.ParamDescr;
        }
    }

    /// <summary>
    /// Mapping the classification class
    /// </summary>
    public class ClassificationMap : ClassMap<Classification> {

        /// <summary>
        /// Default constructor
        /// </summary>
        public ClassificationMap() {
            Table("MES2MPG_MaterialDataClassifications");

            _ = CompositeId().KeyProperty(x => x.MaterialID).KeyProperty(x => x.Param).KeyProperty(x => x.Value);

            _ = Map(x => x.Class).Nullable();
            _ = Map(x => x.ParamDescr).Nullable();
            _ = Map(x => x.ValueDescr).Nullable();
        }
    }
}
