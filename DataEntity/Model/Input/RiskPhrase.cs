using FluentNHibernate.Mapping;

using SAPServices;

namespace DataEntity.Model.Input {

    public class RiskPhrase {
        public virtual string Material { set; get; }
        public virtual string Instr { set; get; }
        public virtual string Risk_Fr { set; get; }
        public virtual string Language { set; get; }
        public virtual string GHS01 { set; get; }
        public virtual string GHS02 { set; get; }
        public virtual string GHS03 { set; get; }
        public virtual string GHS05 { set; get; }
        public virtual string GHS06 { set; get; }
        public virtual string GHS07 { set; get; }
        public virtual string GHS08 { set; get; }
        public virtual string GHS09 { set; get; }
        public virtual string UN1263 { set; get; }

        public virtual void SetPics(ZMESMPGPICTOGRAME item) {
            GHS01 = item.GHS01;
            GHS02 = item.GHS02;
            GHS03 = item.GHS03;
            GHS05 = item.GHS05;
            GHS06 = item.GHS06;
            GHS07 = item.GHS07;
            GHS08 = item.GHS08;
            GHS09 = item.GHS09;
            UN1263 = item.UN1263;
        }

        public virtual void Update(RiskPhrase item) {
            Language = item.Language;
            Risk_Fr = item.Risk_Fr;
            Instr = item.Instr;
            GHS01 = item.GHS01;
            GHS02 = item.GHS02;
            GHS03 = item.GHS03;
            GHS05 = item.GHS05;
            GHS06 = item.GHS06;
            GHS07 = item.GHS07;
            GHS08 = item.GHS08;
            GHS09 = item.GHS09;
            UN1263 = item.UN1263;
        }
    }

    public class RiskPraseMap : ClassMap<RiskPhrase> {
        public RiskPraseMap() {
            Table("MES2MPG_RiskPhrases");

            _ = Id(x => x.Material);
            _ = Map(x => x.Instr).Nullable();
            _ = Map(x => x.Risk_Fr).Nullable();
            _ = Map(x => x.Language).Nullable();
            _ = Map(x => x.GHS01).Nullable();
            _ = Map(x => x.GHS02).Nullable();
            _ = Map(x => x.GHS03).Nullable();
            _ = Map(x => x.GHS05).Nullable();
            _ = Map(x => x.GHS06).Nullable();
            _ = Map(x => x.GHS07).Nullable();
            _ = Map(x => x.GHS08).Nullable();
            _ = Map(x => x.GHS09).Nullable();
            _ = Map(x => x.UN1263).Nullable();
        }
    }
}
