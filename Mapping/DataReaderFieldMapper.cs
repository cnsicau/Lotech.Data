using System;
using System.Data;
using System.Reflection;

namespace Lotech.Data.Mapping {
    /// <summary>
    /// T 
    /// </summary>
    public class DataReaderFieldMapper {
        IDataReader reader;
        int field;
        PropertyInfo property;
        Type distType;
        Type realType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="field"></param>
        /// <param name="property"></param>
        public DataReaderFieldMapper(IDataReader reader, int field, PropertyInfo property)
        {
            if (property == null)
                throw new System.ArgumentNullException("property");

            this.reader = reader;
            this.field = field;
            this.property = property;
            this.distType = property.PropertyType;
            this.realType = ParseRealType();
        }

        private Type ParseRealType() {
            if (this.distType.IsGenericType) {
                return distType.GetGenericArguments()[0];
            }
            return distType;
        }

        /// <summary>
        /// 映射当前值至指定实体属性
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <param name="entity"></param>
        /// <exception cref="System.ArgumentNullException">属性无效</exception>
        public void Mapping<EntityType>(EntityType entity) {
            if (entity == null)
                throw new System.ArgumentNullException("entity");
            property.SetValue(entity, GetValue(), null);
        }
        /// <summary>
        /// 获取当前Reader中此列的值
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">读取器已关闭</exception>
        public object GetValue() {
            if (reader.IsClosed)
                throw new InvalidOperationException("reader is closed");
            object obj = reader[field];
            if (obj == DBNull.Value || obj == null) {
                if (distType.IsValueType || distType.IsEnum)
                    return Activator.CreateInstance(distType);
                return null;
            }
            else if (obj.GetType() == realType || realType.IsEnum)
                return realType.IsEnum ? Convert.ToInt32(obj) : obj;
            else if (!(obj is string) && realType == typeof(string))
                return obj.ToString();

            return Convert.ChangeType(obj, realType);
        }
    }
}
