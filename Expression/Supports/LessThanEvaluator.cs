using System.Linq.Expressions;

namespace Lotech.Data.Expression.Supports {

    class LessThanEvaluator : ExpressionEvaluator<BinaryExpression>
    {
        protected override EvaluateResult OnEvaluate(ExpressionEvaluateContext context, BinaryExpression expression)
        {
            var left = EvaluateChild(context, expression.Left);
            var right = EvaluateChild(context, expression.Right);
            ShareColumnDbType(left, right);
            return new EvaluateResult(
                string.Format("{0} < {1}", left.Fragement, right.Fragement)
                , ResultTypes.LessThan
                , left.Parameters, right.Parameters);
        }
    }
}
