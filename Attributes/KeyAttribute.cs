using System;

namespace Lotech.Data.Attributes
{
    /// <summary>
    /// 主键类型
    /// </summary>
    public enum KeyType
    {
        /// <summary>程序员自定义(默认）</summary>
        Custom = 0,
        /// <summary>程序查询最大值+1生成</summary>
        Sequence = 1,
        /// <summary>由数据库自动生成</summary>
        DbGenerate = 2
    }

    /// <summary>
    /// 主键注解
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class KeyAttribute : FieldAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public KeyAttribute()
        {
            base.IsKey = true;
            KeyType = KeyType.Custom;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsKey
        {
            get
            {
                return true;
            }
            set
            {
                if (!value)
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// 键类型(生成方式)
        /// </summary>
        public KeyType KeyType
        {
            get;
            set;
        }
    }
}
