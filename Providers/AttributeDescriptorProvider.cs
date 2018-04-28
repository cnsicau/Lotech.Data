using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Lotech.Data.Attributes;
using Lotech.Data.Cache;
using Lotech.Data.Descriptor;
using Lotech.Data.Utility;

namespace Lotech.Data.Providers
{
    /// <summary>
    /// 属性注解描述符工厂
    /// </summary>
    public class AttributeDescriptorProvider : IDescriptorProvider
    {
        /// <summary>缓存</summary>
        static readonly EntityDescriptorCache descriptorCache = new EntityDescriptorCache();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        EntityDescriptor IDescriptorProvider.CreateEntityDescriptor(Type entityType)
        {
            EntityDescriptor descriptor;
            if (!descriptorCache.TryGet(entityType, out descriptor))
            {
                lock (descriptorCache)
                {
                    descriptor = new EntityDescriptor();
                    TableAttribute attr = entityType.GetAttribute<TableAttribute>();
                    descriptor.EntityType = entityType;
                    descriptor.Table = attr == null ? entityType.Name : attr.Name;
                    descriptor.Fields = CreateFieldDescriptors(entityType);
                    descriptor.PrimaryKeys = CreatePrimaryKeyDescriptors(entityType);
                    descriptorCache.Set(entityType, descriptor);
                }
            }
            return descriptor;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="System.InvalidOperationException">属性无效</exception>
        /// <param name="property"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        FieldDescriptor IDescriptorProvider.CreateFieldDescriptor(PropertyInfo property)
        {
            var fieldAttribute = property.GetAttribute<FieldAttribute>();

            if (fieldAttribute == null && (!property.CanRead || !property.CanWrite))
                throw new InvalidOperationException(property + " is invalid Field");

            return new FieldDescriptor
            {
                DbType = GetFieldDbType(fieldAttribute, property),
                Name = GetFieldName(fieldAttribute, property),
                Size = fieldAttribute == null ? 0 : fieldAttribute.Size,
                Member = property,
                KeyType = (fieldAttribute is KeyAttribute) ? ((KeyAttribute)fieldAttribute).KeyType : KeyType.Custom
            };
        }

        /// <summary>
        /// 创建字段描述符
        /// </summary>
        /// <returns></returns>
        static FieldDescriptor[] CreateFieldDescriptors(Type entityType, Func<FieldAttribute, bool> filter)
        {
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var fieldDescriptors = new List<FieldDescriptor>();
            foreach (var property in properties)
            {
                var fieldAttribute = property.GetAttribute<FieldAttribute>();
                if (filter != null && !filter(fieldAttribute))
                    continue;
                if (fieldAttribute == null && (!property.CanRead || !property.CanWrite))
                    continue;

                var descriptor = new FieldDescriptor
                {
                    DbType = GetFieldDbType(fieldAttribute, property),
                    Name = GetFieldName(fieldAttribute, property),
                    Size = fieldAttribute == null ? 0 : fieldAttribute.Size,
                    Member = property,
                    KeyType = (fieldAttribute is KeyAttribute) ? ((KeyAttribute)fieldAttribute).KeyType : KeyType.Custom
                };
                fieldDescriptors.Add(descriptor);
            }
            return fieldDescriptors.ToArray();
        }

        /// <summary>
        /// 创建普通字段
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        static FieldDescriptor[] CreateFieldDescriptors(Type entityType)
        {
            return CreateFieldDescriptors(entityType, fieldAttribute => fieldAttribute == null
                    || !fieldAttribute.IsIgnore && !fieldAttribute.IsKey);
        }

        /// <summary>
        /// 创建主键
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        static FieldDescriptor[] CreatePrimaryKeyDescriptors(Type entityType)
        {
            return CreateFieldDescriptors(entityType, fieldAttribute => fieldAttribute != null
                    && !fieldAttribute.IsIgnore && fieldAttribute.IsKey);
        }

        static DbType GetFieldDbType(FieldAttribute attribute, PropertyInfo property)
        {
            if (attribute != null && attribute.DbType != DbType.Object)
            {
                return attribute.DbType;
            }

            return DbType.Object;
        }

        static string GetFieldName(FieldAttribute attribute, PropertyInfo property)
        {
            if (attribute != null && !string.IsNullOrEmpty(attribute.Name))
            {
                return attribute.Name.Trim();
            }
            return property.Name;
        }
    }
}
