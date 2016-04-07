﻿namespace AESharp
{
    public class Exp : BinaryOperator, IInvertable
    {
        public override string Identifier { get { return "^"; } }
        public override int Priority { get{ return 50; } }

        public Exp() { }
        public Exp(Expression left, Expression right) : base(left, right) { }

        public override Expression Evaluate()
        {
            return Left ^ Right;
        }

        protected override Expression ExpandHelper(Expression left, Expression right)
        {
            //(x+y)^2 -> x^2 + y^2 + 2xy
            if (left is Add && Right.CompareTo(Constant.Two))
            {
                return new Add(new Add(new Exp((left as BinaryOperator).Left, right).Reduce(), new Exp((left as BinaryOperator).Right, right).Reduce()), new Mul(new Integer(2), new Mul((left as BinaryOperator).Left, (left as BinaryOperator).Right)).Reduce());
            }
            //(x-y)^2 -> x^2 - y^2 + 2xy
            else if (left is Sub && Right.CompareTo(Constant.Two))
            {
                return new Sub(new Add(new Exp((left as BinaryOperator).Left, right).Reduce(), new Exp((left as BinaryOperator).Right, right).Reduce()), new Mul(new Integer(2), new Mul((left as BinaryOperator).Left, (left as BinaryOperator).Right)).Reduce());
            }
            //(x*y)^z -> x^z * y^z
            else if (left is Mul)
            {
                return new Mul(new Exp((left as BinaryOperator).Left, right).Reduce(), new Exp((left as BinaryOperator).Right, right).Reduce());
            }
            //(x/y)^z -> x^z / y^z
            else if (left is Div)
            {
                return new Div(new Exp((left as BinaryOperator).Left, right).Reduce(), new Exp((left as BinaryOperator).Right, right).Reduce());
            }

            //Couldn't expand.
            return new Exp(left.Expand(), right.Expand());
        }

        protected override Expression ReduceHelper(Expression left, Expression right)
        {
            //When left is one, return 1. 1^x -> 1 or
            //When right is zero, return 1. x^0 -> 1
            if (left.CompareTo(Constant.One) || right.CompareTo(Constant.Zero))
            {
                return new Integer(1);
            }
            //Both are real. Calculate. 2^2 -> 4
            else if (left is Real && right is Real)
            {
                return left ^ right;
            }
            //When left i Variable and right is real, multiply left's exponent with right.
            else if (left is Variable && right is Real)
            {
                return VariableOperation(left as Variable, right as Real);
            }
            //When left i Variable and right is real, multiply left's exponent with right.
            else if (left is Call && left.Value is SqrtFunc && right.CompareTo(Constant.Two))
            {
                return (left as Call).Arguments[0];
            }

            //Couldn't reduce.
            return new Exp(left, right);
        }

        //Multipies Variable's exponent with a real, and returns Variable.
        private Variable VariableOperation(Variable left, Real right)
        {
            Variable res = left.Clone() as Variable;

            res.Exponent = (left.Exponent * right) as Real;

            return res;
        }

        public override Expression Clone(Scope scope)
        {
            return new Exp(Left.Clone(scope), Right.Clone(scope));
        }

        public Expression InvertOn(Expression other)
        {
            //When right is 2, the invert is sqrt. x^2 -> sqrt[other], -sqrt[other]
            if (Right.CompareTo(Constant.Two))
            {
                var args = new List();
                args.Items.Add(other);

                var answer1 = SysFunc.MakeFunction(args, new Scope(), "sqrt");
                var answer2 = new Minus();
                answer2.Child = answer1;
                var answers = new List();

                answers.Items.Add(answer1);
                answers.Items.Add(answer2);
                return answers;
            }

            //Couln't InvertOn.
            return null;
        }

        internal override Expression CurrectOperator()
        {
            return new Exp(Left.CurrectOperator(), Right.CurrectOperator());
        }
    }
}

