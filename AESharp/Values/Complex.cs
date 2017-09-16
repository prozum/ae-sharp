namespace AESharp.Values
{
    public class Complex : Number
    {
        public Real Real;
        public Real Imag;

        public Complex(Real real, Real imag)
        {
            this.Real = real;
            this.Imag = imag;
        }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public override string ToString()
        {
            if (Imag.IsNegative())
                return "(" + Real + Imag.ToString() + "i)";
            else
                return "(" + Real.ToString () + '+' + Imag.ToString() + "i)";
        }

        public override bool CompareTo(Expression other)
        {
            var res = base.CompareTo(other);

            if (res)
            {
                if (Real.CompareTo((other as Complex).Real) || Imag.CompareTo((other as Complex).Imag))
                {
                    res = false;
                }
            }

            return res;
        }

        public override Expression Clone(Scope scope)
        {
            return new Complex(Real.Clone() as Real, Imag.Clone() as Real);
        }

        public override Expression Minus()
        {
            return new Complex(Real.ToNegative() as Real, Imag.ToNegative() as Real);
        }

        public override Expression AddWith(Complex other)
        {
            return new Complex(Real + other.Real as Real, Imag + other.Imag as Real);
        }

        public override Expression SubWith(Complex other)
        {
            return new Complex(Real - other.Real as Real, Imag - other.Imag as Real);
        }
    }
}

