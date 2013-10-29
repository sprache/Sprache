using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Sprache;

namespace LinqyCalculator
{
    static class LangParser
    {
        public static readonly Parser<double> Line = VariableParser.VarDec.XOr(ExpressionParser.AnyExpression);
    }

    static class VariableParser
    {
        public static Parser<double> VarDec =
            from word in Parse.String("var")
            from w1 in Parse.WhiteSpace.Many()
            from name in Parse.Letter.AtLeastOnce().Text()
            from w2 in Parse.WhiteSpace.Many()
            from equ in Parse.String("=")
            from w3 in Parse.WhiteSpace.Many()
            from expr in ExpressionParser.Lambda
            select expr.AddNumVar(name);

        public static IDictionary<string, Expression<Func<double>>> NumVars = 
            new Dictionary<string, Expression<Func<double>>>();

        public static double AddNumVar(this Expression<Func<double>> expr, string name)
        {
            NumVars.Add(name, expr);
            return Convert.ToDouble(expr.Compile()());
        }
    }

    static class ExpressionParser
    {
        static Parser<ExpressionType> Operator(string op, ExpressionType opType)
        {
            return Parse.String(op).Token().Return(opType);
        }

        static readonly Parser<ExpressionType> Add = Operator("+", ExpressionType.AddChecked);
        static readonly Parser<ExpressionType> Subtract = Operator("-", ExpressionType.SubtractChecked);
        static readonly Parser<ExpressionType> Multiply = Operator("*", ExpressionType.MultiplyChecked);
        static readonly Parser<ExpressionType> Divide = Operator("/", ExpressionType.Divide);
        static readonly Parser<ExpressionType> Modulo = Operator("%", ExpressionType.Modulo);
        static readonly Parser<ExpressionType> Power = Operator("^", ExpressionType.Power);

        static readonly Parser<Expression> Variable =
            from name in Parse.Letter.AtLeastOnce().Token().Text()
            select Expression.Constant(GetVar(name).Compile()());

        static Expression<Func<double>> GetVar(string name)
        {
            if (VariableParser.NumVars.ContainsKey(name))
            {
                Expression<Func<double>> toReturn = null;
                VariableParser.NumVars.TryGetValue(name, out toReturn);
                return toReturn;
            }
            else throw new ParseException(string.Format("Variable {0} does not exist.", name));
        }

        static readonly Parser<Expression> Function =
            from name in Parse.Letter.AtLeastOnce().Text()
            from lparen in Parse.Char('(')
            from expr in Parse.Ref(() => Expr).DelimitedBy(Parse.Char(',').Token())
            from rparen in Parse.Char(')')
            select CallFunction(name, expr.ToArray());

        static Expression CallFunction(string name, Expression[] parameters)
        {
            var methodInfo = typeof(Math).GetMethod(name, parameters.Select(e => e.Type).ToArray());
            if (methodInfo == null)
                throw new ParseException(string.Format("Function '{0}({1})' does not exist.", name,
                                                       string.Join(",", parameters.Select(e => e.Type.Name))));

            return Expression.Call(methodInfo, parameters);
        }

        static readonly Parser<Expression> Constant =
             Parse.Decimal
             .Select(x => Expression.Constant(double.Parse(x)))
             .Named("number");

        static readonly Parser<Expression> Factor =
            (from lparen in Parse.Char('(')
             from expr in Parse.Ref(() => Expr)
             from rparen in Parse.Char(')')
             select expr).Named("expression")
             .XOr(Constant)
             .XOr(Function)
             .XOr(Variable);

        static readonly Parser<Expression> Operand =
            ((from sign in Parse.Char('-')
              from factor in Factor
              select Expression.Negate(factor)
             ).XOr(Factor)).Token();

        static readonly Parser<Expression> InnerTerm = Parse.ChainOperator(Power, Operand, Expression.MakeBinary);

        static readonly Parser<Expression> Term = Parse.ChainOperator(Multiply.Or(Divide).Or(Modulo), InnerTerm, Expression.MakeBinary);

        public static readonly Parser<Expression> Expr = Parse.ChainOperator(Add.Or(Subtract), Term, Expression.MakeBinary);

        public static readonly Parser<Expression<Func<double>>> Lambda =
            from body in Expr.End() //.Select(body => Expression.Lambda<Func<double>>(body));
            select Expression.Lambda<Func<double>>(body);

        public static readonly Parser<double> AnyExpression =
            from body in Lambda
            select body.Compile()();
    }
}
