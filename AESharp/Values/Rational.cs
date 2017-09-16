using System;

namespace AESharp.Values
{
    public class Rational : Real
    {
        public Int64 Numerator;
        public Int64 Denominator;

        public Rational(Int64 num, Int64 denom)
        {
            Numerator = num;
            Denominator = denom;
        }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public override decimal Decimal => ((decimal)Numerator) / Denominator;

        public void Reduce(Integer num, Integer denom)
        {
            Gcd (num, denom);
        }

        public static Integer Gcd(Integer num, Integer denom)
        {
            throw new NotImplementedException ();
        }

        public override Expression Clone(Scope scope)
        {
            return new Rational(Numerator, Denominator);
        }
            

        #region AddWith
        public override Expression AddWith(Integer other)
        {
            return this + new Rational(other, new Integer(1));
        }

        public override Expression AddWith(Rational other)
        {
            var newNumerator = Numerator * other.Denominator;
            var otherNewNumerator = Denominator * other.Numerator;

            return new Rational(newNumerator + otherNewNumerator, Denominator * other.Denominator);
        }

        #endregion

        #region SubWith
        public override Expression SubWith(Integer other)
        {
            return this - new Rational(other, new Integer(1));
        }

        public override Expression SubWith(Rational other)
        {
            var leftNumerator = Numerator * other.Denominator;
            var rightNumerator = Denominator * other.Numerator;

            return new Rational(leftNumerator - rightNumerator, Denominator * other.Denominator);
        }

        #endregion

        #region MulWith
        public override Expression MulWith(Integer other)
        {
            return this * new Rational(other, new Integer(1));
        }

        public override Expression MulWith(Rational other)
        {
            return new Rational(Numerator * other.Numerator, Denominator * other.Denominator);
        }

        #endregion

        #region DivWith
        public override Expression DivWith(Integer other)
        {
            return this / new Rational(other, new Integer(1));
        }

        public override Expression DivWith(Rational other)
        {
            return this * new Rational(other.Denominator, other.Numerator);
        }

        #endregion

        #region ExpWith
        public override Expression ExpWith(Integer other)
        {
            return new Rational(Numerator ^ other.Int, Denominator ^ other.Int);
        }

        #endregion

        #region ModuloWith
        public override Expression ModWith(Integer other)
        {
            return this % new Rational(other, new Integer(1));
        }

        public override Expression ModWith(Rational other)
        {
            var newNumerator = Numerator * other.Denominator;
            var otherNewNumerator = Denominator * other.Numerator;

            return new Rational(newNumerator % otherNewNumerator, Denominator * other.Denominator);
        }

        #endregion
    }
}

