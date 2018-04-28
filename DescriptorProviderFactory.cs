using System;
using Lotech.Data.Providers;
using Lotech.Data.Utility;

namespace Lotech.Data {
    /// <summary>
    /// 
    /// </summary>
    public static class DescriptorProviderFactory {
        private static readonly IDescriptorProvider defaultDescriptorProvider = new AttributeDescriptorProvider();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IDescriptorProvider GetEntityDescriptorProvider() {
            return defaultDescriptorProvider;
        }

        /// <summary>
        /// 自描述符提供实现的缓存
        /// </summary>
        static readonly Cache.EntityDescriptorProviderCache selfDescriptorProviderCache = new Cache.EntityDescriptorProviderCache();

        /// <summary>
        /// 检查实体类型中实现的描述符提供者，检测优化级
        ///     1. 实体自身实现了 IDescriptorProvider 创建实体实例
        ///     2. 实体的类注解中有实现 IDescriptorProvider的，取此注解
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static IDescriptorProvider DectectDescriptorProvider(Type entityType) {
            IDescriptorProvider descriptorProvider;
            if (!selfDescriptorProviderCache.TryGet(entityType, out descriptorProvider)) {
                lock (selfDescriptorProviderCache) {
                    if (typeof(IDescriptorProvider).IsAssignableFrom(entityType)) {
                        descriptorProvider = (IDescriptorProvider)Activator.CreateInstance(entityType);
                    }
                    else {
                        descriptorProvider = entityType.GetAttribute<IDescriptorProvider>();
                    }
                    selfDescriptorProviderCache.Set(entityType, descriptorProvider);
                }
            }
            return descriptorProvider;
        }
    }
}
