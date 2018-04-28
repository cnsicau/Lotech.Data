
namespace Lotech.Data.Expression
{
    /// <summary>
    /// 表达式计算器提供者
    /// </summary>
    public interface IExpressionEvaluatorProvider
    {
        /// <summary>
        /// 获取满足此上下文的表达式计算器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="expression"></param>
        /// <returns>无法处理的返回null</returns>
        IExpressionEvaluator GetExpressionEvaluator(ExpressionEvaluateContext context, System.Linq.Expressions.Expression expression);
    }
}
