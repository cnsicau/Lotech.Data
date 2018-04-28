using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Lotech.Data.Descriptor;

namespace Lotech.Data.Meta
{
    /// <summary>
    /// 表元素配置
    /// </summary>
    public class Table
    {
        static readonly List<Field> EmptyFields = new List<Field>();
        /// <summary>
        /// 表名称
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// 实体类型
        /// </summary>
        [XmlAttribute("entity")]
        public string EntityType { get; set; }

        /// <summary>
        /// 字段清单
        /// </summary>
        [XmlElement("Field")]
        public List<Field> Fields { get; set; }

        /// <summary>
        /// 创建实体描述者
        /// </summary>
        /// <returns></returns>
        public EntityDescriptor CreateEntityDescriptor()
        {
            if (Fields == null)
                Fields = EmptyFields;

            Type entityType = Type.GetType(EntityType, false);
            if (entityType == null)
                throw new InvalidProgramException("Missing entity type: " + EntityType);

            IEnumerable<FieldDescriptor> fieldDescriptors = Fields.Select(_ => _.CreateFieldDescriptor(entityType));

            return new EntityDescriptor(Name, entityType)
            {
                Fields = fieldDescriptors.Where(field => !field.IsKey).ToArray(),
                PrimaryKeys = fieldDescriptors.Where(field => field.IsKey).ToArray()
            };
        }
    }
}
