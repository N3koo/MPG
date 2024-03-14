using DataEntity.Model.Output;
using DataEntity.Model.Input;

using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;

using NHibernate.Tool.hbm2ddl;
using NHibernate;
using DataEntity.Properties;

namespace DataEntity.Config {
    public class MpgDb {
        /// <summary>
        /// Used to store the session factory
        /// </summary>
        private ISessionFactory factory;

        /// <summary>
        /// Gets instance for the singleton.
        /// </summary>
        public static MpgDb Instance { get; } = new MpgDb();

        /// <summary>
        /// </summary>
        private MpgDb() {

        }

        /// <summary>
        /// Creates a new session to access the data 
        /// </summary>
        /// <returns>Reference to the new session</returns>
        public ISession GetSession() {

            if (factory == null) {
                var settings = Settings.Default;
                factory = Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql7.ConnectionString(c => c.Server(settings.MPG_Server)
                    .Database(settings.MPG_Database)
                    .Username(settings.MPG_User)
                    .Password(settings.MPG_Pass))
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
                        _ = m.FluentMappings.AddFromAssemblyOf<Classification>();
                        _ = m.FluentMappings.AddFromAssemblyOf<AlternativeName>();
                    })
                    .ExposeConfiguration(config => {
                        _ = new SchemaExport(config);
                    })
                    .BuildSessionFactory();
            }

            return factory.OpenSession();
        }

        /// <summary>
        /// Destructor that closes the session factory
        /// </summary>
        ~MpgDb() {
            factory?.Close();
        }
    }
}
