using System.Linq.Expressions;

namespace Lotech.Data.Expression.Supports {
    class ConvertEvaluator : ExpressionEvaluator<UnaryExpression> {
        protected override EvaluateResult OnEvaluate(ExpressionEvaluateContext context, UnaryExpression expression)
        {
            return EvaluateChild(context, expression.Operand);
        }
    }
}
