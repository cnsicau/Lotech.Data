using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Lotech.Data.Attributes;
using Lotech.Data.Expression;
using Lotech.Data.Utility;

namespace Lotech.Data
{
    /// <summary>
    /// 更新字段清单 Set Columns = Values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UpdateSetClusters<T>
    {
        class UpdateSetExpressionEvaluatorContext : ExpressionEvaluateContext, IExpressionEvaluatorProvider, IExpressionEvaluator
        {
            public UpdateSetExpressionEvaluatorContext(IDatabase database, Providers.IDescriptorProvider descriptorProvider)
                : base(database, descriptorProvider) { }

            public UpdateSetExpressionEvaluatorContext(ExpressionEvaluateContext context)
                : base(context.Database, context.DescriptionProvider) { }

            IExpressionEvaluator IExpressionEvaluatorProvider.GetExpressionEvaluator(ExpressionEvaluateContext context, System.Linq.Expressions.Expression expression)
            {
                if (context is UpdateSetExpressionEvaluatorContext
                    && expression.NodeType == ExpressionType.Equal)
                {
                    return this;
                }
                return null;
            }

            EvaluateResult IExpressionEvaluator.Evaluate(ExpressionEvaluateContext context, System.Linq.Expressions.Expression expression)
            {
                if (expression.NodeType == ExpressionType.Equal)
                {
                    BinaryExpression binaryExpression = expression as BinaryExpression;

                    var left = ExpressionEvaluatorFactory.Create(context, binaryExpression.Left).Evaluate(context, binaryExpression.Left);
                    var right = ExpressionEvaluatorFactory.Create(context, binaryExpression.Right).Evaluate(context, binaryExpression.Right);
                    Expression.ExpressionEvaluator<System.Linq.Expressions.Expression>.ShareColumnDbType(left, right);
                    return new EvaluateResult(
                        string.Format("{0} = {1}", left.Fragement, right.Fragement)
                        , ResultTypes.None, left.Parameters, right.Parameters);
                }
                throw new InvalidProgramException(expression.ToString());
            }
        }

        List<System.Linq.Expressions.Expression> expressions = new List<System.Linq.Expressions.Expression>();

        /// <summary>
        /// 添加一个 Set  项
        /// </summary>
        /// <typeparam name="CT"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public UpdateSetClusters<T> Set<CT>(Expression<Func<T, CT>> field, CT value)
        {
            if (field.Body.NodeType != ExpressionType.MemberAccess)
                throw new InvalidOperationException("field can only access member/field");

            var member = field.Body as MemberExpression;
            if (member.Expression.NodeType != ExpressionType.Parameter)
                throw new InvalidOperationException("field can only access parameter member/field");

            var fieldAttribute = member.Member.GetAttribute<FieldAttribute>();
            var fieldName = fieldAttribute == null ? member.Member.Name : fieldAttribute.Name;

            expressions.Add(System.Linq.Expressions.Expression.Equal(
                field.Body, System.Linq.Expressions.Expression.Constant(value, typeof(CT))
            ));

            return this;
        }

        /// <summary>
        /// 生成更新片断
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Expression.EvaluateResult Evaluate(Expression.ExpressionEvaluateContext context)
        {
            Expression.EvaluateResult ret = new Expression.EvaluateResult();
            context = new UpdateSetExpressionEvaluatorContext(context);
            bool concated = false;
            expressions.ForEach(exp =>
            {
                var evaluator = Expression.ExpressionEvaluatorFactory.Create(context, exp);
                var itemResult = evaluator.Evaluate(context, exp);
                if (concated)
                {
                    ret.Fragement += ", " + itemResult.Fragement;
                }
                else
                {
                    concated = true;
                    ret.Fragement = itemResult.Fragement;
                }
                ret.Parameters.AddRange(itemResult.Parameters);
            });

            return ret;
        }
    }
}
