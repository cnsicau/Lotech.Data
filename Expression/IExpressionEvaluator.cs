using System;
using System.Collections.Generic;
namespace Lotech.Data.Expression
{
    /// <summary>
    /// 结果类型
    /// </summary>
    public enum ResultTypes
    {
        /// <summary>
        /// 
        /// </summary>
        AndAlso = 0,
        /// <summary>
        /// 
        /// </summary>
        Constants,
        /// <summary>
        /// 
        /// </summary>
        Equal,
        /// <summary>
        /// 
        /// </summary>
        GreaterThan,
        /// <summary>
        /// 
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// 
        /// </summary>
        LessThan,
        /// <summary>
        /// 
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// 
        /// </summary>
        NotEqual,
        /// <summary>
        /// 
        /// </summary>
        Not,
        /// <summary>
        /// 
        /// </summary>
        OrElse,
        /// <summary>
        /// 
        /// </summary>
        PrameterMember,
        /// <summary>
        /// 
        /// </summary>
        None,
    }
    /// <summary>
    /// 计算结果
    /// </summary>
    public class EvaluateResult
    {
        /// <summary>
        /// 
        /// </summary>
        public EvaluateResult() { Parameters = new ExpressionParameterCollection(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fragement"></param>
        public EvaluateResult(string fragement) : this() { this.Fragement = fragement; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resultType"></param>
        public EvaluateResult(ResultTypes resultType) : this() { this.ResultType = resultType; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fragement"></param>
        /// <param name="resultType"></param>
        public EvaluateResult(string fragement, ResultTypes resultType) : this(fragement) { this.ResultType = resultType; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fragement"></param>
        /// <param name="resultType"></param>
        /// <param name="parameters"></param>
        public EvaluateResult(string fragement, ResultTypes resultType
            , params IEnumerable<ExpressionParameter>[] parameters)
            : this(fragement, resultType)
        {
            if (parameters != null && parameters.Length > 0)
                Array.ForEach(parameters, _ => ((ExpressionParameterCollection)Parameters).AddRange(_));
        }

        /// <summary>SQL片断</summary>
        public string Fragement { get; set; }
        /// <summary>结果类型（如字段引用、常量等）</summary>
        public ResultTypes ResultType { get; set; }
        /// <summary>参数集</summary>
        public ExpressionParameterCollection Parameters { get; private set; }
        /// <summary>Tag值</summary>
        public object Tag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Fragement;
        }
    }

    /// <summary>
    /// 表达式计算
    /// </summary>
    public interface IExpressionEvaluator
    {
        /// <summary>
        /// 计算表达式
        /// </summary>
        /// <param name="context">计算的上下文</param>
        /// <param name="expression"></param>
        EvaluateResult Evaluate(ExpressionEvaluateContext context, System.Linq.Expressions.Expression expression);
    }
}
