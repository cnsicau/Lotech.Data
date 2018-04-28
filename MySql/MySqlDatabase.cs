using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Transactions;
using Lotech.Data.Descriptor;
using Lotech.Data.Utility;
using System.Data;
using System.Text.RegularExpressions;

namespace Lotech.Data.MySql
{
    /// <summary>
    /// 增加 MySql 的插入主键获取支持
    /// </summary>
    public class MySqlDatabase : DbProviderDatabase, Expression.IExpressionEvaluatorProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbProviderFactory"></param>
        public MySqlDatabase(DbProviderFactory dbProviderFactory) : base(dbProviderFactory) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string BuildParameterName(string name)
        {
            return "@" + GetParameterName(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string GetParameterName(string name)
        {
            return Regex.Replace(name, @"^\s*`|`\s*$", string.Empty);
        }

        private void SetIdentityKeyValue(FieldDescriptor identityKey, object entity)
        {
            if (identityKey != null)
            {
                using (IDataReader reader = ExecuteReader("SELECT LAST_INSERT_ID()"))
                {
                    if (reader.Read())
                    {
                        var readerMapper = new Mapping.DataReaderFieldMapper(reader, 0, identityKey.Member);
                        readerMapper.Mapping(entity);
                    }
                }
            }
        }

        /// <summary>
        /// 
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
        public override void InsertEntities<EntityType>(IEnumerable<EntityType> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");
            var descriptor = GetEntityDescriptor(typeof(EntityType));
            string query = QueryUtility.GetCreateQueryWithoutPrimaryKeys(this, descriptor);

            FieldDescriptor identityKey = descriptor.PrimaryKeys != null && descriptor.PrimaryKeys.Length == 1
                ? descriptor.PrimaryKeys.FirstOrDefault(_ => _.KeyType == Attributes.KeyType.DbGenerate) : null;

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
                    SetIdentityKeyValue(identityKey, entity);
                }
                scope.Complete();
            }
        }

        Expression.IExpressionEvaluator Expression.IExpressionEvaluatorProvider.GetExpressionEvaluator(Expression.ExpressionEvaluateContext context, System.Linq.Expressions.Expression expression)
        {
            Expression.IExpressionEvaluatorProvider stringExpressionEvaluatorProvider
                = new Expression.StringExpressionEvaluatorProvider(args => "CONCAT(" + string.Join(", ", args) + ")"
                    , "{0} IS NULL", "CAST({0} AS VARCHAR(8000))");
            return stringExpressionEvaluatorProvider.GetExpressionEvaluator(context, expression);
        }
    }
}
