using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Transactions;
using TransactionScopeConnectionCache = System.Collections.Generic.Dictionary<System.Transactions.Transaction
            , System.Collections.Generic.Dictionary<string, System.Data.Common.DbConnection>>;

namespace Lotech.Data
{
    /// <summary>
    /// 事务范围连接缓存
    /// </summary>
    static public class TransactionScopeConnections
    {
        /// <summary>
        /// 连接缓存
        /// </summary>
        private static readonly TransactionScopeConnectionCache connectionCache = new TransactionScopeConnectionCache();

        /// <summary>
        /// 获取事务范围内的数据库连接
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000")]
        public static DbConnection GetConnection(IDatabase database)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }
            if (Transaction.Current == null)
            {
                return null;
            }
            Dictionary<string, DbConnection> item;
            if (!connectionCache.TryGetValue(Transaction.Current, out item))
            {
                lock (connectionCache)
                {
                    if (!connectionCache.TryGetValue(Transaction.Current, out item))
                    {
                        item = new Dictionary<string, DbConnection>();

                        #region Dispose Transaction & Resources
                        Transaction.Current.TransactionCompleted += (s, e) => {
                            Dictionary<string, DbConnection> cacheItem;
                            if (!connectionCache.TryGetValue(e.Transaction, out cacheItem))
                                return;
                            foreach (DbConnection connection in cacheItem.Values)
                            {
                                connection.Close();
                            }
                            lock (connectionCache) { connectionCache.Remove(e.Transaction); }
                        };
                        #endregion

                        connectionCache.Add(Transaction.Current, item);
                    }
                }
            }
            DbConnection result;
            if (!item.TryGetValue(database.ConnectionString, out result))
            {
                lock (item)
                {
                    if (!item.TryGetValue(database.ConnectionString, out result))
                    {
                        result = database.CreateConnection();
                        item.Add(database.ConnectionString, result);
                    }
                }
            }
            return result;
        }
    }
}
