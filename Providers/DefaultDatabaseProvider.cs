using System.Data.Common;

namespace Lotech.Data.Providers
{
    class DefaultDatabaseProvider : IDatabaseProvider
    {
        IDatabase IDatabaseProvider.GetDatabase(DbProviderFactory dbProviderFactory, string connectionString)
        {
            return ((IDatabaseProvider)this).GetDatabase(dbProviderFactory, connectionString, ":");
        }

        IDatabase IDatabaseProvider.GetDatabase(DbProviderFactory dbProviderFactory, string connectionString, string parameterPrefix)
        {
            DbProviderDatabase database;
            switch (dbProviderFactory.GetType().Name)
            {
                case "OracleClientFactory":
                    database = new Oracle.OracleDatabase(dbProviderFactory);
                    break;
                case "SqlClientFactory":
                    database = new SqlServer.SqlServerDatabase();
                    break;
                case "MySqlClientFactory":
                    database = new MySql.MySqlDatabase(dbProviderFactory);
                    break;
                case "SQLiteFactory":
                    database = new SQLite.SQLiteDatabase(dbProviderFactory);
                    break;
                default:
                    database = new GenericDatabase(dbProviderFactory, parameterPrefix);
                    break;
            }

            database.ConnectionString = connectionString;

            return database;
        }
    }
}
