namespace AESharp
{
    public class Self : Expression
    {
        public Self(Scope scope)
        {
            CurScope = scope;
        }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public override Expression Evaluate()
        {
            return Value;
        }

        public override Expression Value => CurScope;

        public override Expression Clone(Scope scope)
        {
            return new Self(scope);
        }

        public override string ToString()
        {
            return "self";
        }
    }
}

