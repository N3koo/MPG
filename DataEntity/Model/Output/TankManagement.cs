using FluentNHibernate.Mapping;
using NHibernate.Linq.Functions;
using System;
using System.Security.Permissions;

namespace DataEntity.Model.Output {
    public class TankManagement {
        public virtual string Tank_Name { set; get; }

        public virtual string Tank_Mat { set; get; }

        public virtual int Tank_UOM { set; get; }

        public virtual int Tank_Const { set; get; }

        public virtual decimal Tank_Dens { set; get; }

        public virtual decimal Tank_Value { set; get; }

        public virtual decimal Tank_Capacity {  set; get; }

        public virtual decimal Tank_Max_Value { set; get; }

        public virtual decimal Tank_Min_Value { set; get; }

        public virtual int Tank_Status { set; get; }

        public virtual decimal Tank_Temp { set; get; }

        public virtual int Reserved_1 { set; get; }

        public virtual int Reserved_2 { set; get; }

        public virtual DateTime Row_Updated { set; get; }

    }

    public class TankManagementMap : ClassMap<TankManagement> { 
    
        public TankManagementMap() {
            Table("MPG2MES_TankManagement");

            _ = Id(x => x.Tank_Name).Not.Nullable();
            _ = Map(x => x.Tank_Mat).Not.Nullable();
            _ = Map(x => x.Tank_UOM).Not.Nullable();
            _ = Map(x => x.Tank_Const).Not.Nullable();
            _ = Map(x => x.Tank_Dens).Not.Nullable();
            _ = Map(x => x.Tank_Value).Not.Nullable();
            _ = Map(x => x.Tank_Capacity).Not.Nullable();
            _ = Map(x => x.Tank_Max_Value).Not.Nullable();
            _ = Map(x => x.Tank_Min_Value).Not.Nullable();
            _ = Map(x => x.Tank_Status).Not.Nullable();
            _ = Map(x => x.Tank_Temp).Not.Nullable();
            _ = Map(x => x.Reserved_1).Nullable();
            _ = Map(x => x.Reserved_2).Nullable();
            _ = Map(x => x.Row_Updated).Not.Nullable();
        }
    }
}
