namespace AESharp
{
    public abstract class Number : Expression
    {
        public override Expression Evaluate()
        {
            return this;
        }
    }
}

