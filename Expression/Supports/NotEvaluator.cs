using System.Data;
using System.Linq.Expressions;

namespace Lotech.Data.Expression.Supports
{
    class NotEvaluator : ExpressionEvaluator<UnaryExpression>
    {
        protected override EvaluateResult OnEvaluate(ExpressionEvaluateContext context, UnaryExpression expression)
        {
            var operand = EvaluateChild(context, expression.Operand);
            string name = Utility.SequenceParameterUtility.GetNextParameterName();
            return new EvaluateResult(
                string.Format("{0} <> {1}", operand, context.Database.BuildParameterName(name))
                , ResultTypes.Not
                , operand.Parameters
                , new[] { new ExpressionParameter(name, DbType.Boolean, false) });
        }
    }
}
