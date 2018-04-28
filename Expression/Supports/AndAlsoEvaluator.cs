using System.Linq.Expressions;

namespace Lotech.Data.Expression.Supports
{
    /// <summary>
    ///  AND 表达式
    /// </summary>
    class AndAlsoEvaluator : ExpressionEvaluator<BinaryExpression>
    {
        protected override EvaluateResult OnEvaluate(ExpressionEvaluateContext context, BinaryExpression expression)
        {
            var left = EvaluateChild(context, expression.Left);
            var right = EvaluateChild(context, expression.Right);

            return new EvaluateResult(
                string.Format("({0} AND {1})", left.Fragement, right.Fragement)
                , ResultTypes.AndAlso
                , left.Parameters, right.Parameters);
        }
    }
}
