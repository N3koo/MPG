using FluentNHibernate.Mapping;
using SAPServices;

namespace DataEntity.Model.Input {

    public class ProductionOrderAlternativeName {

        public virtual string MaterialID { set; get; }
        public virtual string Language { set; get; }
        public virtual string Description { set; get; }
        public virtual string Colour { set; get; }

        public ProductionOrderAlternativeName() {

        }

        public ProductionOrderAlternativeName(ZMPGPOALTDESCR data) {
            MaterialID = data.MATERIALID;
            Language = data.LANGUAGE;
            Description = data.DESCRIPTION;
            Colour = data.COLOUR;
        }
    }

    public class ProductionOrderAlternativeNameMap : ClassMap<ProductionOrderAlternativeName> {
        public ProductionOrderAlternativeNameMap() {
            Table("MES2MPG_ProductionOrderAlternativeName");

            _ = Id(x => x.MaterialID).Not.Nullable();
            _ = Map(x => x.Language).Nullable();
            _ = Map(x => x.Description).Nullable();
            _ = Map(x => x.Colour).Nullable();
        }
    }
}
