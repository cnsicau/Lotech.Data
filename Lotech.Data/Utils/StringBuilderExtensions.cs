using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Text
{
    static class StringBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="seperator"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        static public StringBuilder AppendJoin<T>(this StringBuilder builder, string seperator, IEnumerable<T> values)
        {
            return builder.Append(string.Join(seperator, values));
        }
    }
}
