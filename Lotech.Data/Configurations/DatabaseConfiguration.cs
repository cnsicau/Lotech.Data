using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.Common;

namespace Lotech.Data.Configurations
{
    /// <summary>
    /// 库配置
    /// </summary>
    class DatabaseConfiguration
    {
        /// <summary>
        /// 库设置配置节名称
        /// </summary>
        internal const string DatabaseSettingsName = "databaseSettings";
        
        /// <summary>
        /// 连接串配置节名称
        /// </summary>
        internal const string ConnectionStringsName = "connectionStrings";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public DatabaseConfiguration(string fileName)
        {
            FileName = fileName;

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(fileName, false, false)
                .Build();

            var connections = new ConnectionStringSettingsCollection();
            configuration.GetSection(ConnectionStringsName).Bind(connections);
            ConnectionStrings = connections;

            var settings = new DatabaseSettings();
            configuration.GetSection(DatabaseSettingsName).Bind(settings);
            DatabaseSettings = settings;

            Configuration = configuration;
        }

        /// <summary>
        /// 连接串设置
        /// </summary>
        public ConnectionStringSettingsCollection ConnectionStrings { get; }

        /// <summary>
        /// 库设置
        /// </summary>
        public DatabaseSettings DatabaseSettings { get; }

        /// <summary>
        /// 配置
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 配置文件名称
        /// </summary>
        public string FileName { get; }
    }

}
