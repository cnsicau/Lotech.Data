using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Transactions;
using Lotech.Data.Attributes;
using Lotech.Data.Utility;

namespace Lotech.Data.SqlServer
{
    /// <summary>
    /// SQL Server数据库
    /// </summary>
    public class SqlServerDatabase : DbProviderDatabase, IDatabase, Expression.IExpressionEvaluatorProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public SqlServerDatabase()
            : base(System.Data.SqlClient.SqlClientFactory.Instance)
        {
        }

        /// <summary>
        /// SQL参数  @ParameterName 格式
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string BuildParameterName(string name)
        {
            return GetParameterName(name.StartsWith("@") ? name : "@" + GetParameterName(name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string GetParameterName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name is null or empty");
            var match = Regex.Match(name, @"^(\s*\[(?<Name>.*)\]\s*)|(\s*""(?<Name>.*)""\s*)$");
            if (match != null && match.Success)
                return match.Groups["Name"].Value;
            return name;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        Expression.IExpressionEvaluator Expression.IExpressionEvaluatorProvider.GetExpressionEvaluator(
            Expression.ExpressionEvaluateContext context, System.Linq.Expressions.Expression expression)
        {
            Expression.IExpressionEvaluatorProvider stringExpressionEvaluatorProvider
                = new Expression.StringExpressionEvaluatorProvider(args => string.Join(" + ", args)
                    , "({0} IS NULL OR {0} = '')", "CAST({0} AS VARCHAR(8000))");
            return stringExpressionEvaluatorProvider.GetExpressionEvaluator(context, expression);
        }

        #region 重载INSERT 实现主键生成算法
        /// <summary>
        /// 仅支持单一IDFieldAttribute主键的自动生成
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="entity"></param>
        public override void InsertEntity<EntityType>(EntityType entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            InsertEntities<EntityType>(new EntityType[] { entity });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="entities"></param>
        public override void InsertEntities<EntityType>(System.Collections.Generic.IEnumerable<EntityType> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");
            var entityDescriptor = GetEntityDescriptor(typeof(EntityType));
            // 处理DB主键
            var hasDbGeneratedKeys = Array.Exists(entityDescriptor.PrimaryKeys, keyDescriptor => {
                return entityDescriptor.PrimaryKeys[0].KeyType == KeyType.DbGenerate;
            });
            if (hasDbGeneratedKeys)
            {
                string query = string.Format("INSERT INTO {0}({1}) OUTPUT INSERTED.{2} VALUES({3})"
                    , entityDescriptor.Table, QueryUtility.GetFieldList(entityDescriptor.Fields.Where(_=>_.KeyType != KeyType.DbGenerate))
                    , QueryUtility.GetFieldList(entityDescriptor.PrimaryKeys)
                    , QueryUtility.GetFieldParameterList(this, entityDescriptor.Fields.Where(_ => _.KeyType != KeyType.DbGenerate)));
                using (DbCommand command = GetSqlStringCommand(query))
                using (var scope = new TransactionScope())
                {
                    foreach (var entity in entities)
                    {
                        command.Parameters.Clear();
                        Array.ForEach(entityDescriptor.Fields, field => AddInParameter(command, field, entity));
                        using (var reader = ExecuteReader(command))
                        {
                            reader.Read();
                            var fieldMapper = new Mapping.DataReaderFieldMapper(reader, 0, entityDescriptor.PrimaryKeys[0].Member);
                            fieldMapper.Mapping(entity);
                        }
                    }
                    scope.Complete();
                }
            }
            else
                base.InsertEntities(entities);
        }
        #endregion

        #region Update输出原始实体
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="entity"></param>
        /// <param name="originalEntity"></param>
        public override void UpdateEntity<EntityType>(EntityType entity, out EntityType originalEntity)
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
                using (var reader = base.ExecuteReader(command))
                {
                    var mapper = new Mapping.DataReaderMapper(this, reader, descriptor);
                    originalEntity = mapper.Mapping<EntityType>();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="entities"></param>
        /// <param name="originalEntities"></param>
        public override void UpdateEntities<EntityType>(System.Collections.Generic.IEnumerable<EntityType> entities, out System.Collections.Generic.IEnumerable<EntityType> originalEntities)
        {
            var descriptor = GetEntityDescriptor(typeof(EntityType));
            if (descriptor.PrimaryKeys.Length == 0)
                throw new InvalidOperationException("Can't update Entity without PrimaryKey");
            string query = QueryUtility.GetUpdateWithIDQuery(this, descriptor);
            using (var command = GetSqlStringCommand(query))
            using (var scope = new TransactionScope())
            {
                List<EntityType> updatedEntities = new List<EntityType>();
                foreach (var entity in entities)
                {
                    command.Parameters.Clear();
                    #region Bind Parameters
                    Array.ForEach(descriptor.Fields, field => AddInParameter(command, field, entity));
                    Array.ForEach(descriptor.PrimaryKeys, key => AddInParameter(command, key, entity));
                    #endregion
                    using (var reader = base.ExecuteReader(command))
                    {
                        var mapper = new Mapping.DataReaderMapper(this, reader, descriptor);
                        updatedEntities.Add(mapper.Mapping<EntityType>());
                    }
                }
                originalEntities = updatedEntities.ToArray();
                scope.Complete();
            }
        }
        #endregion
    }
}
