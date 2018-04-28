using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Lotech.Data.Expression
{
    /// <summary>
    /// 表达式参数集合
    /// </summary>
    public class ExpressionParameterCollection : List<ExpressionParameter>, IEnumerable<ExpressionParameter>
    {
        /// <summary>
        /// 添加单一参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="size"></param>
        public void Add(string name, DbType type, object value, int size)
        {
            this.Add(new ExpressionParameter
            {
                Name = name,
                DbType = type,
                Value = value,
                Size = size
            });
        }
        /// <summary>
        /// 添加单一参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void Add(string name, DbType type, object value)
        {
            this.Add(new ExpressionParameter
            {
                Name = name,
                DbType = type,
                Value = value
            });
        }
        /// <summary>
        /// 添加单一参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Add(string name, object value)
        {
            this.Add(new ExpressionParameter
            {
                Name = name,
                Value = value
            });
        }
        /// <summary>
        /// 添加集合
        /// </summary>
        /// <param name="parameters"></param>
        public void AddRange(params ExpressionParameter[] parameters)
        {
            this.AddRange((IEnumerable<ExpressionParameter>)parameters);
        }
    }
}
