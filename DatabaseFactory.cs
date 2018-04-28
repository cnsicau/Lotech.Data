using System;
using System.Configuration;
using System.Data.Common;
using Lotech.Data.Cache;
using Lotech.Data.Configuration;
using Lotech.Data.Providers;

namespace Lotech.Data
{
    /// <summary>
    /// Database 工厂
    ///     通过此工厂创建的Database必须支持并发处理，因为一旦成功创建将被缓存
    /// </summary>
    public static class DatabaseFactory
    {
        static readonly DatabaseCache databaseCache = new DatabaseCache();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static public IDatabase CreateDatabase()
        {
            if (DatabaseSettings.Current == null)
                throw new InvalidProgramException("missing defaultDatabase");
            var settings = DatabaseSettings.Current;
            return CreateDatabase(settings.DefaultDatabase, settings.ParameterPrefix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        static public IDatabase CreateDatabase(string connectionStringName)
        {
            if (connectionStringName == null)
            {
                throw new ArgumentNullException("connectionStringName");
            }
            var settings = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (settings == null)
            {
                throw new InvalidProgramException("Connection not found: " + connectionStringName);
            }
            DbProviderFactory dbProviderFactory = DbProviderFactories.GetFactory(settings.ProviderName);

            return CreateDatabase(dbProviderFactory, settings.ConnectionString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <param name="parameterPrefix"></param>
        /// <returns></returns>
        static public IDatabase CreateDatabase(string connectionStringName, string parameterPrefix)
        {
            if (connectionStringName == null)
            {
                throw new ArgumentNullException("connectionStringName");
            }
            var settings = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (settings == null)
            {
                throw new InvalidProgramException("Connection not found: " + connectionStringName);
            }
            DbProviderFactory dbProviderFactory = DbProviderFactories.GetFactory(settings.ProviderName);

            return CreateDatabase(dbProviderFactory, settings.ConnectionString, parameterPrefix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbProviderFactory"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        static public IDatabase CreateDatabase(DbProviderFactory dbProviderFactory, string connectionString)
        {
            return CreateDatabase(dbProviderFactory, connectionString, ":");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbProviderFactory"></param>
        /// <param name="connectionString"></param>
        /// <param name="parameterPrefix"></param>
        /// <returns></returns>
        static public IDatabase CreateDatabase(DbProviderFactory dbProviderFactory, string connectionString, string parameterPrefix)
        {
            IDatabase database;
            var key = new DatabaseCacheName(dbProviderFactory, connectionString);
            if (databaseCache.TryGet(key, out database))
            {
                return database;
            }
            else
            {
                var providers = Utility.DatabaseProviderUtility.DetectDatabaseProviders();
                providers.Add(new DefaultDatabaseProvider());
                if (providers != null)
                {
                    foreach (IDatabaseProvider provider in providers)
                    {
                        database = provider.GetDatabase(dbProviderFactory, connectionString, parameterPrefix);
                        if (database != null)
                        {
                            databaseCache.Set(key, database); // 缓存此最佳的提供者
                            return database;
                        }
                    }
                }
            }
            throw new InvalidProgramException("Unsupported DbProviderFactory: " + dbProviderFactory);
        }
    }
}
