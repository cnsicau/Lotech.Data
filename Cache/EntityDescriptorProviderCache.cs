using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lotech.Data.Descriptor;

namespace Lotech.Data.Cache {
    /// <summary>
    /// 数据库实体描述提供者缓存
    /// </summary>
    public class EntityDescriptorProviderCache : FreeReadCache<Type, Providers.IDescriptorProvider> {
        #region 构造函数
        /// <summary>
        /// 构造实体描述符缓存（强引用）
        /// </summary>
        public EntityDescriptorProviderCache()
            : base(false) {
        }

        #endregion
    }
}
