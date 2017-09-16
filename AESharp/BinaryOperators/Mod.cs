namespace AESharp.BinaryOperators
{
    public class Mod : BinaryOperator
    {
        public override string Identifier => "%";
        public override int Priority => 40;

        public Mod() { }
        public Mod(Expression left, Expression right) : base(left, right) { }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public override Expression Evaluate()
        {
            return Left % Right;
        }

        protected override Expression ExpandHelper(Expression left, Expression right)
        {
            return new Mod(left, right);
        }

        protected override Expression ReduceHelper(Expression left, Expression right)
        {
            return new Mod(left, right);
        }

        public override Expression Clone(Scope scope)
        {
            return new Mod(Left.Clone(scope), Right.Clone(scope));
        }
    }
}

