using DataEntity.Model.Output;
using DataEntity.Model.Input;

using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;

using NHibernate.Tool.hbm2ddl;
using NHibernate;

namespace DataEntity.Config {

    /// <summary>
    /// Used to access the MES database
    /// </summary>
    public class MesDb {

        /// <summary>
        /// Used to store the session factory
        /// </summary>
        private ISessionFactory _factory;

        /// <summary>
        /// Instance for the singleton
        /// </summary>
        public static MesDb Instance { get; } = new MesDb();

        /// <summary>
        /// Hide the constructor
        /// </summary>
        private MesDb() {

        }

        /// <summary>
        /// Creates a new session to access the data 
        /// </summary>
        /// <returns>Reference to the new session</returns>
        public ISession GetSession() {

            if (_factory == null) {
                _factory = Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql7.ConnectionString(c => c.Server(Properties.Resources.MES_Server)
                    .Database(Properties.Resources.MES_Database)
                    .Username(Properties.Resources.MES_User)
                    .Password(Properties.Resources.MES_Pass))
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
                        _ = new SchemaExport(config);
                    })
                    .BuildSessionFactory();
            }

            return _factory.OpenSession();
        }

        /// <summary>
        /// Destructor that closes the session factory
        /// </summary>
        ~MesDb() {
            if (_factory != null) {
                _factory.Close();
            }
        }
    }
}
