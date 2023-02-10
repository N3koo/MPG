using FluentNHibernate.Mapping;
using SAPServices;

namespace DataEntity.Model.Input {

    public class ProductionOrderTechDetail {
        public virtual string POID { set; get; }
        public virtual string OPNO { set; get; }
        public virtual string SUPOP { set; get; }
        public virtual decimal Basic_Amount { set; get; }
        public virtual string Ba_UOM { set; get; }
        public virtual string CTRL_KEY { set; get; }
        public virtual string Standard_Text_Key { set; get; }
        public virtual string OP_DESCR { set; get; }
        public virtual string Resources { set; get; }
        public virtual string ResourceDescr { set; get; }
        public virtual decimal Prep_Time { set; get; }
        public virtual string Activity_Type1 { set; get; }
        public virtual decimal Machine_Time { set; get; }
        public virtual string Activity_Type2 { set; get; }
        public virtual decimal Working_Time { set; get; }
        public virtual string Activity_Type3 { set; get; }
        public virtual string ACT_UOM { set; get; }

        public ProductionOrderTechDetail() {

        }

        public ProductionOrderTechDetail(ZMPGPOTECHDET data) {
            POID = data.POID;
            OPNO = data.OPNO;
            SUPOP = data.SUP_OP;
            Basic_Amount = data.BASIC_AMOUNT;
            Ba_UOM = data.BA_UOM;
            CTRL_KEY = data.CTRL_KEY;
            Standard_Text_Key = data.STANDARD_TEXT_KEY;
            OP_DESCR = data.OP_DESCR;
            Resources = data.RESOURCES;
            ResourceDescr = data.RESOURCEDESCR;
            Prep_Time = data.PREP_TIME;
            Activity_Type1 = data.ACTIVITY_TYPE1;
            Machine_Time = data.MACHINE_TIME;
            Activity_Type2 = data.ACTIVITY_TYPE2;
            Working_Time = data.WORKING_TIME;
            Activity_Type3 = data.ACTIVITY_TYPE3;
            ACT_UOM = data.ACT_UOM;
        }
    }

    public class ProductionOrderTechDetailMap : ClassMap<ProductionOrderTechDetail> {

        public ProductionOrderTechDetailMap() {
            Table("MES2MPG_ProductionOrderTechDetails");

            _ = Id(x => x.POID).Not.Nullable();
            _ = Map(x => x.OPNO).Not.Nullable();
            _ = Map(x => x.SUPOP).Not.Nullable();
            _ = Map(x => x.Basic_Amount).Not.Nullable();
            _ = Map(x => x.Ba_UOM).Not.Nullable();
            _ = Map(x => x.CTRL_KEY).Not.Nullable();
            _ = Map(x => x.Standard_Text_Key).Not.Nullable();
            _ = Map(x => x.OP_DESCR).Not.Nullable();
            _ = Map(x => x.Resources).Not.Nullable();
            _ = Map(x => x.ResourceDescr).Not.Nullable();
            _ = Map(x => x.Prep_Time).Not.Nullable();
            _ = Map(x => x.Activity_Type1).Not.Nullable();
            _ = Map(x => x.Machine_Time).Not.Nullable();
            _ = Map(x => x.Activity_Type2).Not.Nullable();
            _ = Map(x => x.Working_Time).Not.Nullable();
            _ = Map(x => x.Activity_Type3).Not.Nullable();
            _ = Map(x => x.ACT_UOM).Not.Nullable();
        }
    }
}
