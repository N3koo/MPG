using DataEntity.Model.Input;
using DataEntity.Model.Output;

using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;

using NHibernate.Tool.hbm2ddl;
using NHibernate;
using NHibernate.Cfg;

namespace DataEntity.Config {
    public class MpgDb {
        /// <summary>
        /// Used to store the session factory
        /// </summary>
        private ISessionFactory _factory;

        /// <summary>
        /// Instance for the singleton
        /// </summary>
        public static MpgDb Instance { get; } = new MpgDb();

        /// <summary>
        /// Hide the constructor
        /// </summary>
        private MpgDb() {

        }

        /// <summary>
        /// Creates a new session to access the data 
        /// </summary>
        /// <returns>Reference to the new session</returns>
        public ISession GetSession() {

            if (_factory == null) {
                _factory = Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql7.ConnectionString(c => c.Server(Properties.Resources.MPG_Server)
                    .Database(Properties.Resources.MPG_Database)
                    .Username(Properties.Resources.MPG_User)
                    .Password(Properties.Resources.MPG_Pass))
                    .ShowSql())
                    .Mappings(m => {
                        _ = m.FluentMappings.AddFromAssemblyOf<MaterialDataUOMS>();
                        _ = m.FluentMappings.AddFromAssemblyOf<MaterialData>();
                        _ = m.FluentMappings.AddFromAssemblyOf<ProductionOrderFinalItem>();
                        _ = m.FluentMappings.AddFromAssemblyOf<ProductionOrderLotDetail>();
                        _ = m.FluentMappings.AddFromAssemblyOf<ProductionOrderLotHeader>();
                        _ = m.FluentMappings.AddFromAssemblyOf<ProductionOrder>();
                        _ = m.FluentMappings.AddFromAssemblyOf<ProductionOrderAlternativeName>();
                        _ = m.FluentMappings.AddFromAssemblyOf<ProductionOrderTechDetail>();
                        _ = m.FluentMappings.AddFromAssemblyOf<ProductionOrderConsumption>();
                        _ = m.FluentMappings.AddFromAssemblyOf<ProductionOrderCorection>();
                        _ = m.FluentMappings.AddFromAssemblyOf<ProductionOrderGoodsReceipt>();
                        _ = m.FluentMappings.AddFromAssemblyOf<ProductionOrderPailStatus>();
                        _ = m.FluentMappings.AddFromAssemblyOf<ProductOrderQualityCheck>();
                        _ = m.FluentMappings.AddFromAssemblyOf<ProductionOrderStatus>();
                        _ = m.FluentMappings.AddFromAssemblyOf<ProductionOrderBom>();
                        _ = m.FluentMappings.AddFromAssemblyOf<StockVessel>();
                        _ = m.FluentMappings.AddFromAssemblyOf<Clasification>();
                        _ = m.FluentMappings.AddFromAssemblyOf<AlternativeName>();
                    })
                    .ExposeConfiguration(config => {
                        new SchemaExport(config).Execute(true, true, false);
                    })
                    .BuildSessionFactory();

            }

            return _factory.OpenSession();
        }

        /// <summary>
        /// Destructor that closes the session factory
        /// </summary>
        ~MpgDb() {
            if (_factory != null) {
                _factory.Close();
            }
        }
    }
}
