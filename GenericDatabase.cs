using System.Data.Common;

namespace Lotech.Data
{
    /// <summary>
    /// 通用Database实现
    /// </summary>
    public class GenericDatabase : DbProviderDatabase
    {
        private string _parameterPrefix;

        /// <summary>
        /// 构造通用库
        /// </summary>
        /// <param name="dbProviderFactory">DbProvider实例</param>
        /// <param name="parameterPrefix">参数前缀</param>
        public GenericDatabase(DbProviderFactory dbProviderFactory
            , string parameterPrefix)
            : base(dbProviderFactory) { _parameterPrefix = parameterPrefix; }

        /// <summary>
        /// 创建参数名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string BuildParameterName(string name)
        {
            return string.Concat(_parameterPrefix, name);
        }
    }
}
