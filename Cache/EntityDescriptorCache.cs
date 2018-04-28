using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lotech.Data.Descriptor;

namespace Lotech.Data.Cache {
    /// <summary>
    /// 数据库实体描述缓存
    /// </summary>
    public class EntityDescriptorCache : FreeReadCache<Type, EntityDescriptor> {
        #region 构造函数
        /// <summary>
        /// 构造实体描述符缓存（强引用）
        /// </summary>
        public EntityDescriptorCache()
            : base(false) {
        }

        #endregion
    }
}
