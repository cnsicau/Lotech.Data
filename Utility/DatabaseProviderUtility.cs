using System;
using System.Collections.Generic;
using Lotech.Data.Providers;

namespace Lotech.Data.Utility {
    static class DatabaseProviderUtility {
        /// <summary>
        /// 探测Database 提供者
        /// </summary>
        /// <returns></returns>
        static internal List<IDatabaseProvider> DetectDatabaseProviders() {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<IDatabaseProvider> providers = new List<IDatabaseProvider>();
            foreach (var assembly in assemblies) {
                try {
                    var types = assembly.GetTypes();
                    foreach (var type in types) {
                        try {
                            if (type.IsClass && !type.IsAbstract && type.IsPublic/*仅自动探测公共类型*/
                                && typeof(IDatabaseProvider).IsAssignableFrom(type)) {
                                providers.Add((IDatabaseProvider)Activator.CreateInstance(type));
                            }
                        }
                        catch {
                        }
                    }
                }
                catch {
                }
            }
            return providers;
        }
    }
}
