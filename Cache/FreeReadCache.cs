using System;
using System.Collections.Generic;
using System.Threading;

namespace Lotech.Data.Cache {
    /// <summary>
    /// 开放式读取缓存
    /// </summary>
    /// <typeparam name="K">缓存键</typeparam>
    /// <typeparam name="V">缓存值</typeparam>
    public abstract class FreeReadCache<K, V> : IDisposable {
        #region Data Defines
        /// <summary>
        /// 读/写锁
        /// </summary>
        private ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
        /// <summary>
        /// 缓存内容
        /// </summary>
        private Dictionary<K, WeakReference> weakObjectCacheCollection;
        /// <summary>
        /// 强引用集合
        /// </summary>
        private Dictionary<K, V> objectCacheCollection;

        private bool _weakMode;
        #endregion

        #region Constructor
        /// <summary>
        /// 构造弱引用缓存
        /// </summary>
        protected FreeReadCache()
            : this(true) {
        }

        /// <summary>
        /// 构造指定模式缓存
        /// </summary>
        /// <param name="weakMode">是否为弱引用缓存</param>
        public FreeReadCache(bool weakMode) {
            _weakMode = weakMode;
            if (_weakMode)
                weakObjectCacheCollection = new Dictionary<K, WeakReference>();
            else
                objectCacheCollection = new Dictionary<K, V>();
        }
        #endregion

        #region Method
        /// <summary>
        /// 尝试读取
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool TryGet(K cacheName, out V value) {
            if (_weakMode)
                return TryGetWeak(cacheName, out value);
            return TryGetStrong(cacheName, out value);
        }
        #region TryGet
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool TryGetWeak(K cacheName, out V value) {
            WeakReference cacheReference;
            value = default(V);
            if (weakObjectCacheCollection.TryGetValue(cacheName, out cacheReference)) {
                object cacheItem = cacheReference.Target;
                if (cacheReference.IsAlive)
                    value = (V)cacheItem;
            }
            return value != null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool TryGetStrong(K cacheName, out V value) {
            return objectCacheCollection.TryGetValue(cacheName, out value);
        }
        #endregion

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="cacheObject"></param>
        public virtual void Set(K cacheName, V cacheObject) {
            if (_weakMode)
                SetWeak(cacheName, cacheObject);
            else
                SetStrong(cacheName, cacheObject);
        }
        #region Set
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="cacheObject"></param>
        protected virtual void SetWeak(K cacheName, V cacheObject) {
            if (rwLock.TryEnterUpgradeableReadLock(350))
                try {
                    if (!weakObjectCacheCollection.ContainsKey(cacheName))   // 不存在
                    {
                        if (rwLock.TryEnterWriteLock(350))
                            try {
                                weakObjectCacheCollection.Add(cacheName, new WeakReference(cacheObject));
                            }
                            finally {
                                rwLock.ExitWriteLock();
                            }
                    }
                    else if (!weakObjectCacheCollection[cacheName].IsAlive)  // 存在已失效
                    {
                        weakObjectCacheCollection[cacheName].Target = cacheObject;
                    }
                }
                finally {
                    rwLock.ExitUpgradeableReadLock();
                }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <param name="cacheObject"></param>
        protected virtual void SetStrong(K cacheName, V cacheObject) {
            if (rwLock.TryEnterUpgradeableReadLock(350))
                try {
                    if (!objectCacheCollection.ContainsKey(cacheName))   // 不存在
                    {
                        if (rwLock.TryEnterWriteLock(350))
                            try {
                                objectCacheCollection.Add(cacheName, cacheObject);
                            }
                            finally {
                                rwLock.ExitWriteLock();
                            }
                    }
                }
                finally {
                    rwLock.ExitUpgradeableReadLock();
                }
        }
        #endregion
        /// <summary>
        /// 释放
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063")]
        public void Dispose() {
            if (rwLock != null)
                rwLock.Dispose();
            if (weakObjectCacheCollection != null)
                weakObjectCacheCollection.Clear();
            if (objectCacheCollection != null)
                objectCacheCollection.Clear();
        }
        #endregion
    }
}
