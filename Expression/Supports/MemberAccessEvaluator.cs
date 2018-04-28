using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using Lotech.Data.Descriptor;

namespace Lotech.Data.Expression.Supports
{
    /// <summary>
    /// 成员访问
    /// </summary>
    class MemberAccessEvaluator : ExpressionEvaluator<MemberExpression>
    {
        protected override EvaluateResult OnEvaluate(ExpressionEvaluateContext context, MemberExpression expression)
        {
            if (expression.Expression != null && expression.Expression.NodeType == ExpressionType.Parameter)
            {
                if (expression.Member.MemberType != MemberTypes.Property)
                {
                    throw new InvalidProgramException("Property required, but " + expression.Member.MemberType);
                }

                Type entityType = expression.Member.DeclaringType;
                FieldDescriptor descriptor = context.DescriptionProvider.CreateFieldDescriptor(expression.Member as PropertyInfo);
                if (descriptor == null)
                    throw new InvalidProgramException("Invalid Field: " + expression.Member);
                return new EvaluateResult(descriptor.Name, ResultTypes.PrameterMember) { Tag = descriptor };
            }
            else if (expression.Expression == null)
            {
                return EvaluateExternalStaticMemberAccess(context, expression);
            }
            else
            {
                return EvaluateExternalMemberAccess(context, expression);
            }
        }

        static EvaluateResult EvaluateExternalStaticMemberAccess(ExpressionEvaluateContext context, MemberExpression memberExpression)
        {
            object value = memberExpression.Type.InvokeMember(memberExpression.Member.Name
                , BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
                    | BindingFlags.GetField | BindingFlags.GetProperty, null, null, null);
            return MergeExternalResult(context, context.Database.ParseDbType(memberExpression.Type), value);
        }

        static EvaluateResult EvaluateExternalMemberAccess(ExpressionEvaluateContext context, MemberExpression memberExpression)
        {
            var expressions = new Stack<System.Linq.Expressions.Expression>();
            System.Linq.Expressions.Expression expression = memberExpression;
            while (expression != null && expression.NodeType != ExpressionType.Constant)
            {
                if (expression.NodeType == ExpressionType.MemberAccess)
                {
                    expressions.Push(expression);
                    expression = ((MemberExpression)expression).Expression;
                }
            }
            object value = ((ConstantExpression)expression).Value;
            var type = ((ConstantExpression)expression).Type;
            while (expressions.Count > 0)
            {
                expression = expressions.Pop();
                if (value == null)
                    throw new ArgumentException(expression.ToString());
                EvaluateMemberAccess((MemberExpression)expression, ref value, ref type);
            }
            return MergeExternalResult(context, context.Database.ParseDbType(type), value);
        }

        static void EvaluateMemberAccess(MemberExpression expression, ref object value, ref Type type)
        {
            value = type.InvokeMember(expression.Member.Name
                , BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                    | BindingFlags.GetField | BindingFlags.GetProperty
                , null, value, null);
            type = expression.Type;
        }

        static EvaluateResult MergeExternalResult(ExpressionEvaluateContext context, DbType expressionType, object value)
        {
            string parameterName = Utility.SequenceParameterUtility.GetNextParameterName();
            return new EvaluateResult(context.Database.BuildParameterName(parameterName)
                , ResultTypes.Constants
                , new[] { new ExpressionParameter(parameterName, expressionType, value) });
        }
    }
}
