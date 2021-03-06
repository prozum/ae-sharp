﻿namespace AESharp.BinaryOperators
{
    public class Greater : BinaryOperator
    {
        public override string Identifier => ">";
        public override int Priority => 20;

        public Greater() { }
        public Greater(Expression left, Expression right) : base(left, right) { }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public override Expression Evaluate()
        {
            return Left > Right;
        }

        public override Expression Clone(Scope scope)
        {
            return new Greater(Left.Clone(scope), Right.Clone(scope));
        }

        internal override Expression CurrectOperator()
        {
            return new Greater(Left.CurrectOperator(), Right.CurrectOperator());
        }

        protected override Expression ExpandHelper(Expression left, Expression right)
        {
            return new Greater(left, right);
        }

        protected override Expression ReduceHelper(Expression left, Expression right)
        {
            return new Greater(left, right);
        }
    }
}

