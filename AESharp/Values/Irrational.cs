namespace AESharp.Values
{
    public class Irrational : Real
    {
        public decimal V;

        public Irrational(double value) : this ((decimal)value) {}
        public Irrational(decimal value)
        {
            V = value;
        }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public override decimal Decimal => V;

        public override Expression Clone(Scope scope)
        {
            return new Irrational(V);
        }
    }
}

