using System;
using System.Linq.Expressions;
using Sprache;

namespace LinqyCalculator
{
    static class ExpressionParser
    {
        public static Expression<Func<decimal>> ParseExpression(string text)
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
            (from d in Parse.Decimal.Token()
             select (Expression)Expression.Constant(decimal.Parse(d))).Named("number");

        static readonly Parser<Expression> Factor =
            ((from lparen in Parse.Char('(')
              from expr in Parse.Ref(() => Expr)
              from rparen in Parse.Char(')')
              select expr).Named("expression")
             .XOr(Constant)).Token();

        static readonly Parser<Expression> Term = Parse.ChainOperator(Multiply.Or(Divide), Factor, Expression.MakeBinary);

        static readonly Parser<Expression> Expr = Parse.ChainOperator(Add.Or(Subtract), Term, Expression.MakeBinary);
        
        static readonly Parser<Expression<Func<decimal>>> Lambda =
            Expr.End().Select(body => Expression.Lambda<Func<decimal>>(body));
    }
}
