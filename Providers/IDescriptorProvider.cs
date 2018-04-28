using System;
using System.Reflection;
using Lotech.Data.Descriptor;

namespace Lotech.Data.Providers {
    /// <summary>
    /// 实体描述工厂接口，实现通过实体类型创建实体描述
    /// </summary>
    public interface IDescriptorProvider {
        /// <summary>
        /// 创建实体描述符对象
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <returns></returns>
        EntityDescriptor CreateEntityDescriptor(Type entityType);

        /// <summary>
        /// 创建字段的描述符
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        FieldDescriptor CreateFieldDescriptor(PropertyInfo property);
    }
}
