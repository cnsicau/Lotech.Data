using System;
using System.Data;

namespace Lotech.Data.Attributes
{
    /// <summary>
    /// 字段属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class FieldAttribute : Attribute
    {
        private DbType dbType = DbType.Object;

        /// <summary>
        /// 是否忽略
        /// </summary>
        public virtual bool IsIgnore
        {
            get;
            set;
        }
        /// <summary>
        /// 主键
        /// </summary>
        public virtual bool IsKey
        {
            get;
            set;
        }
        /// <summary>
        /// 字段名称
        /// </summary>
        public virtual string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 字段类型
        /// </summary>
        public virtual DbType DbType
        {
            get { return dbType; }
            set { dbType = value; }
        }

        /// <summary>
        /// 长度
        /// </summary>
        public virtual int Length { get; set; }

        /// <summary>
        /// 长度(包含精度）
        /// </summary>
        public virtual int Precision { get; set; }

        /// <summary>
        /// 精度
        /// </summary>
        public virtual int Scale { get; set; }

        /// <summary>
        /// 字段大小
        /// </summary>
        public virtual int Size
        {
            get;
            set;
        }
    }
}
