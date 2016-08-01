﻿using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace Sprache.Tests.Scenarios
{
    [TestFixture]
    public class ExpressionGrammarTests
    {
        [Test]
        public void DroppedClosingParenthesisProducesMeaningfulError()
        {
            const string input = "1 + (2 * 3";
            var x = Assert.Throws<ParseException>(() => ExpressionParser.ParseExpression(input));
            Assert.That(x.Message, Does.Contain("expected )"));
        }

        [Test]
        public void MissingOperandProducesMeaningfulError()
        {
            const string input = "1 + * 3";
            var x = Assert.Throws<ParseException>(() => ExpressionParser.ParseExpression(input));
            Assert.That(x.Message, Does.Not.Contain("expected end of input"));
        }    
    }

    static class ExpressionParser
    {
        public static Expression<Func<double>> ParseExpression(string text)
        {
            return Lambda.Parse(text);
        }

        static Parser<ExpressionType> Operator(string op, ExpressionType opType)
        {
            return Parse.String(op).Token().Return(opType);
        }

        static readonly Parser<ExpressionType> Add = Operator("+", ExpressionType.AddChecked);
        static readonly Parser<ExpressionType> Subtract = Operator("-", ExpressionType.SubtractChecked);
        static readonly Parser<ExpressionType> Multiply = Operator("*", ExpressionType.MultiplyChecked);
        static readonly Parser<ExpressionType> Divide = Operator("/", ExpressionType.Divide);

        static readonly Parser<Expression> Constant =
             Parse.Decimal
             .Select(x => Expression.Constant(double.Parse(x)))
             .Named("number");

        static readonly Parser<Expression> Factor =
            (from lparen in Parse.Char('(')
             from expr in Parse.Ref(() => Expr)
             from rparen in Parse.Char(')')
             select expr).Named("expression")
             .XOr(Constant);

        static readonly Parser<Expression> Operand =
            ((from sign in Parse.Char('-')
              from factor in Factor
              select Expression.Negate(factor)
             ).XOr(Factor)).Token();

        static readonly Parser<Expression> Term = Parse.XChainOperator(Multiply.XOr(Divide), Operand, Expression.MakeBinary);

        static readonly Parser<Expression> Expr = Parse.XChainOperator(Add.XOr(Subtract), Term, Expression.MakeBinary);

        static readonly Parser<Expression<Func<double>>> Lambda =
            Expr.End().Select(body => Expression.Lambda<Func<double>>(body));
    }
}
