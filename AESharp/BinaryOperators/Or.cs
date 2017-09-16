namespace AESharp.BinaryOperators
{
    public class Or : BinaryOperator
    {
        public override string Identifier => "|";
        public override int Priority => 10;

        public Or() { }
        public Or(Expression left, Expression right) : base(left, right) { }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public override Expression Evaluate()
        {
            return Left | Right;
        }

        protected override Expression ExpandHelper(Expression left, Expression right)
        {
            return new Or(left, right);
        }

        protected override Expression ReduceHelper(Expression left, Expression right)
        {
            return new Or(left, right);
        }

        public override Expression Clone(Scope scope)
        {
            return new Or(Left.Clone(scope), Right.Clone(scope));
        }
    }
}

