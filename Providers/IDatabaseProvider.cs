using System.Data.Common;

namespace Lotech.Data.Providers {
    /// <summary>
    /// Database提供者
    /// </summary>
    public interface IDatabaseProvider {
        /// <summary>
        /// 获取Database
        /// </summary>
        /// <param name="dbProviderFactory"></param>
        /// <param name="connectionString"></param>
        /// <returns>无法提供时返回 null，便于容器继续查找</returns>
        IDatabase GetDatabase(DbProviderFactory dbProviderFactory, string connectionString);
        /// <summary>
        /// 获取Database
        /// </summary>
        /// <param name="dbProviderFactory"></param>
        /// <param name="connectionString"></param>
        /// <param name="parametrPrefix">参数前缀，如SQLServer默认为 @</param>
        /// <returns>无法提供时返回 null，便于容器继续查找</returns>
        IDatabase GetDatabase(DbProviderFactory dbProviderFactory, string connectionString, string parametrPrefix);
    }
}
