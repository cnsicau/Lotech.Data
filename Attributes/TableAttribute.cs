using System;

namespace Lotech.Data.Attributes {
    /// <summary>
    /// 实体属性注解
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TableAttribute : Attribute {
        /// <summary>
        /// 设置或获取应对的表名称，默认为null
        ///     当名称为null时，取对应实体的名称
        /// </summary>
        public string Name {
            get;
            set;
        }
    }
}
