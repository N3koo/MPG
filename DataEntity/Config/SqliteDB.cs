using DataEntity.Model.Output;
using DataEntity.Model.Input;

using NHibernate.Tool.hbm2ddl;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Cfg;
using NHibernate;

using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;

using System.Globalization;
using System.IO;

namespace DataEntity.Config {

    /// <summary>
    /// Used to export data to a SQLite database
    /// </summary>
    public class SqliteDB {

        /// <summary>
        /// Factory session
        /// </summary>
        private ISessionFactory _factory;

        /// <summary>
        /// Driver path of database
        /// </summary>
        private string _driverPath = "Data Source={0};Version=3";

        /// <summary>
        /// Path of the file
        /// </summary>
        private string _path;

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static SqliteDB Instance { get; } = new SqliteDB();

        /// <summary>
        /// Hide constructor
        /// </summary>
        private SqliteDB() {

        }

        /// <summary>
        /// Used to create a factory to create sessions
        /// </summary>
        private void CreateFactory() {
            _driverPath = string.Format(CultureInfo.InvariantCulture, _driverPath, Properties.Resources.Path_Sqlite);
            _path = string.Format(CultureInfo.InvariantCulture, "{0}", Properties.Resources.Path_Sqlite);

            _factory = Fluently.Configure()
                .Database(SQLiteConfiguration.Standard
                .Driver<SQLite20Driver>()
                .Dialect<SQLiteDialect>()
                .UsingFile(_path).ShowSql())
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
                }).ExposeConfiguration(BuildSchema)
                .BuildSessionFactory();
        }

        /// <summary>
        /// Creates a new session
        /// </summary>
        /// <returns>Reference to the new session</returns>
        public ISession GetSession() {
            if (_factory == null) {
                CreateFactory();
            }

            return _factory.OpenSession();
        }

        /// <summary>
        /// Sets the details for creating the new SQLite file database
        /// </summary>
        /// <param name="obj">Reference to the default configuration object</param>
        private void BuildSchema(Configuration obj) {
            bool reset = !File.Exists(_path);
            if (reset) {
                File.Create(_path).Close();
            }

            _ = obj.SetProperty(Environment.ConnectionString, _driverPath);
            new SchemaExport(obj).Create(false, reset);
        }

        /// <summary>
        /// Destructor that closes the factory
        /// </summary>
        ~SqliteDB() {
            if (_factory != null) {
                _factory.Close();
            }
        }
    }
}
