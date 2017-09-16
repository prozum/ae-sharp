using System.Collections.Generic;
using AESharp.BinaryOperators;
using AESharp.UnaryOperators;
using AESharp.Values;

namespace AESharp.SystemFunctions
{
    public class SolveFunc : SysFunc
    {
        private Equal _equal;
        private Variable _var;

        public SolveFunc() : this(null) { }
        public SolveFunc(Scope scope)
            : base("solve", scope)
        {
            ValidArguments = new List<ArgumentType>()
                {
                    ArgumentType.Equation,
                    ArgumentType.Variable
                };
        }

        public override Expression Call(List args)
        {
            Equal solved;

            _equal = (Equal)args[0];
            _var = (Variable)args[1];

            if (_equal.Right.ContainsVariable(_var))
            {
                solved = new Equal(new Sub(_equal.Left, _equal.Right).Expand(), new Integer(0));
            }
            else
            {
                solved = (_equal.Expand() as Equal);
            }

            while (!((solved.Left is Variable) && solved.Left.CompareTo(_var)))
            {
                if (solved.Left is BinaryOperator && solved.Left is IInvertable)
                {
                    solved = InvertOperator(solved.Left, solved.Right);

                    if (solved == null)
                        return new Error(this, " could not solve " + _var.ToString());
                }
                else if (solved.Left is Call && (solved.Left as Call).Child.Value is IInvertable)
                {
                    solved = InvertFunction((solved.Left as Call), solved.Right);

                    if (solved == null)
                        return new Error(this, " could not solve " + _var.ToString());
                }
                else if (solved.Left is Variable && (solved.Left as Variable).Identifier == _var.Identifier)
                {
                    var newLeft = (solved.Left as Variable).SeberateNumbers();

                    solved = new Equal(newLeft, solved.Right);
                }
                else
                {
                    return new Error(this, " could not solve " + _var.ToString());
                }
            }

            return solved.Right.ReduceCurrectOp();
        }

        private Equal InvertOperator(Expression left, Expression right)
        {
            BinaryOperator op = left as BinaryOperator;

            if (op.Right.ContainsVariable(_var) && op.Left.ContainsVariable(_var))
            {
                return BothSideSymbolSolver(left, right);
            }
            else if (op.Left.ContainsVariable(_var))
            {
                var inverted = (op as IInvertable).InvertOn(right);

                if (inverted == null)
                    return null;

                return new Equal(op.Left, inverted);
            }
            else if (op.Right.ContainsVariable(_var))
            {
                if (op is ISwappable)
                {
                    return new Equal((op as ISwappable).Swap(), right);
                }
                else if (op is Div)
                {
                    if (!right.CompareTo(Constant.Zero))
                    {
                        return new Equal(op.Right, new Div(op.Left, right));
                    }
                }
            }

            return null;
        }

        private Equal BothSideSymbolSolver(Expression left, Expression right)
        {
            var leftSimplified = left.Reduce();

            if (leftSimplified is BinaryOperator && ((leftSimplified as BinaryOperator).Left.ContainsVariable(_var) && (leftSimplified as BinaryOperator).Right.ContainsVariable(_var)))
            {
                return null;
            }
            else
            {
                return new Equal(leftSimplified, right);
            }
        }

        private Equal InvertFunction(Call call, Expression right)
        {
            SysFunc func = call.Child.Value as SysFunc;

            if (call.ContainsVariable(_var))
            {
                return new Equal(call.Arguments[0], (func as IInvertable).InvertOn(right));
            }

            return null;
        }
    }
}

