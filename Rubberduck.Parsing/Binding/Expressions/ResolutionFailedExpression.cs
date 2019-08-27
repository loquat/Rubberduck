﻿using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;

namespace Rubberduck.Parsing.Binding
{
    public sealed class ResolutionFailedExpression : BoundExpression
    {
        private readonly List<IBoundExpression> _successfullyResolvedExpressions = new List<IBoundExpression>();

        public ResolutionFailedExpression(ParserRuleContext context, bool isDefaultMemberResolution = false)
            : base(null, ExpressionClassification.ResolutionFailed, context)
        {
            IsDefaultMemberResolution = isDefaultMemberResolution;
            IsJoinedExpression = false;
        }

        public ResolutionFailedExpression(params ResolutionFailedExpression[] expressions)
            : base(null, ExpressionClassification.ResolutionFailed, null)
        {
            IsDefaultMemberResolution = false;
            IsJoinedExpression = true;

            AddSuccessfullyResolvedExpressions(expressions);
        }

        public IReadOnlyList<IBoundExpression> SuccessfullyResolvedExpressions => _successfullyResolvedExpressions;
        public bool IsDefaultMemberResolution { get; }
        public bool IsJoinedExpression { get; }

        public void AddSuccessfullyResolvedExpression(IBoundExpression expression)
        {
            _successfullyResolvedExpressions.Add(expression);
        }

        public void AddSuccessfullyResolvedExpressions(IEnumerable<IBoundExpression> expressions)
        {
            _successfullyResolvedExpressions.AddRange(expressions);
        }
    }

    public static class ResolutionFailedExpressionExtensions
    {
        public static ResolutionFailedExpression Join(this ResolutionFailedExpression expression, params IBoundExpression[] otherExpressions)
        {
            return expression.Join((IEnumerable<IBoundExpression>)otherExpressions);
        }

        public static ResolutionFailedExpression Join(this ResolutionFailedExpression expression, IEnumerable<IBoundExpression> otherExpressions)
        {
            var otherExprs = otherExpressions.ToList();

            var failedExpressions = otherExprs.OfType<ResolutionFailedExpression>().Concat(new []{expression}).ToArray();
            var failedExpression = new ResolutionFailedExpression(failedExpressions);

            var successfulExpressions = otherExprs.Where(expr => expr.Classification != ExpressionClassification.ResolutionFailed);

            failedExpression.AddSuccessfullyResolvedExpressions(successfulExpressions);

            return failedExpression;
        }
    }
}
