using System;
using System.Collections.Generic;
using System.Text;
using Lotech.Data.Descriptor;

namespace Lotech.Data.Utility
{
    /// <summary>
    /// SQL查询工具类
    /// </summary>
    public static class QueryUtility
    {
        /// <summary>
        /// 遍历所有描述符
        /// </summary>
        /// <param name="iterateAction">遍历时的处理回调</param>
        /// <param name="fieldDescriptorsCollection"></param>
        public static void IterateFieldDescriptor(Action<FieldDescriptor> iterateAction, params IEnumerable<FieldDescriptor>[] fieldDescriptorsCollection)
        {
            if (iterateAction == null)
                throw new ArgumentNullException("iterateAction");
            if (fieldDescriptorsCollection == null)
                throw new ArgumentNullException("fieldDescriptorsCollection");
            foreach (var fieldDescriptors in fieldDescriptorsCollection)
            {
                if (fieldDescriptors != null)
                {
                    foreach (var fieldDescriptor in fieldDescriptors)
                    {
                        iterateAction.Invoke(fieldDescriptor);
                    }
                }
            }
        }

        /// <summary>
        /// 遍历所有描述符，构造SQL片断
        /// </summary>
        /// <param name="seperator">拼接的分隔符</param>
        /// <param name="concatAction"></param>
        /// <param name="fieldDescriptorsCollection"></param>
        /// <returns>返回拼接的SQL片断</returns>
        public static string ConcatQueryFragement(string seperator, Func<FieldDescriptor, string> concatAction, params IEnumerable<FieldDescriptor>[] fieldDescriptorsCollection)
        {
            if (seperator == null)
                throw new ArgumentNullException("seperator");
            if (fieldDescriptorsCollection == null)
                throw new ArgumentNullException("fieldDescriptorsCollection");
            StringBuilder queryFragement = new StringBuilder();
            IterateFieldDescriptor((fieldDescriptor) => {
                queryFragement.Append(concatAction(fieldDescriptor));
                queryFragement.Append(seperator);
            }, fieldDescriptorsCollection);
            if (queryFragement.Length > seperator.Length)
                queryFragement.Length -= seperator.Length;
            return queryFragement.ToString();
        }

        /// <summary>
        /// 获取字段清单，如：KeyID, Code, Name, State, Comment
        /// </summary>
        /// <param name="fieldDescriptorsCollection">字段描述列表</param>
        /// <returns></returns>
        public static string GetFieldList(params IEnumerable<FieldDescriptor>[] fieldDescriptorsCollection)
        {
            if (fieldDescriptorsCollection == null)
                throw new ArgumentNullException("fieldDescriptorsCollection");
            return ConcatQueryFragement(", ", fieldDescriptor => fieldDescriptor.Name, fieldDescriptorsCollection);
        }

        /// <summary>
        /// 获取字段清单，如：@KeyID, @Code, @Name, @State, @Comment
        /// </summary>
        /// <param name="database">数据库</param>
        /// <param name="fieldDescriptorsCollection">字段描述列表</param>
        /// <returns></returns>
        public static string GetFieldParameterList(IDatabase database, params IEnumerable<FieldDescriptor>[] fieldDescriptorsCollection)
        {
            if (database == null)
                throw new ArgumentNullException("database");
            if (fieldDescriptorsCollection == null)
                throw new ArgumentNullException("fieldDescriptorsCollection");
            return ConcatQueryFragement(", ", fieldDescriptor => database.BuildParameterName(fieldDescriptor.Name), fieldDescriptorsCollection);
        }

        /// <summary>
        /// 获取更新的SET集合，如：ID = @ID, Code= @Code
        /// </summary>
        /// <param name="database"></param>
        /// <param name="fieldDescriptorsCollection"></param>
        /// <returns></returns>
        public static string GetUpdateSetList(IDatabase database, params IEnumerable<FieldDescriptor>[] fieldDescriptorsCollection)
        {
            if (database == null)
                throw new ArgumentNullException("database");
            if (fieldDescriptorsCollection == null)
                throw new ArgumentNullException("fieldDescriptorsCollection");
            return ConcatQueryFragement(", ", fieldDescriptor =>
                string.Concat(fieldDescriptor.Name, " = ",
                database.BuildParameterName(fieldDescriptor.Name)), fieldDescriptorsCollection);
        }


        /// <summary>
        /// 获取条件集合，如：ID = @ID AND Code= @Code
        /// </summary>
        /// <param name="database"></param>
        /// <param name="fieldDescriptorsCollection"></param>
        /// <returns></returns>
        public static string GetConditiontList(IDatabase database, params IEnumerable<FieldDescriptor>[] fieldDescriptorsCollection)
        {
            if (database == null)
                throw new ArgumentNullException("database");
            if (fieldDescriptorsCollection == null)
                throw new ArgumentNullException("fieldDescriptorsCollection");
            return ConcatQueryFragement(" AND ", fieldDescriptor =>
                string.Concat(fieldDescriptor.Name, " = ",
                database.BuildParameterName(fieldDescriptor.Name)), fieldDescriptorsCollection);
        }
        /// <summary>
        /// 获取INSERT语句，如： INSERT INTO MessageTrace(ID, Code) VALUES(@ID, @Code)
        /// </summary>
        /// <param name="database"></param>
        /// <param name="entityDescriptor"></param>
        /// <returns></returns>
        public static string GetCreateQuery(IDatabase database, EntityDescriptor entityDescriptor)
        {
            if (database == null)
                throw new ArgumentNullException("database");
            if (entityDescriptor == null)
                throw new ArgumentNullException("entityDescriptor");

            return string.Format("INSERT INTO {0}({1}) VALUES({2})", entityDescriptor.Table
                , GetFieldList(entityDescriptor.PrimaryKeys, entityDescriptor.Fields)
                , GetFieldParameterList(database, entityDescriptor.PrimaryKeys, entityDescriptor.Fields));
        }/// <summary>
        /// 获取INSERT语句，如： INSERT INTO MessageTrace(ID, Code) VALUES(@ID, @Code)
        /// </summary>
        /// <param name="database"></param>
        /// <param name="entityDescriptor"></param>
        /// <returns></returns>
        public static string GetCreateQueryWithoutPrimaryKeys(IDatabase database, EntityDescriptor entityDescriptor)
        {
            if (database == null)
                throw new ArgumentNullException("database");
            if (entityDescriptor == null)
                throw new ArgumentNullException("entityDescriptor");

            return string.Format("INSERT INTO {0}({1}) VALUES({2})", entityDescriptor.Table
                , GetFieldList(entityDescriptor.Fields)
                , GetFieldParameterList(database, entityDescriptor.Fields));
        }

        /// <summary>
        /// 获取SELECT查询，如： SELECT * FROM MessageTrace
        /// </summary>
        /// <param name="entityDescriptor"></param>
        /// <returns></returns>
        public static string GetRetrieveQuery(EntityDescriptor entityDescriptor)
        {
            if (entityDescriptor == null)
                throw new ArgumentNullException("entityDescriptor");

            return string.Format("SELECT {0} FROM {1}",
                GetFieldList(entityDescriptor.PrimaryKeys, entityDescriptor.Fields), entityDescriptor.Table);
        }

        /// <summary>
        /// 获取SELECT按主键过滤查询，如： SELECT * FROM MessageTrace WHERE ID = @ID
        /// </summary>
        /// <param name="database"></param>
        /// <param name="entityDescriptor"></param>
        /// <returns></returns>
        public static string GetRetrieveWithIDQuery(IDatabase database, EntityDescriptor entityDescriptor)
        {
            if (database == null)
                throw new ArgumentNullException("database");
            if (entityDescriptor == null)
                throw new ArgumentNullException("entityDescriptor");

            return string.Format("SELECT {0} FROM {1} WHERE {2}"
                , GetFieldList(entityDescriptor.PrimaryKeys, entityDescriptor.Fields)
                , entityDescriptor.Table, GetConditiontList(database, entityDescriptor.PrimaryKeys));
        }

        /// <summary>
        /// 获取主键更新数据的查询，如： UPDATE Code = @Code, Name = @Name WHERE ID = @ID
        /// </summary>
        /// <param name="database"></param>
        /// <param name="entityDescriptor"></param>
        /// <returns></returns>
        public static string GetUpdateWithIDQuery(IDatabase database, EntityDescriptor entityDescriptor)
        {
            if (database == null)
                throw new ArgumentNullException("database");
            if (entityDescriptor == null)
                throw new ArgumentNullException("entityDescriptor");

            return string.Format("UPDATE {0} SET {1} WHERE {2}", entityDescriptor.Table
                , GetUpdateSetList(database, entityDescriptor.Fields)
                , GetConditiontList(database, entityDescriptor.PrimaryKeys));
        }


        /// <summary>
        /// 获取按主键删除数据查询，如：DELETE MessageType WHERE ID = @ID
        /// </summary>
        /// <param name="database"></param>
        /// <param name="entityDescriptor"></param>
        /// <returns></returns>
        public static string GetDeleteWithIDQuery(IDatabase database, EntityDescriptor entityDescriptor)
        {
            if (database == null)
                throw new ArgumentNullException("database");
            if (entityDescriptor == null)
                throw new ArgumentNullException("entityDescriptor");

            return string.Format("DELETE FROM {0} WHERE {1}", entityDescriptor.Table
                , GetConditiontList(database, entityDescriptor.PrimaryKeys));
        }

    }
}
