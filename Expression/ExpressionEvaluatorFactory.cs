using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Lotech.Data.Expression
{
    /// <summary>
    /// 工厂方法
    /// </summary>
    public class ExpressionEvaluatorFactory : IExpressionEvaluatorProvider
    {
        static readonly IExpressionEvaluatorProvider defaultProvider = new ExpressionEvaluatorFactory();

        /// <summary>创建查询表达式处理工具</summary>
        static public IExpressionEvaluator Create(Expression.ExpressionEvaluateContext context
            , System.Linq.Expressions.Expression expression)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            var providers = ConstructEvaluatorProviders(context);
            foreach (var provider in providers)
            {
                var evaluator = provider.GetExpressionEvaluator(context, expression);
                if (evaluator != null)
                    return evaluator;
            }
            throw new InvalidProgramException("Unsupported expression: " + expression);
        }

        /// <summary>
        /// 构造Providers
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        static IEnumerable<IExpressionEvaluatorProvider> ConstructEvaluatorProviders(Expression.ExpressionEvaluateContext context)
        {
            List<IExpressionEvaluatorProvider> providers = new List<IExpressionEvaluatorProvider>();
            if (context.Database is IExpressionEvaluatorProvider)
                providers.Add((IExpressionEvaluatorProvider)context.Database);

            if (context.DescriptionProvider is IExpressionEvaluatorProvider)
                providers.Add((IExpressionEvaluatorProvider)context.DescriptionProvider);

            if (context is IExpressionEvaluatorProvider)
                providers.Add((IExpressionEvaluatorProvider)context);

            providers.Add(defaultProvider);
            return providers.ToArray();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        IExpressionEvaluator IExpressionEvaluatorProvider.GetExpressionEvaluator(
            ExpressionEvaluateContext context, System.Linq.Expressions.Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            IExpressionEvaluator evaluator;
            switch (expression.NodeType)
            {
                case ExpressionType.AndAlso:
                    evaluator = new Supports.AndAlsoEvaluator();
                    break;
                case ExpressionType.Constant:
                    evaluator = new Supports.ConstantEvaluator();
                    break;
                case ExpressionType.Convert:
                    evaluator = new Supports.ConvertEvaluator();
                    break;
                case ExpressionType.Equal:
                    evaluator = new Supports.EqualEvaluator();
                    break;
                case ExpressionType.GreaterThan:
                    evaluator = new Supports.GreaterThanEvaluator();
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    evaluator = new Supports.GreaterThanOrEqualEvaluator();
                    break;
                case ExpressionType.LessThan:
                    evaluator = new Supports.LessThanEvaluator();
                    break;
                case ExpressionType.LessThanOrEqual:
                    evaluator = new Supports.LessThanOrEqualEvaluator();
                    break;
                case ExpressionType.MemberAccess:
                    evaluator = new Supports.MemberAccessEvaluator();
                    break;
                case ExpressionType.Not:
                    evaluator = new Supports.NotEvaluator();
                    break;
                case ExpressionType.NotEqual:
                    evaluator = new Supports.NotEqualEvaluator();
                    break;
                case ExpressionType.OrElse:
                    evaluator = new Supports.OrElseEvaluator();
                    break;
                default:
                    return null;
            }
            return evaluator;
        }
    }
}
