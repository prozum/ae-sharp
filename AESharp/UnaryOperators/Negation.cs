namespace AESharp.UnaryOperators
{
    public class Negation : PrefixOperator
    {
        public Negation() : base("!") { }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public override Expression Evaluate()
        {
            return Child.Evaluate().Negation();
        }

        public override Expression Reduce()
        {
            var res = new Negation();
            res.Child = Child.Reduce();
            return res;
        }

        public override Expression Clone(Scope scope)
        {
            var nega = new Negation();
            nega.CurScope = scope;
            nega.Child = Child.Clone(scope);

            return nega;
        }
    }
}

