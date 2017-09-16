namespace AESharp.Values
{
    public class Null : Expression
    {
        public Null()
        {
        }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public override string ToString()
        {
            return "null";
        }

        public override Expression Evaluate()
        {
            return this;
        }

        public override Expression Clone(Scope scope)
        {
            return this;
        }
    }
}