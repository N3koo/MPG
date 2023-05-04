using FluentNHibernate.Mapping;

using SAPServices;
using System;

namespace DataEntity.Model.Input {

    /// <summary>
    /// Class for materials alternative names
    /// </summary>
    public class MaterialDataUOMS {

        public virtual Int64 ID { get; set; }
        public virtual string MaterialID { get; set; }
        public virtual string UOM { get; set; }
        public virtual string EAN { get; set; }
        public virtual decimal? NUMERATOR { get; set; }
        public virtual decimal? DENOMINATOR { get; set; }
        public virtual string CATEGORY { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MaterialDataUOMS() {

        }

        /// <summary>
        /// Constructor for SAP data
        /// </summary>
        /// <param name="data"></param>
        public MaterialDataUOMS(ZMPGPOALTUOMS data) {
            MaterialID = data.MATERIALID;
            UOM = data.UOM;
            EAN = data.EAN;
            NUMERATOR = data.NUMERATOR;
            DENOMINATOR = data.DENOMINATOR;
            CATEGORY = data.CATEGORY;
        }
    }

    /// <summary>
    /// Mapping the material data alternative names
    /// </summary>
    public class MaterialAlternativeUaomsMap : ClassMap<MaterialDataUOMS> {

        /// <summary>
        /// Default constructor
        /// </summary>
        public MaterialAlternativeUaomsMap() {
            Table("MES2MPG_MaterialDataAlternativeUOMS");

            _ = Id(x => x.ID).Not.Nullable();
            _ = Map(x => x.MaterialID).Not.Nullable();
            _ = Map(x => x.UOM).Not.Nullable();
            _ = Map(x => x.EAN).Nullable();
            _ = Map(x => x.NUMERATOR).Nullable();
            _ = Map(x => x.DENOMINATOR).Nullable();
            _ = Map(x => x.CATEGORY).Nullable();
        }
    }
}
