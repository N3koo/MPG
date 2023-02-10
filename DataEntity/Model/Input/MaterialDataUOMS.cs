using FluentNHibernate.Mapping;

using SAPServices;

namespace DataEntity.Model.Input {

    public class MaterialDataUOMS {
        public virtual int ID { get; set; }
        public virtual string MaterialID { get; set; }
        public virtual string UOM { get; set; }
        public virtual string EAN { get; set; }
        public virtual decimal NUMERATOR { get; set; }
        public virtual decimal DENOMINATOR { get; set; }
        public virtual string CATEGORY { get; set; }

        public MaterialDataUOMS() {

        }

        public MaterialDataUOMS(ZMPGPOALTUOMS data) {
            MaterialID = data.MATERIALID;
            UOM = data.UOM;
            EAN = data.EAN;
            NUMERATOR = data.NUMERATOR;
            DENOMINATOR = data.DENOMINATOR;
            CATEGORY = data.CATEGORY;
        }
    }

    public class MaterialAlternativeUaomsMap : ClassMap<MaterialDataUOMS> {
        public MaterialAlternativeUaomsMap() {
            Table("MES2MPG_MaterialDataAlternativeUoms");

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
