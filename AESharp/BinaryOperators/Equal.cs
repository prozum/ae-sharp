namespace AESharp.BinaryOperators
{
    public class Equal : BinaryOperator
    {
        public override string Identifier => "=";
        public override int Priority => 0;

        public Equal() { }
        public Equal(Expression left, Expression right) : base(left, right) { }

        public override bool Visit(IVisitor v) => v.Visit(this);

        protected override Expression ExpandHelper(Expression left, Expression right)
        {
            return new Equal(left, right);
        }

        protected override Expression ReduceHelper(Expression left, Expression right)
        {
            return new Equal(left, right);
        }

        public override Expression Clone(Scope scope)
        {
            return new Equal(Left.Clone(scope), Right.Clone(scope));
        }

        internal override Expression CurrectOperator()
        {
            return new Equal(Left.CurrectOperator(), Right.CurrectOperator());
        }
    }
}

