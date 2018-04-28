using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Transactions;
using Lotech.Data.Descriptor;
using Lotech.Data.Mapping;
using Lotech.Data.Utility;
using System.Linq.Expressions;

namespace Lotech.Data
{
    /// <summary>
    /// DbProvider 实现
    /// </summary>
    public abstract class DbProviderDatabase : IDatabase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentDbProviderFactory"></param>
        protected DbProviderDatabase(DbProviderFactory currentDbProviderFactory)
        {
            this.currentDbProviderFactory = currentDbProviderFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        protected DbProviderFactory currentDbProviderFactory;

        /// <summary>
        /// 获取实体的描述符
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        protected EntityDescriptor GetEntityDescriptor(Type entityType)
        {
            var entityDescriptorProvider = GetEntityDescriptorProvider(entityType);

            if (entityDescriptorProvider == null)
            {
                throw new InvalidProgramException("EntityDescriptorProvider");
            }

            return entityDescriptorProvider.CreateEntityDescriptor(entityType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        protected virtual Providers.IDescriptorProvider GetEntityDescriptorProvider(Type entityType)
        {
            return DescriptorProviderFactory.DectectDescriptorProvider(entityType)
                        ?? DescriptorProviderFactory.GetEntityDescriptorProvider();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string ConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual System.Data.Common.DbConnection CreateConnection()
        {
            if (String.IsNullOrEmpty(ConnectionString))
            {
                throw new InvalidProgramException("ConnectionString is empty");
            }

            var connection = currentDbProviderFactory.CreateConnection();
            connection.ConnectionString = ConnectionString;
            return connection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected ConnectionSubstitute GetOpenConnection()
        {
            var connection = TransactionScopeConnections.GetConnection(this);
            bool transactionScopeManaged = connection != null;
            if (connection == null)
            {
                connection = CreateConnection();
            }
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.ConnectionString = this.ConnectionString;
                connection.Open();
            }
            return new ConnectionSubstitute(connection, transactionScopeManaged);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100")]
        public virtual DbCommand GetCommand(System.Data.CommandType commandType, string commandText)
        {
            var command = currentDbProviderFactory.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = commandType;
            return command;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual System.Data.Common.DbCommand GetSqlStringCommand(string query)
        {
            return GetCommand(System.Data.CommandType.Text, query);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        public virtual System.Data.Common.DbCommand GetStoredProcedureCommand(string procedureName)
        {
            return GetCommand(System.Data.CommandType.StoredProcedure, procedureName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="direction"></param>
        /// <param name="value"></param>
        public virtual void AddParameter(System.Data.Common.DbCommand command, string parameterName, System.Data.DbType dbType, System.Data.ParameterDirection direction, object value)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (string.IsNullOrEmpty(parameterName))
            {
                throw new ArgumentNullException("parameterName");
            }

            var parameter = currentDbProviderFactory.CreateParameter();
            parameter.Direction = direction;
            parameter.DbType = dbType;
            parameter.Value = value ?? DBNull.Value;
            parameter.ParameterName = parameterName;

            command.Parameters.Add(parameter);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="direction"></param>
        /// <param name="size"></param>
        /// <param name="nullable"></param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        /// <param name="value"></param>
        public virtual void AddParameter(System.Data.Common.DbCommand command, string parameterName, System.Data.DbType dbType, System.Data.ParameterDirection direction, int size, bool nullable, int precision, int scale, object value)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (string.IsNullOrEmpty(parameterName))
            {
                throw new ArgumentNullException("parameterName");
            }

            var parameter = currentDbProviderFactory.CreateParameter();
            parameter.Direction = System.Data.ParameterDirection.Output;
            parameter.DbType = dbType;
            parameter.SourceColumnNullMapping = nullable;
            parameter.Size = size;
            parameter.ParameterName = parameterName;

            command.Parameters[parameterName] = parameter;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="value"></param>
        public virtual void AddInParameter(System.Data.Common.DbCommand command, string parameterName, System.Data.DbType dbType, object value)
        {
            AddParameter(command, parameterName, dbType, System.Data.ParameterDirection.Input, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        public virtual void AddOutParameter(System.Data.Common.DbCommand command, string parameterName, System.Data.DbType dbType, int size)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (string.IsNullOrEmpty(parameterName))
            {
                throw new ArgumentNullException("parameterName");
            }

            var parameter = currentDbProviderFactory.CreateParameter();
            parameter.Direction = System.Data.ParameterDirection.Output;
            parameter.DbType = dbType;
            parameter.Size = size;
            parameter.ParameterName = parameterName;

            command.Parameters[parameterName] = parameter;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract string BuildParameterName(string name);

        /// <summary>
        ///  获取参数名，默认返回name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual string GetParameterName(string name) { return name; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public virtual System.Data.DbType ParseDbType(Type propertyType)
        {
            return Utility.DbTypeUtility.ParseDbType(propertyType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public virtual object GetParameterValue(System.Data.Common.DbCommand command, string parameterName)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (parameterName == null)
                throw new ArgumentNullException("parameterName");
            var parameter = command.Parameters[parameterName];
            return parameter.Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual System.Data.IDataReader ExecuteReader(System.Data.Common.DbCommand command)
        {
            var connectionSubstitute = GetOpenConnection();
            try
            {
                command.Connection = connectionSubstitute.Connection;
                var reader = command.ExecuteReader(connectionSubstitute.DataReaderBehavior);
                return new CompositedDataReader(reader, connectionSubstitute);
            }
            catch
            {
                if (connectionSubstitute != null)
                    connectionSubstitute.Dispose();
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public System.Data.IDataReader ExecuteReader(string commandText)
        {
            return ExecuteReader(System.Data.CommandType.Text, commandText);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public virtual System.Data.IDataReader ExecuteReader(System.Data.CommandType commandType, string commandText)
        {
            var command = GetCommand(commandType, commandText);
            try
            {
                return ExecuteReader(command);
            }
            catch
            {
                if (command != null)
                    command.Dispose();
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual object ExecuteScalar(System.Data.Common.DbCommand command)
        {
            using (var connectionSubstitute = GetOpenConnection())
            {
                command.Connection = connectionSubstitute.Connection;
                object ret = command.ExecuteScalar();
                return ret == DBNull.Value ? null : ret;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText)
        {
            return ExecuteScalar(System.Data.CommandType.Text, commandText);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public virtual object ExecuteScalar(System.Data.CommandType commandType, string commandText)
        {
            using (var command = GetCommand(commandType, commandText))
            {
                return this.ExecuteScalar(command);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TScalar"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual TScalar ExecuteScalar<TScalar>(System.Data.Common.DbCommand command)
        {
            var scalar = ExecuteScalar(command);
            if (scalar == null)
                return default(TScalar);
            else if (scalar is TScalar)
                return (TScalar)scalar;
            else
                return (TScalar)Convert.ChangeType(scalar, typeof(TScalar));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TScalar"></typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public TScalar ExecuteScalar<TScalar>(string commandText)
        {
            return ExecuteScalar<TScalar>(System.Data.CommandType.Text, commandText);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TScalar"></typeparam>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public virtual TScalar ExecuteScalar<TScalar>(System.Data.CommandType commandType, string commandText)
        {
            using (var command = GetCommand(commandType, commandText))
            {
                return this.ExecuteScalar<TScalar>(command);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000")]
        public virtual System.Data.DataSet ExecuteDataSet(System.Data.Common.DbCommand command)
        {
            using (DbDataAdapter adapter = currentDbProviderFactory.CreateDataAdapter())
            {
                adapter.SelectCommand = command;
                using (var connectionSubsitute = GetOpenConnection())
                {
                    command.Connection = connectionSubsitute.Connection;
                    var dataSet = new System.Data.DataSet("Table");
                    adapter.Fill(dataSet);
                    return dataSet;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000")]
        public System.Data.DataSet ExecuteDataSet(string commandText)
        {
            return ExecuteDataSet(System.Data.CommandType.Text, commandText);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000")]
        public virtual System.Data.DataSet ExecuteDataSet(System.Data.CommandType commandType, string commandText)
        {
            using (var command = GetCommand(commandType, commandText))
            {
                return this.ExecuteDataSet(command);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual int ExecuteNonQuery(System.Data.Common.DbCommand command)
        {
            using (var connectionSubsitute = GetOpenConnection())
            {
                command.Connection = connectionSubsitute.Connection;
                return command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public virtual int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(System.Data.CommandType.Text, commandText);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public virtual int ExecuteNonQuery(System.Data.CommandType commandType, string commandText)
        {
            using (var command = GetCommand(commandType, commandText))
            {
                return ExecuteNonQuery(command);
            }
        }

        #region Exists

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Exists<EntityType>(object id)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            var entityDescriptor = GetEntityDescriptor(typeof(EntityType));
            if (entityDescriptor.PrimaryKeys.Length == 0)
                throw new InvalidProgramException("Missing PrimaryKey: " + typeof(EntityType));

            if (entityDescriptor.PrimaryKeys.Length > 1)
                throw new InvalidProgramException(typeof(EntityType) + "Only one PrimaryKey required, but : " + entityDescriptor.PrimaryKeys.Length);
            string query = string.Format("SELECT 1 FROM {0} WHERE {1} = {2}"
                , entityDescriptor.Table
                , entityDescriptor.PrimaryKeys[0].Name
                , BuildParameterName(entityDescriptor.PrimaryKeys[0].Name));

            using (var command = GetSqlStringCommand(query))
            {
                AddInParameter(command, GetParameterName(entityDescriptor.PrimaryKeys[0].Name), entityDescriptor.PrimaryKeys[0].DbType, id);
                return ExecuteScalar(command) != null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public virtual bool Exists<EntityType>(System.Linq.Expressions.Expression<Func<EntityType, bool>> conditions)
        {
            if (conditions == null)
                throw new ArgumentNullException("conditions");

            var entityDescriptor = GetEntityDescriptor(typeof(EntityType));
            string query = string.Format("SELECT 1 FROM {0}", entityDescriptor.Table);
            var ret = EvaluateExpression<EntityType>(this, GetEntityDescriptorProvider(typeof(EntityType)), conditions);
            if (!string.IsNullOrEmpty(ret.Fragement))
            {
                query = string.Concat(query, " WHERE ", ret.Fragement);
            }

            using (var command = GetSqlStringCommand(query))
            {
                ret.Parameters.ForEach(p => AddInParameter(command, GetParameterName(p.Name), p.DbType, p.Value));
                return ExecuteScalar(command) != null;
            }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual EntityType LoadEntity<EntityType>(object id)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            var entityDescriptor = GetEntityDescriptor(typeof(EntityType));
            if (entityDescriptor.PrimaryKeys.Length == 0)
                throw new InvalidProgramException("Missing PrimaryKey: " + typeof(EntityType));

            if (entityDescriptor.PrimaryKeys.Length > 1)
                throw new InvalidProgramException(typeof(EntityType) + "Only one PrimaryKey required, but : " + entityDescriptor.PrimaryKeys.Length);
            string query = QueryUtility.GetRetrieveWithIDQuery(this, entityDescriptor);

            using (var command = GetSqlStringCommand(query))
            {
                AddInParameter(command, GetParameterName(entityDescriptor.PrimaryKeys[0].Name), entityDescriptor.PrimaryKeys[0].DbType, id);
                return ExecuteEntity<EntityType>(command);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public virtual EntityType LoadEntity<EntityType>(System.Linq.Expressions.Expression<Func<EntityType, bool>> conditions)
        {
            if (conditions == null)
                throw new ArgumentNullException("conditions");

            var entityDescriptor = GetEntityDescriptor(typeof(EntityType));
            string query = QueryUtility.GetRetrieveQuery(entityDescriptor);
            var ret = EvaluateExpression<EntityType>(this, GetEntityDescriptorProvider(typeof(EntityType)), conditions);
            if (!string.IsNullOrEmpty(ret.Fragement))
            {
                query = string.Concat(query, " WHERE ", ret.Fragement);
            }

            using (var command = GetSqlStringCommand(query))
            {
                ret.Parameters.ForEach(p => AddInParameter(command, GetParameterName(p.Name), p.DbType, p.Value));
                return ExecuteEntity<EntityType>(command);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual EntityType ExecuteEntity<EntityType>(DbCommand command)
        {
            var descriptor = GetEntityDescriptor(typeof(EntityType));
            using (var reader = ExecuteReader(command))
            {
                var mapper = new Mapping.DataReaderMapper(this, reader, descriptor);
                return mapper.Mapping<EntityType>();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public virtual IEnumerable<EntityType> ExecuteEntities<EntityType>(DbCommand command)
        {
            var descriptor = GetEntityDescriptor(typeof(EntityType));

            using (var reader = ExecuteReader(command))
            {
                var mapper = new DataReaderMapper(this, reader, descriptor);
                var entity = Activator.CreateInstance<EntityType>();
                var result = new List<EntityType>();
                while (mapper.Mapping(ref entity))
                {
                    result.Add(entity);
                    entity = Activator.CreateInstance<EntityType>();
                }
                return result.ToArray();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public virtual EntityType ExecuteEntity<EntityType>(string commandText)
        {
            return ExecuteEntity<EntityType>(System.Data.CommandType.Text, commandText);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public virtual EntityType ExecuteEntity<EntityType>(System.Data.CommandType commandType, string commandText)
        {
            var descriptor = GetEntityDescriptor(typeof(EntityType));
            using (var reader = ExecuteReader(commandType, commandText))
            {
                var mapper = new Mapping.DataReaderMapper(this, reader, descriptor);
                return mapper.Mapping<EntityType>();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public virtual IEnumerable<EntityType> ExecuteEntities<EntityType>(string commandText)
        {
            return ExecuteEntities<EntityType>(System.Data.CommandType.Text, commandText);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public virtual IEnumerable<EntityType> ExecuteEntities<EntityType>(System.Data.CommandType commandType, string commandText)
        {
            var descriptor = GetEntityDescriptor(typeof(EntityType));
            using (var reader = ExecuteReader(commandType, commandText))
            {
                var mapper = new Mapping.DataReaderMapper(this, reader, descriptor);
                var result = new List<EntityType>();
                var entity = Activator.CreateInstance<EntityType>();
                while (mapper.Mapping(ref entity))
                {
                    result.Add(entity);
                    entity = Activator.CreateInstance<EntityType>();
                }
                return result.ToArray();
            }
        }

        /// <summary>
        /// 添加实体字段描述的参数
        /// </summary>
        /// <param name="command"></param>
        /// <param name="descriptor"></param>
        /// <param name="entity"></param>
        protected virtual void AddInParameter(DbCommand command, FieldDescriptor descriptor, object entity)
        {
            if (descriptor.DbType == System.Data.DbType.Object)
            {
                descriptor.DbType = ParseDbType(descriptor.Member.PropertyType);
            }
            AddInParameter(command, GetParameterName(descriptor.Name), descriptor.DbType,
                entity != null ? descriptor.Member.GetValue(entity, null) : DBNull.Value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="entity"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202")]
        public virtual void InsertEntity<EntityType>(EntityType entity)
        {
            var descriptor = GetEntityDescriptor(typeof(EntityType));
            string query = QueryUtility.GetCreateQuery(this, descriptor);
            using (var command = GetSqlStringCommand(query))
            {
                #region Bind Parameters
                Array.ForEach(descriptor.PrimaryKeys, key => AddInParameter(command, key, entity));
                Array.ForEach(descriptor.Fields, field => AddInParameter(command, field, entity));
                #endregion

                this.ExecuteNonQuery(command);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="entities"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202")]
        public virtual void InsertEntities<EntityType>(IEnumerable<EntityType> entities)
        {
            var descriptor = GetEntityDescriptor(typeof(EntityType));
            string query = QueryUtility.GetCreateQuery(this, descriptor);
            using (var command = GetSqlStringCommand(query))
            using (var scope = new TransactionScope())
            {
                var connection = GetOpenConnection();
                command.Connection = connection.Connection;
                foreach (var entity in entities)
                {
                    command.Parameters.Clear();
                    #region Bind Parameters
                    Array.ForEach(descriptor.PrimaryKeys, key => AddInParameter(command, key, entity));
                    Array.ForEach(descriptor.Fields, field => AddInParameter(command, field, entity));
                    #endregion

                    command.ExecuteNonQuery();
                }
                scope.Complete();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="entity"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202")]
        public virtual void UpdateEntity<EntityType>(EntityType entity)
        {
            var descriptor = GetEntityDescriptor(typeof(EntityType));
            if (descriptor.PrimaryKeys.Length == 0)
                throw new InvalidOperationException("Can't update Entity without PrimaryKey");

            string query = QueryUtility.GetUpdateWithIDQuery(this, descriptor);
            using (var command = GetSqlStringCommand(query))
            {
                #region Bind Parameters
                Array.ForEach(descriptor.Fields, field => AddInParameter(command, field, entity));
                Array.ForEach(descriptor.PrimaryKeys, key => AddInParameter(command, key, entity));
                #endregion
                ExecuteNonQuery(command);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="entity"></param>
        /// <param name="originalEntity"></param>
        public virtual void UpdateEntity<EntityType>(EntityType entity, out EntityType originalEntity)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="entities"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202")]
        public virtual void UpdateEntities<EntityType>(IEnumerable<EntityType> entities)
        {
            var descriptor = GetEntityDescriptor(typeof(EntityType));
            if (descriptor.PrimaryKeys.Length == 0)
                throw new InvalidOperationException("Can't update Entity without PrimaryKey");

            string query = QueryUtility.GetUpdateWithIDQuery(this, descriptor);
            using (var command = GetSqlStringCommand(query))
            using (var scope = new TransactionScope())
            {
                var connection = GetOpenConnection();
                command.Connection = connection.Connection;

                foreach (var entity in entities)
                {
                    command.Parameters.Clear();
                    #region Bind Parameters
                    Array.ForEach(descriptor.Fields, field => AddInParameter(command, field, entity));
                    Array.ForEach(descriptor.PrimaryKeys, key => AddInParameter(command, key, entity));
                    #endregion
                    command.ExecuteNonQuery();
                }
                scope.Complete();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="entities"></param>
        /// <param name="originalEntities"></param>
        public virtual void UpdateEntities<EntityType>(IEnumerable<EntityType> entities, out IEnumerable<EntityType> originalEntities)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="sets"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public int UpdateEntities<EntityType>(UpdateSetClusters<EntityType> sets
                            , Expression<Func<EntityType, bool>> conditions)
        {
            var context = new Expression.ExpressionEvaluateContext(this, GetEntityDescriptorProvider(typeof(EntityType)));
            var set = sets.Evaluate(context);
            var where = EvaluateExpression<EntityType>(this, GetEntityDescriptorProvider(typeof(EntityType)), conditions);
            string query = string.Format("UPDATE {0} SET {1} WHERE {2}"
                , GetEntityDescriptor(typeof(EntityType)).Table
                , set.Fragement
                , where.Fragement);
            using (var command = GetSqlStringCommand(query))
            {
                set.Parameters.ForEach(p => AddInParameter(command, p.Name, p.DbType, p.Value));
                where.Parameters.ForEach(p => AddInParameter(command, p.Name, p.DbType, p.Value));
                return ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="id"></param>
        public virtual void DeleteEntity<EntityType>(object id)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            var entityDescriptor = GetEntityDescriptor(typeof(EntityType));
            if (entityDescriptor.PrimaryKeys.Length == 0)
                throw new InvalidProgramException("PrimaryKey required: " + typeof(EntityType));

            if (entityDescriptor.PrimaryKeys.Length > 1)
                throw new InvalidProgramException(typeof(EntityType) + "Only one PrimaryKey required, but : " + entityDescriptor.PrimaryKeys.Length);
            string query = QueryUtility.GetDeleteWithIDQuery(this, entityDescriptor);

            using (var command = GetSqlStringCommand(query))
            {
                AddInParameter(command, GetParameterName(entityDescriptor.PrimaryKeys[0].Name)
                    , entityDescriptor.PrimaryKeys[0].DbType, id);

                ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="entity"></param>
        public virtual void DeleteEntity<EntityType>(EntityType entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var entityDescriptor = GetEntityDescriptor(typeof(EntityType));
            if (entityDescriptor.PrimaryKeys.Length == 0)
                throw new InvalidProgramException("PrimaryKey required: " + typeof(EntityType));

            string query = QueryUtility.GetDeleteWithIDQuery(this, entityDescriptor);
            using (var command = GetSqlStringCommand(query))
            {
                Array.ForEach(entityDescriptor.PrimaryKeys, key =>
                    AddInParameter(command, key, entity));

                ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="entities"></param>
        public virtual void DeleteEntities<EntityType>(IEnumerable<EntityType> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            var entityDescriptor = GetEntityDescriptor(typeof(EntityType));
            if (entityDescriptor.PrimaryKeys.Length == 0)
                throw new InvalidProgramException("PrimaryKey required: " + typeof(EntityType));
            string query = QueryUtility.GetDeleteWithIDQuery(this, entityDescriptor);
            using (var command = GetSqlStringCommand(query))
            using (var scope = new TransactionScope())
            {
                var connection = GetOpenConnection();
                command.Connection = connection.Connection;
                foreach (var entity in entities)
                {
                    command.Parameters.Clear();

                    Array.ForEach(entityDescriptor.PrimaryKeys, key =>
                        AddInParameter(command, key, entity));
                    command.ExecuteNonQuery();
                }
                scope.Complete();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="conditions"></param>
        public virtual void DeleteEntities<EntityType>(System.Linq.Expressions.Expression<Func<EntityType, bool>> conditions)
        {
            if (conditions == null)
                throw new ArgumentNullException("conditions");

            var entityDescriptor = GetEntityDescriptor(typeof(EntityType));

            string query = string.Format("DELETE {0}", entityDescriptor.Table);
            var ret = EvaluateExpression<EntityType>(this, GetEntityDescriptorProvider(typeof(EntityType)), conditions);
            if (!string.IsNullOrEmpty(ret.Fragement))
            {
                query = string.Concat(query, " WHERE ", ret.Fragement);
            }
            using (var command = GetSqlStringCommand(query))
            {
                ret.Parameters.ForEach(p => AddInParameter(command, GetParameterName(p.Name), p.DbType, p.Value));
                ExecuteNonQuery(command);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <returns></returns>
        public virtual IEnumerable<EntityType> FindEntities<EntityType>()
        {
            var entityDescriptor = GetEntityDescriptor(typeof(EntityType));

            string query = QueryUtility.GetRetrieveQuery(entityDescriptor);
            using (var command = GetSqlStringCommand(query))
            {
                return ExecuteEntities<EntityType>(command);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public virtual IEnumerable<EntityType> FindEntities<EntityType>(System.Linq.Expressions.Expression<Func<EntityType, bool>> conditions)
        {
            if (conditions == null)
                throw new ArgumentNullException("conditions");

            var entityDescriptor = GetEntityDescriptor(typeof(EntityType));

            string query = QueryUtility.GetRetrieveQuery(entityDescriptor);
            var ret = EvaluateExpression<EntityType>(this, GetEntityDescriptorProvider(typeof(EntityType)), conditions);
            if (!string.IsNullOrEmpty(ret.Fragement))
            {
                query = string.Concat(query, " WHERE ", ret.Fragement);
            }
            using (var command = GetSqlStringCommand(query))
            {
                ret.Parameters.ForEach(p => AddInParameter(command, GetParameterName(p.Name), p.DbType, p.Value));
                return ExecuteEntities<EntityType>(command);
            }
        }

        protected static Expression.EvaluateResult EvaluateExpression<EntityType>(IDatabase database
            , Providers.IDescriptorProvider provider
            , System.Linq.Expressions.Expression<Func<EntityType, bool>> conditions)
        {
            var context = new Expression.ExpressionEvaluateContext(database, provider);
            var evaluator = Expression.ExpressionEvaluatorFactory.Create(context, conditions.Body);
            return evaluator.Evaluate(context, conditions.Body);
        }
    }
}
