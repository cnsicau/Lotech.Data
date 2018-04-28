using System;
using System.Xml.Serialization;
using Lotech.Data.Attributes;
using Lotech.Data.Descriptor;
using Lotech.Data.Utility;

namespace Lotech.Data.Meta
{
    /// <summary>
    /// 字段元素配置
    /// </summary>
    [XmlRoot("Field")]
    public class Field
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// 实体中属性名称
        /// </summary>
        [XmlAttribute("property")]
        public string PropertyName { get; set; }

        /// <summary>
        /// 数据库 DbType
        /// </summary>
        [XmlAttribute("dbType")]
        public string DbType { get; set; }

        /// <summary>
        /// 主键类型，如数据库生成、自定义生成
        /// </summary>
        [XmlAttribute("keyType")]
        public string KeyType { get; set; }

        /// <summary>
        /// 是否键
        /// </summary>
        [XmlAttribute("isKey")]
        public bool IsKey { get; set; }

        /// <summary>
        /// 创建字段描述
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public FieldDescriptor CreateFieldDescriptor(Type entityType)
        {
            System.Data.DbType dbType;
            if (!EnumUtility.TryParse<System.Data.DbType>(this.DbType ?? "String", out dbType))
            {
                throw new InvalidProgramException("DbType is of System.Data.DbType, invalid value: " + this.DbType);
            }
            KeyType keyType = global::Lotech.Data.Attributes.KeyType.Custom;
            if (IsKey)
            {
                KeyType = KeyType ?? "Custom";
                if (!EnumUtility.TryParse<KeyType>(KeyType, out keyType))
                {
                    throw new InvalidProgramException("DbType is of Lotech.Data.Attributes.KeyType, invalid value: " + this.KeyType);
                }
            }
            string memberName = PropertyName ?? Name;
            FieldDescriptor descriptor = new FieldDescriptor(this.Name, dbType, 0, entityType, memberName);
            descriptor.IsKey = IsKey;
            descriptor.KeyType = keyType;

            return descriptor;
        }
    }
}
