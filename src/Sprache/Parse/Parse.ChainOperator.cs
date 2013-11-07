using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sprache
{
    partial class Parse
    {
        /// <summary>
        /// Chain a left-associative operator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOp"></typeparam>
        /// <param name="op"></param>
        /// <param name="operand"></param>
        /// <param name="apply"></param>
        /// <returns></returns>
        public static Parser<T> ChainOperator<T, TOp>(
            Parser<TOp> op,
            Parser<T> operand,
            Func<TOp, T, T, T> apply)
        {
            if (op == null) throw new ArgumentNullException("op");
            if (operand == null) throw new ArgumentNullException("operand");
            if (apply == null) throw new ArgumentNullException("apply");
            return operand.Then(first => ChainOperatorRest(first, op, operand, apply, Or));
        }

        /// <summary>
        /// Chain a left-associative operator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOp"></typeparam>
        /// <param name="op"></param>
        /// <param name="operand"></param>
        /// <param name="apply"></param>
        /// <returns></returns>
        public static Parser<T> XChainOperator<T, TOp>(
            Parser<TOp> op,
            Parser<T> operand,
            Func<TOp, T, T, T> apply)
        {
            if (op == null) throw new ArgumentNullException("op");
            if (operand == null) throw new ArgumentNullException("operand");
            if (apply == null) throw new ArgumentNullException("apply");
            return operand.Then(first => ChainOperatorRest(first, op, operand, apply, XOr));
        }

        static Parser<T> ChainOperatorRest<T, TOp>(
            T firstOperand,
            Parser<TOp> op,
            Parser<T> operand,
            Func<TOp, T, T, T> apply,
            Func<Parser<T>, Parser<T>, Parser<T>> or)
        {
            if (op == null) throw new ArgumentNullException("op");
            if (operand == null) throw new ArgumentNullException("operand");
            if (apply == null) throw new ArgumentNullException("apply");
            return or(op.Then(opvalue =>
                          operand.Then(operandValue =>
                              ChainOperatorRest(apply(opvalue, firstOperand, operandValue), op, operand, apply, or))),
                      Return(firstOperand));
        }

        /// <summary>
        /// Chain a right-associative operator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOp"></typeparam>
        /// <param name="op"></param>
        /// <param name="operand"></param>
        /// <param name="apply"></param>
        /// <returns></returns>
        public static Parser<T> ChainRightOperator<T, TOp>(
            Parser<TOp> op,
            Parser<T> operand,
            Func<TOp, T, T, T> apply)
        {
            if (op == null) throw new ArgumentNullException("op");
            if (operand == null) throw new ArgumentNullException("operand");
            if (apply == null) throw new ArgumentNullException("apply");
            return operand.Then(first => ChainRightOperatorRest(first, op, operand, apply, Or));
        }

        /// <summary>
        /// Chain a right-associative operator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOp"></typeparam>
        /// <param name="op"></param>
        /// <param name="operand"></param>
        /// <param name="apply"></param>
        /// <returns></returns>
        public static Parser<T> XChainRightOperator<T, TOp>(
            Parser<TOp> op,
            Parser<T> operand,
            Func<TOp, T, T, T> apply)
        {
            if (op == null) throw new ArgumentNullException("op");
            if (operand == null) throw new ArgumentNullException("operand");
            if (apply == null) throw new ArgumentNullException("apply");
            return operand.Then(first => ChainRightOperatorRest(first, op, operand, apply, XOr));
        }

        static Parser<T> ChainRightOperatorRest<T, TOp>(
            T lastOperand,
            Parser<TOp> op,
            Parser<T> operand,
            Func<TOp, T, T, T> apply,
            Func<Parser<T>, Parser<T>, Parser<T>> or)
        {
            if (op == null) throw new ArgumentNullException("op");
            if (operand == null) throw new ArgumentNullException("operand");
            if (apply == null) throw new ArgumentNullException("apply");
            return or(op.Then(opvalue =>
                        operand.Then(operandValue =>
                            ChainRightOperatorRest(operandValue, op, operand, apply, or)).Then(r =>
                                Return(apply(opvalue, lastOperand, r)))),
                      Return(lastOperand));
        }
    }
}
