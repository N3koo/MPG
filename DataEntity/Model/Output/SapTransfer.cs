using FluentNHibernate.Mapping;
using System;

namespace DataEntity.Model.Output {
    public class SapTransfer {
        public virtual int ID { set; get; }
        public virtual string POID { set; get; }
        public virtual DateTime CreationDate { set; get; }
        public virtual int MPGStatus { set; get; }
        public virtual DateTime MESRowUpdated { set; get; }
        public virtual int MESStatus { set; get; }
        public virtual int ConsumptionStatus { set; get; }
        public virtual string ConsumptionMessage { set; get; }
        public virtual string ConsumptionDocument { set; get; }
        public virtual int GRStatus { set; get; }
        public virtual string GRMessage { set; get; }
        public virtual string GRDocument { set; get; }

        public static SapTransfer CreateRecord(string POID) {
            return new SapTransfer {
                POID = POID,
                CreationDate = DateTime.Now,
                MPGStatus = 1
            };
        }
    }

    public class SapTransferMap : ClassMap<SapTransfer> {
        public SapTransferMap() {
            Table("MPG2MES_SAPDataTransferRequest");

            _ = Id(x => x.ID).Not.Nullable();

            _ = Map(x => x.POID).Not.Nullable();
            _ = Map(x => x.CreationDate).Not.Nullable();
            _ = Map(x => x.MPGStatus).Not.Nullable();
            _ = Map(x => x.MESRowUpdated).Nullable();
            _ = Map(x => x.ConsumptionStatus).Nullable();
            _ = Map(x => x.ConsumptionMessage).Nullable();
            _ = Map(x => x.ConsumptionDocument).Nullable();
            _ = Map(x => x.GRStatus).Nullable();
            _ = Map(x => x.GRMessage).Nullable();
            _ = Map(x => x.GRDocument).Nullable();
        }
    }
}
