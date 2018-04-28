
namespace Lotech.Data.Configuration
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    public class DatabaseSettings : System.Configuration.ConfigurationSection
    {
        /// <summary>
        /// 默认数据库连接名称
        /// </summary>
        [System.Configuration.ConfigurationProperty("defaultDatabase", IsRequired = true)]
        public string DefaultDatabase
        {
            get
            {
                return base["defaultDatabase"] as string;
            }
            set
            {
                base["defaultDatabase"] = value;
            }
        }


        /// <summary>
        /// 默认数据库连接名称
        /// </summary>
        [System.Configuration.ConfigurationProperty("parameterPrefix", DefaultValue = ":")]
        public string ParameterPrefix
        {
            get
            {
                return base["parameterPrefix"] as string;
            }
            set
            {
                base["parameterPrefix"] = value;
            }
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        static public DatabaseSettings Current
        {
            get
            {
                return System.Configuration.ConfigurationManager.GetSection("databaseSettings") as DatabaseSettings;
            }
        }
    }
}
