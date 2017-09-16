using System;

namespace AESharp.Values
{
    public abstract class Real : Number, INegative
    {
        public abstract Decimal Decimal
        {
            get;
        }
            
        public static implicit operator Decimal(Real r)
        {
            return r.Decimal;
        }

        public static implicit operator double(Real r)
        {
            return (Double)r.Decimal;
        }

        public override string ToString()
        {
            return Decimal.ToString();
        }

        public override bool CompareTo(Expression other)
        {
            Expression evalOther;

            if (other is Variable && !(other as Variable).IsDefined)
                return false;

            evalOther = other.Evaluate();

            if (evalOther is Real)
            {
                return Decimal == evalOther as Real;
            }

            return false;
        }

        public Expression ToNegative()
        {
            return this * Constant.MinusOne;
        }

        public bool IsNegative()
        {
            return Decimal < 0;
        }

        public override Expression Minus()
        {
            return ToNegative();
        }

        #region AddWith
        public override Expression AddWith(Integer other)
        {
            return new Irrational(Decimal + other.Decimal);
        }

        public override Expression AddWith(Rational other)
        {
            return new Irrational(Decimal + other.Decimal);
        }

        public override Expression AddWith(Irrational other)
        {
            return new Irrational(Decimal + other.V);
        }

        public override Expression AddWith(Complex other)
        {
            return new Complex(new Irrational(Decimal + other.Real.Decimal), other.Imag);
        }

        public override Expression AddWith(Text other)
        {
            return new Text(Decimal + other.String);
        }

        #endregion

        #region SubWith
        public override Expression SubWith(Integer other)
        {
            return new Irrational(Decimal - other.Decimal);
        }

        public override Expression SubWith(Rational other)
        {
            return new Irrational(Decimal - other.Decimal);
        }

        public override Expression SubWith(Irrational other)
        {
            return new Irrational(Decimal - other.V);
        }

        public override Expression SubWith(Complex other)
        {
            return new Complex(new Irrational(Decimal - other.Real.Decimal), other.Imag);
        }

        #endregion

        #region MulWith
        public override Expression MulWith(Integer other)
        {
            return new Irrational(Decimal * other.Decimal);
        }

        public override Expression MulWith(Rational other)
        {
            return new Irrational(Decimal * other.Decimal);
        }

        public override Expression MulWith(Irrational other)
        {
            return new Irrational(Decimal * other.V);
        }

        public override Expression MulWith(Complex other)
        {
            return new Complex(new Irrational(Decimal * other.Real.Decimal), new Irrational(Decimal * other.Imag.Decimal));
        }

        #endregion

        #region DivWith
        public override Expression DivWith(Integer other)
        {
            return new Irrational(Decimal / other.Decimal);
        }

        public override Expression DivWith(Rational other)
        {
            return new Irrational(Decimal / other.Decimal);
        }

        public override Expression DivWith(Irrational other)
        {
            return new Irrational(Decimal / other.V);
        }

        public override Expression DivWith(Complex other)
        {
            return new Complex(new Irrational(Decimal / other.Real.Decimal), new Irrational(Decimal / other.Imag.Decimal));
        }

        #endregion

        #region ExpWith
        public override Expression ExpWith(Integer other)
        {
            return new Irrational(Math.Pow((double)Decimal, (double)other.Decimal));
        }

        public override Expression ExpWith(Rational other)
        {
            return new Irrational(Math.Pow((double)Decimal, (double)other.Decimal));
        }

        public override Expression ExpWith(Irrational other)
        {
            return new Irrational(Math.Pow((double)Decimal, (double)other.V));
        }

        #endregion

        #region GreaterThan
        public override Expression GreaterThan(Integer other)
        {
            return new Boolean(Decimal > other.Decimal);
        }

        public override Expression GreaterThan(Rational other)
        {
            return new Boolean(Decimal > other.Decimal);
        }

        public override Expression GreaterThan(Irrational other)
        {
            return new Boolean(Decimal > other.V);
        }

        #endregion

        #region LesserThan
        public override Expression LesserThan(Integer other)
        {
            return new Boolean(Decimal < other.Decimal);
        }

        public override Expression LesserThan(Rational other)
        {
            return new Boolean(Decimal < other.Decimal);
        }

        public override Expression LesserThan(Irrational other)
        {
            return new Boolean(Decimal < other.V);
        }

        #endregion

        #region GreaterThanEqualTo
        public override Expression GreaterThanOrEqualTo(Integer other)
        {
            return new Boolean(Decimal >= other.Decimal);
        }

        public override Expression GreaterThanOrEqualTo(Rational other)
        {
            return new Boolean(Decimal >= other.Decimal);
        }

        public override Expression GreaterThanOrEqualTo(Irrational other)
        {
            return new Boolean(Decimal >= other.V);
        }

        #endregion

        #region LesserThanOrEqualTo
        public override Expression LesserThanOrEqualTo(Integer other)
        {
            return new Boolean(Decimal <= other.Decimal);
        }

        public override Expression LesserThanOrEqualTo(Rational other)
        {
            return new Boolean(Decimal <= other.Decimal);
        }

        public override Expression LesserThanOrEqualTo(Irrational other)
        {
            return new Boolean(Decimal <= other.V);
        }

        #endregion

        #region ModuloWith
        public override Expression ModWith(Integer other)
        {
            return new Irrational(Decimal % other.Decimal);
        }

        public override Expression ModWith(Rational other)
        {
            return new Irrational(Decimal % other.Decimal);
        }

        public override Expression ModWith(Irrational other)
        {
            return new Irrational(Decimal % other.V);
        }

        #endregion
    }
}

