using System.Data.Common;
namespace Lotech.Data.Oracle
{

    /// <summary>
    /// Oracle 数据库
    /// </summary>
    public class OracleDatabase : DbProviderDatabase, IDatabase, Expression.IExpressionEvaluatorProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public OracleDatabase(DbProviderFactory dbProviderFactory)
            : base(dbProviderFactory) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string BuildParameterName(string name)
        {
            return name.StartsWith(":") ? name : (":" + GetParameterName(name));
        }

        public override string GetParameterName(string name)
        {
            return System.Text.RegularExpressions.Regex.Replace(name, @"^\s*""|""\s*$", string.Empty);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        Expression.IExpressionEvaluator Expression.IExpressionEvaluatorProvider.GetExpressionEvaluator(
            Expression.ExpressionEvaluateContext context, System.Linq.Expressions.Expression expression)
        {
            Expression.IExpressionEvaluatorProvider stringExpressionEvaluatorProvider
                = new Expression.StringExpressionEvaluatorProvider(args => string.Join(" || ", args)
                    , "{0} IS NULL", "CAST({0} AS VARCHAR(8000))");
            return stringExpressionEvaluatorProvider.GetExpressionEvaluator(context, expression);
        }
    }
}
