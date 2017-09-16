namespace AESharp.Values
{
    public class Boolean : Expression
    {
        public bool Bool;

        public Boolean(bool value)
        {
            Bool = value;
        }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public override Expression Evaluate()
        {
            return this;
        }

        public static bool operator true (Boolean b)
        {
            return b.Bool;
        }

        public static bool operator false (Boolean b)
        {
            return b.Bool;
        }

        public override string ToString()
        {
            return Bool.ToString();
        }

        public override Expression Clone(Scope scope)
        {
            return new Boolean(Bool);
        }

        public override Expression Negation()
        {
            return new Boolean(!Bool);
        }

        public override Expression AndWith(Boolean other)
        {
            return new Boolean(Bool && other.Bool);
        }

        public override Expression OrWith(Boolean other)
        {
            return new Boolean(Bool || other.Bool);
        }
    }
}

