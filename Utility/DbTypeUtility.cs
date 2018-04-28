using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Lotech.Data.Utility {
    /// <summary>
    /// DbType 工具类
    /// </summary>
    static internal class DbTypeUtility {
        private static readonly Dictionary<Type, DbType> innerTypeMapping = new Dictionary<Type, DbType>();

        static DbTypeUtility() {
            lock (innerTypeMapping) {
                innerTypeMapping.Add(typeof(bool), DbType.Boolean);
                innerTypeMapping.Add(typeof(byte), DbType.Byte);
                innerTypeMapping.Add(typeof(sbyte), DbType.Byte);
                innerTypeMapping.Add(typeof(char), DbType.String);
                innerTypeMapping.Add(typeof(ushort), DbType.Int16);
                innerTypeMapping.Add(typeof(short), DbType.Int16);
                innerTypeMapping.Add(typeof(uint), DbType.Int32);
                innerTypeMapping.Add(typeof(int), DbType.Int32);
                innerTypeMapping.Add(typeof(ulong), DbType.Int64);
                innerTypeMapping.Add(typeof(long), DbType.Int64);
                innerTypeMapping.Add(typeof(float), DbType.Single);
                innerTypeMapping.Add(typeof(double), DbType.Double);
                innerTypeMapping.Add(typeof(decimal), DbType.Decimal);
                innerTypeMapping.Add(typeof(DateTime), DbType.DateTime);
                innerTypeMapping.Add(typeof(Guid), DbType.Guid);

                innerTypeMapping.Add(typeof(bool?), DbType.Boolean);
                innerTypeMapping.Add(typeof(byte?), DbType.Byte);
                innerTypeMapping.Add(typeof(sbyte?), DbType.Byte);
                innerTypeMapping.Add(typeof(char?), DbType.String);
                innerTypeMapping.Add(typeof(ushort?), DbType.Int16);
                innerTypeMapping.Add(typeof(short?), DbType.Int16);
                innerTypeMapping.Add(typeof(uint?), DbType.Int32);
                innerTypeMapping.Add(typeof(int?), DbType.Int32);
                innerTypeMapping.Add(typeof(ulong?), DbType.Int64);
                innerTypeMapping.Add(typeof(long?), DbType.Int64);
                innerTypeMapping.Add(typeof(float?), DbType.Single);
                innerTypeMapping.Add(typeof(double?), DbType.Double);
                innerTypeMapping.Add(typeof(decimal?), DbType.Decimal);
                innerTypeMapping.Add(typeof(DateTime?), DbType.DateTime);
                innerTypeMapping.Add(typeof(Guid?), DbType.Guid);

                innerTypeMapping.Add(typeof(byte[]), DbType.Binary);
                innerTypeMapping.Add(typeof(string), DbType.AnsiString);
            }
        }

        /// <summary>
        /// 通过数据类型查询数据库类型
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        static public DbType ParseDbType(Type dataType) {
            if (dataType.IsEnum)
                return DbType.Int32;
            if (dataType.IsGenericType && dataType.GetGenericArguments()[0].IsEnum)
                return DbType.Int32;

            return innerTypeMapping[dataType];
        }
    }
}
