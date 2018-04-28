using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Lotech.Data.Descriptor;

namespace Lotech.Data.Meta
{
    /// <summary>
    /// 元数据配置
    /// </summary>
    [XmlRoot("Meta")]
    public class MetaXml
    {
        /// <summary>
        /// 同步对象
        /// </summary>
        public readonly object SyncRoot = new object();

        /// <summary>
        /// 元数据中配置的表清单
        /// </summary>
        [XmlElement("Table")]
        public Table[] Tables { get; set; }

        bool cacheInitialized = false;
        private readonly Dictionary<Type, EntityDescriptor> descriptorCache = new Dictionary<Type, EntityDescriptor>();

        void EnsureCacheInitialized()
        {
            if (!cacheInitialized)
            {
                lock (SyncRoot)
                {
                    if (!cacheInitialized)
                    {
                        foreach (EntityDescriptor descriptor in Tables.Select(_ => _.CreateEntityDescriptor()))
                        {
                            descriptorCache[descriptor.EntityType] = descriptor;
                        }
                        cacheInitialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// 创建指定类型的实体描述
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public EntityDescriptor CreateEntityDescriptor(System.Type entityType)
        {
            EnsureCacheInitialized();
            EntityDescriptor descriptor;
            descriptorCache.TryGetValue(entityType, out descriptor);
            return descriptor;
        }

        /// <summary>
        /// 从文件创建
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        static public MetaXml CreateFromFile(string file)
        {
            return MetaXmlManager.Get(file);
        }
    }
}
