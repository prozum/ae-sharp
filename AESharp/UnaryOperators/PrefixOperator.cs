namespace AESharp.UnaryOperators
{
    public abstract class PrefixOperator : UnaryOperator
    {
        public string Identifier;

        protected PrefixOperator(string identifier)
        {
            Identifier = identifier;
        }
            
        public override bool ContainsVariable(Variable other)
        {
            return Child.ContainsVariable(other);
        }

        public override string ToString()
        {
            return Identifier + Child.ToString();
        }

        public override bool CompareTo(Expression other)
        {
            return Evaluate().CompareTo(other);
        }
    }
}

