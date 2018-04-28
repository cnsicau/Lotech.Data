﻿using System;
using System.Linq.Expressions;

namespace Lotech.Data.Expression.Supports
{
    //[System.Diagnostics.DebuggerStepThrough]
    class EqualEvaluator : ExpressionEvaluator<BinaryExpression>
    {
        protected override EvaluateResult OnEvaluate(ExpressionEvaluateContext context, BinaryExpression expression)
        {
            bool leftIsNull = IsNullEqual(expression.Left);
            bool rightIsNull = IsNullEqual(expression.Right);
            EvaluateResult result = new EvaluateResult(ResultTypes.Equal);
            if (!leftIsNull && !rightIsNull)
            {
                var left = EvaluateChild(context, expression.Left);
                var right = EvaluateChild(context, expression.Right);
                ShareColumnDbType(left, right);
                result.Fragement = string.Format("{0} = {1}", left.Fragement, right.Fragement);
                result.Parameters.AddRange(left.Parameters);
                result.Parameters.AddRange(right.Parameters);
            }
            else if (leftIsNull && rightIsNull)
            {
                result.Fragement = "NULL IS NULL";
            }
            else
            {
                var exp = EvaluateChild(context, leftIsNull ? expression.Right : expression.Left);
                result.Parameters.AddRange(exp.Parameters);
                result.Fragement = string.Format("{0} IS NULL", exp.Fragement);
            }
            return result;
        }
    }
}