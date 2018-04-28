
using System.Linq.Expressions;
using System;
namespace Lotech.Data.Expression
{
    /// <summary>
    /// 查询表达式求值
    /// </summary>
    public abstract class ExpressionEvaluator<TExpression> : Lotech.Data.Expression.IExpressionEvaluator
        where TExpression : System.Linq.Expressions.Expression
    {

        /// <summary>
        /// 计算子表达式
        /// </summary>
        /// <param name="context"></param>
        /// <param name="childExpression"></param>
        /// <returns>返回子表达式的SQL片断</returns>
        protected EvaluateResult EvaluateChild(ExpressionEvaluateContext context, System.Linq.Expressions.Expression childExpression)
        {
            var childEvaluator = ExpressionEvaluatorFactory.Create(context, childExpression);
            return childEvaluator.Evaluate(context, childExpression);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        EvaluateResult IExpressionEvaluator.Evaluate(ExpressionEvaluateContext context, System.Linq.Expressions.Expression expression)
        {
            if (context == null)
                throw new System.ArgumentNullException("context");
            return OnEvaluate(context, (TExpression)expression);
        }

        /// <summary>
        /// 实现计算
        /// </summary>
        /// <param name="context"></param>
        /// <param name="expression"></param>
        protected abstract EvaluateResult OnEvaluate(ExpressionEvaluateContext context, TExpression expression);

        #region Static Members

        /// <summary>
        /// 是否null相等
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static bool IsNullEqual(System.Linq.Expressions.Expression expression)
        {
            if (expression is ConstantExpression)
            {
                if (((ConstantExpression)expression).Value == null
                    || ((ConstantExpression)expression).Value == DBNull.Value)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 若两表达式存在一常量、一字段的情况，则自适应常量值
        /// </summary>
        internal static void ShareColumnDbType(EvaluateResult left, EvaluateResult right)
        {
            if (left.ResultType == ResultTypes.PrameterMember && right.ResultType == ResultTypes.Constants)
            {
                var descriptor = left.Tag as Descriptor.FieldDescriptor;
                right.Parameters.ForEach(p => p.DbType = descriptor.DbType);
            }
            else if (left.ResultType == ResultTypes.Constants && right.ResultType == ResultTypes.PrameterMember)
            {
                var descriptor = right.Tag as Descriptor.FieldDescriptor;
                left.Parameters.ForEach(p => p.DbType = descriptor.DbType);
            }
        }
        #endregion
    }
}
