﻿namespace AESharp.BinaryOperators
{
    public class LesserEqual : BinaryOperator
    {
        public override string Identifier => "<=";
        public override int Priority => 20;

        public LesserEqual() { }
        public LesserEqual(Expression left, Expression right) : base(left, right) { }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public override Expression Evaluate()
        {
            return Left <= Right;
        }

        public override Expression Clone(Scope scope)
        {
            return new LesserEqual(Left.Clone(scope), Right.Clone(scope));
        }

        internal override Expression CurrectOperator()
        {
            return new LesserEqual(Left.CurrectOperator(), Right.CurrectOperator());
        }

        protected override Expression ExpandHelper(Expression left, Expression right)
        {
            return new LesserEqual(left, right);
        }

        protected override Expression ReduceHelper(Expression left, Expression right)
        {
            return new LesserEqual(left, right);
        }
    }
}

