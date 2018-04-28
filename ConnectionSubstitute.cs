using System;
using System.Data;
using System.Data.Common;

namespace Lotech.Data
{
    /// <summary>
    /// 连接替代品
    ///     用于关注增加是否受TransactionScope管理
    /// </summary>
    public sealed class ConnectionSubstitute : IDisposable
    {
        private DbConnection connection;
        private bool transactionScopeManaged;

        /// <summary>
        /// 
        /// </summary>
        public ConnectionSubstitute(DbConnection connection, bool transactionScopeManaged)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            this.connection = connection;
            this.transactionScopeManaged = transactionScopeManaged;
        }

        /// <summary>
        /// 
        /// </summary>
        public DbConnection Connection { get { return connection; } }

        /// <summary>
        /// 
        /// </summary>
        public CommandBehavior DataReaderBehavior { get { return transactionScopeManaged ? CommandBehavior.Default : CommandBehavior.CloseConnection; } }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (!transactionScopeManaged && connection != null)
            {
                connection.Close();
                connection.Dispose();

                connection = null;
            }
        }
    }
}
