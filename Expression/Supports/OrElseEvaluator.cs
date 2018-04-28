using System.Linq.Expressions;

namespace Lotech.Data.Expression.Supports {
    class OrElseEvaluator : ExpressionEvaluator<BinaryExpression>
    {
        protected override EvaluateResult OnEvaluate(ExpressionEvaluateContext context, BinaryExpression expression)
        {
            var left = EvaluateChild(context, expression.Left);
            var right = EvaluateChild(context, expression.Right);

            return new EvaluateResult(
                string.Format("({0} OR {1})", left.Fragement, right.Fragement)
                , ResultTypes.OrElse
                , left.Parameters, right.Parameters);
        }
    }
}
