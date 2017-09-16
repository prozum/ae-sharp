namespace AESharp.UnaryOperators
{
    public class Referation : PrefixOperator
    {
        public Referation() : base("~") { }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public override Expression Value
        {
            get => Child.Value;
            set => Child.Value = value;
        }

        public override Expression Evaluate()
        {
            return Child;
        }

        public override Expression Clone(Scope scope)
        {
            var refe = new Referation();
            refe.CurScope = scope;
            refe.Child = Child.Clone(scope);

            return refe;
        }
    }
}

