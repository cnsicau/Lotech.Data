using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace Lotech.Data.Meta
{
    static class MetaXmlManager
    {
        /// <summary>
        /// 元数据缓存
        /// </summary>
        class MetaCache : Cache.FreeReadCache<string, MetaXml> { }

        static readonly MetaCache cache = new MetaCache();

        /// <summary>
        /// 从文件中解析配置，调用一次解析一次，未缓存此结果
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        static MetaXml Parse(string file)
        {
            using (Stream reader = File.OpenRead(file))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MetaXml));
                return (MetaXml)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// 从文件中获取配置，启用了缓存，如果结果有变化，也将自动加载
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        static public MetaXml Get(string file)
        {
            MetaXml meta;
            if (!cache.TryGet(file, out meta))
            {
                meta = Parse(file);
                FileSystemWatcher watcher = new FileSystemWatcher(
                    Path.GetDirectoryName(file), Path.GetFileName(file));
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Changed += (s, e) => cache.Set(file, Parse(file));
                new Thread(() => watcher.WaitForChanged(WatcherChangeTypes.Changed)) { IsBackground = true }.Start();

                cache.Set(file, meta);
            }
            return meta;
        }
    }
}
