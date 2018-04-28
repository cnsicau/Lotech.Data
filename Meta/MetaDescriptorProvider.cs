using System;
using System.Linq;
using Lotech.Data.Descriptor;

namespace Lotech.Data.Meta
{
    /// <summary>
    /// 元数据提供者
    /// </summary>
    public class MetaDescriptorProvider : Providers.IDescriptorProvider
    {
        private MetaXml metaXml;

        /// <summary>
        /// 构造基于 XML 源的描述子提供器
        /// </summary>
        /// <param name="metaXml"></param>
        public MetaDescriptorProvider(MetaXml metaXml)
        {
            this.metaXml = metaXml;
        }

        /// <summary>
        /// 创建实体描述子
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public EntityDescriptor CreateEntityDescriptor(Type entityType)
        {
            return metaXml.CreateEntityDescriptor(entityType);
        }

        /// <summary>
        /// 创建字段描述子
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public FieldDescriptor CreateFieldDescriptor(System.Reflection.PropertyInfo property)
        {
            EntityDescriptor entity = metaXml.CreateEntityDescriptor(property.DeclaringType);
            return entity.PrimaryKeys.FirstOrDefault(_ => _.Member == property)
                ?? entity.Fields.FirstOrDefault(_ => _.Member == property);
        }
    }
}
