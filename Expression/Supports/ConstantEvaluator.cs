using System.Linq.Expressions;

namespace Lotech.Data.Expression.Supports
{
    class ConstantEvaluator : ExpressionEvaluator<ConstantExpression>
    {
        protected override EvaluateResult OnEvaluate(ExpressionEvaluateContext context, ConstantExpression expression)
        {
            var name = Utility.SequenceParameterUtility.GetNextParameterName();
            var result = new EvaluateResult(context.Database.BuildParameterName(name), ResultTypes.Constants);
            result.Parameters.Add(name, context.Database.ParseDbType(expression.Type), expression.Value);
            return result;
        }
    }
}
