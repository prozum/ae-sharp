using AESharp.Values;

namespace AESharp.BinaryOperators
{
    public class NotEqual : BinaryOperator
    {
        public override string Identifier => "!=";
        public override int Priority => 20;

        public NotEqual() { }
        public NotEqual(Expression left, Expression right) : base(left, right) { }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public override Expression Evaluate()
        {
            return new Boolean(!Left.CompareTo(Right));
        }

        public override Expression Clone(Scope scope)
        {
            return new NotEqual(Left.Clone(scope), Right.Clone(scope));
        }

        internal override Expression CurrectOperator()
        {
            return new NotEqual(Left.CurrectOperator(), Right.CurrectOperator());
        }

        protected override Expression ExpandHelper(Expression left, Expression right)
        {
            return new NotEqual(left, right);
        }

        protected override Expression ReduceHelper(Expression left, Expression right)
        {
            return new NotEqual(left, right);
        }
    }
}

