using System;
using System.Data.Common;

namespace Lotech.Data.Cache
{
    /// <summary>
    /// Database缓存键
    /// </summary>
    class DatabaseCacheName
    {
        int hashCode = 0;
        public DatabaseCacheName(DbProviderFactory dbProviderFactory, string connectionString)
        {
            this.DbProviderType = dbProviderFactory.GetType();
            this.ConnectionString = connectionString;
            ComputeHashCode();
        }
        public Type DbProviderType
        {
            get;
            private set;
        }
        public string ConnectionString
        {
            get;
            private set;
        }
        public override bool Equals(object obj)
        {
            return obj is DatabaseCacheName && ((DatabaseCacheName)obj).DbProviderType == this.DbProviderType
                && ((DatabaseCacheName)obj).ConnectionString == this.ConnectionString;
        }
        private void ComputeHashCode()
        {
            hashCode = string.Concat(DbProviderType, ConnectionString).GetHashCode();
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
    /// <summary>
    /// Database缓存
    /// </summary>
    class DatabaseCache : Cache.FreeReadCache<DatabaseCacheName, IDatabase>
    {
        public DatabaseCache()
            : base(false)
        {
        }
    }
}
