using System.Data;

namespace Lotech.Data.Expression
{
    /// <summary>
    /// 表达式参数
    /// </summary>
    public sealed class ExpressionParameter
    {
        /// <summary>
        /// 
        /// </summary>
        public ExpressionParameter() { }

        /// <summary>
        /// 
        /// </summary>
        public ExpressionParameter(string name, DbType dbType, object value)
        {
            this.Name = name;
            this.DbType = dbType;
            this.Value = value;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DbType DbType { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 参数大小
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}[{1}]:{2}", Name, DbType, Value);
        }
    }
}
