using System;
using System.Collections.Generic;
using System.Data;
using Lotech.Data.Descriptor;

namespace Lotech.Data.Mapping
{
    /// <summary>
    /// 读取器映射
    /// </summary>
    public class DataReaderMapper
    {
        #region Fields

        IDataReader reader;
        EntityDescriptor descriptor;
        IEnumerable<DataReaderFieldMapper> fieldMappers;
        #endregion

        /// <summary>
        /// 创建Reader的字段映射
        /// </summary>
        /// <param name="database"></param>
        /// <param name="reader">DataReader对象</param>
        /// <param name="descriptor">实体描述符</param>
        /// <returns></returns>
        protected static IEnumerable<DataReaderFieldMapper> BuildDataReaderFieldMappers(IDatabase database, IDataReader reader, EntityDescriptor descriptor)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (descriptor == null)
                throw new ArgumentNullException("descriptor");
            if (reader.IsClosed)
                throw new InvalidOperationException("reader is closed");
            if ((descriptor.Fields == null || descriptor.Fields.Length == 0)
                && (descriptor.PrimaryKeys == null || descriptor.PrimaryKeys.Length == 0))
                throw new InvalidOperationException(descriptor.EntityType + "descriptor is invalid: Fields is empty");

            List<DataReaderFieldMapper> fieldMappers = new List<DataReaderFieldMapper>();
            for (var i = 0;i < reader.FieldCount;i++)
            {
                var fieldName = reader.GetName(i);
                foreach (var field in descriptor.Fields)
                {
                    if (string.Compare(database.GetParameterName(field.Name), fieldName, true) == 0)
                    {
                        var fieldMapper = new DataReaderFieldMapper(reader, i, field.Member);
                        fieldMappers.Add(fieldMapper);
                    }
                }
                foreach (var field in descriptor.PrimaryKeys)
                {
                    if (string.Compare(database.GetParameterName(field.Name), fieldName, true) == 0)
                    {
                        var fieldMapper = new DataReaderFieldMapper(reader, i, field.Member);
                        fieldMappers.Add(fieldMapper);
                    }
                }
            }
            return fieldMappers.ToArray();
        }

        /// <summary>
        /// 构造DataReader映射
        /// </summary>
        /// <param name="database">DB</param>
        /// <param name="reader">DataReader对象</param>
        /// <param name="descriptor">实体描述符</param>
        public DataReaderMapper(IDatabase database, IDataReader reader, EntityDescriptor descriptor)
        {
            this.descriptor = descriptor;
            this.reader = reader;
            this.fieldMappers = BuildDataReaderFieldMappers(database, reader, descriptor);
        }

        /// <summary>
        /// 将Reader映射至指定实体
        /// </summary>
        /// <typeparam name="EntityType">实体类型</typeparam>
        /// <param name="entity">映射的实体</param>
        /// <returns>返回是否映射成功</returns>
        public bool Mapping<EntityType>(ref EntityType entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (typeof(EntityType) != descriptor.EntityType
                    && !descriptor.EntityType.IsSubclassOf(typeof(EntityType))
                    && !typeof(EntityType).IsSubclassOf(descriptor.EntityType))
                throw new InvalidProgramException(string.Format("Can't mapping {0} to {1}(descriptor)."
                    , typeof(EntityType), descriptor.EntityType));

            if (!reader.Read())
                return false;

            foreach (var fieldMapper in fieldMappers)
            {
                fieldMapper.Mapping(entity);
            }

            return true;
        }

        /// <summary>
        /// 映射并创建实体
        /// </summary>
        /// <typeparam name="EntityType"></typeparam>
        /// <returns></returns>
        /// <exception cref="System.InvalidProgramException">描述符不一致时抛出</exception>
        public EntityType Mapping<EntityType>()
        {
            if (typeof(EntityType) != descriptor.EntityType
                    && !descriptor.EntityType.IsSubclassOf(typeof(EntityType))
                    && !typeof(EntityType).IsSubclassOf(descriptor.EntityType))
                throw new InvalidProgramException(string.Format("Can't mapping {0} to {1}(descriptor)."
                    , typeof(EntityType), descriptor.EntityType));
            if (!reader.Read())
            {
                return default(EntityType);
            }

            var entity = (EntityType)Activator.CreateInstance(descriptor.EntityType);

            foreach (var fieldMapper in fieldMappers)
            {
                fieldMapper.Mapping(entity);
            }
            return entity;
        }
    }
}
