using System;
using System.Linq.Expressions;

namespace Lotech.Data.Expression
{
    /// <summary>
    /// 提供字符串 Contains, EndsWith, IsNullOrEmpty, StartsWith, ToString 相关处理
    /// </summary>
    public class StringExpressionEvaluatorProvider : ExpressionEvaluator<MethodCallExpression>, IExpressionEvaluator, IExpressionEvaluatorProvider
    {
        private ConcatStrings concat;
        private string isNullOrEmptyFragement;
        private string castStringFragement;
        private Func<EvaluateResult> evaluateCallback;

        /// <summary>
        /// 字符串拼接
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public delegate string ConcatStrings(params string[] args);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="concat"></param>
        /// <param name="isNullOrEmptyFragement"></param>
        /// <param name="castStringFragement"></param>
        public StringExpressionEvaluatorProvider(ConcatStrings concat, string isNullOrEmptyFragement, string castStringFragement)
        {
            this.concat = concat;
            this.isNullOrEmptyFragement = isNullOrEmptyFragement;
            this.castStringFragement = castStringFragement;
        }

        StringExpressionEvaluatorProvider(Func<EvaluateResult> evaluateCallback)
        {
            this.evaluateCallback = evaluateCallback;
        }

        protected override EvaluateResult OnEvaluate(ExpressionEvaluateContext context, MethodCallExpression expression)
        {
            return evaluateCallback();
        }

        IExpressionEvaluator IExpressionEvaluatorProvider.GetExpressionEvaluator(ExpressionEvaluateContext context, System.Linq.Expressions.Expression expr)
        {
            MethodCallExpression expression = expr as MethodCallExpression;

            if (expression == null)
                return null;

            EvaluateResult left, right;
            if (expression.Method.DeclaringType == typeof(string))
            {
                switch (expression.Method.Name)
                {
                    case "Contains":
                        left = EvaluateChild(context, expression.Object);
                        right = EvaluateChild(context, expression.Arguments[0]);
                        return new StringExpressionEvaluatorProvider(() => new Expression.EvaluateResult(
                                string.Format("{0} LIKE ({1})", left.Fragement, concat("'%'", right.Fragement, "'%'"))
                                , Expression.ResultTypes.None
                                , left.Parameters, right.Parameters));
                    case "StartsWith":
                        left = EvaluateChild(context, expression.Object);
                        right = EvaluateChild(context, expression.Arguments[0]);

                        return new StringExpressionEvaluatorProvider(() => new Expression.EvaluateResult(
                                string.Format("{0} LIKE ({1})", left.Fragement, concat(right.Fragement, "'%'"))
                                , Expression.ResultTypes.None
                                , left.Parameters, right.Parameters));
                    case "EndsWith":
                        left = EvaluateChild(context, expression.Object);
                        right = EvaluateChild(context, expression.Arguments[0]);

                        return new StringExpressionEvaluatorProvider(() => new Expression.EvaluateResult(
                                string.Format("{0} LIKE ({1})", left.Fragement, concat("'%'", right.Fragement))
                                , Expression.ResultTypes.None
                                , left.Parameters, right.Parameters));
                    case "IsNullOrEmpty":
                        left = EvaluateChild(context, expression.Arguments[0]);
                        return new StringExpressionEvaluatorProvider(() => new Expression.EvaluateResult(string.Format(isNullOrEmptyFragement, left.Fragement)
                            , Expression.ResultTypes.None, left.Parameters));
                }
            }
            else if (expression.Method.Name == "ToString" && expression.Method.GetParameters().Length == 0)
            {
                left = EvaluateChild(context, expression.Object);
                return new StringExpressionEvaluatorProvider(() => new Expression.EvaluateResult(string.Format(castStringFragement, left.Fragement)
                    , Expression.ResultTypes.None, left.Parameters));
            }
            return null;
        }
    }
}
