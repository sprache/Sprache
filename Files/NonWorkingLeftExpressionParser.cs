using System;
using System.Linq.Expressions;
using Sprache;

namespace LinqyCalculator
{
    static class NonWorkingLeftExpressionParser
    {
        public static Expression<Func<decimal>> ParseExpression(string text)
        {
            return Expr.Parse(text);
        }

        static Parser<ExpressionType> Operator(string op, ExpressionType opType)
        {
            return Parse.String(op).Token().Return(opType);
        }

        static readonly Parser<ExpressionType> Add = Operator("+", ExpressionType.AddChecked);
        static readonly Parser<ExpressionType> Subtract = Operator("-", ExpressionType.SubtractChecked);
        static readonly Parser<ExpressionType> Multiply = Operator("*", ExpressionType.MultiplyChecked);
        static readonly Parser<ExpressionType> Divide = Operator("/", ExpressionType.Divide);

        static readonly Parser<Expression> Decimal =
            from d in Parse.Decimal.Token()
            select (Expression)Expression.Constant(decimal.Parse(d));

        static readonly Parser<Expression> Factor =
            (from lparen in Parse.Char('(')
             from expr in Parse.Ref(() => Sum)
             from rparen in Parse.Char(')')
             select expr)
            .XOr(Decimal).Token();

        static readonly Parser<Expression> Term =
            (from t in Parse.Ref(() => Term)
             from op in Multiply.XOr(Divide)
             from f in Factor
             select (Expression)Expression.MakeBinary(op, t, f)).Try()
            .Or(Factor).Token();

        static readonly Parser<Expression> Sum =
            (from e in Parse.Ref(() => Sum)
             from op in Add.XOr(Subtract)
             from t in Factor
             select (Expression)Expression.MakeBinary(op, e, t)).Try()
            .XOr(Factor).Token();

        static readonly Parser<Expression<Func<decimal>>> Expr =
            Sum.End().Select(r => Expression.Lambda<Func<decimal>>(r));
    }
}
