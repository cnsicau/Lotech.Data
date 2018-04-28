using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Lotech.Data
{
    /// <summary>
    /// 属性提供扩展
    /// </summary>
    public static class CustomAttributeProviderExtension
    {
        static public IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider provider, bool inherit)
        {
            if (provider == null)
                return Enumerable.Empty<T>();

            return provider.GetCustomAttributes(typeof(T), inherit) as T[];
        }


        static public IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider provider)
        {
            return provider.GetAttributes<T>(true);
        }

        static public T GetAttribute<T>(this ICustomAttributeProvider provider, bool inherit)
        {
            return provider.GetAttributes<T>(inherit).FirstOrDefault();
        }

        static public T GetAttribute<T>(this ICustomAttributeProvider provider)
        {
            return provider.GetAttributes<T>(true).FirstOrDefault();
        }
    }
}
