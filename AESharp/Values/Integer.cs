using System;

namespace AESharp.Values
{
    public class Integer : Real
    {
        public Int64 Int;

        public Integer(Int64 value)
        {
            Int = value;
        }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public override decimal Decimal => (decimal)Int;

        public static implicit operator Int64(Integer i)
        {
            return i.Int;
        }

        public override Expression Clone(Scope scope)
        {
            return new Integer(Int);
        }

        #region AddWith
        public override Expression AddWith(Integer other)
        {
            return new Integer(Int + other.Int);
        }

        public override Expression AddWith(Rational other)
        {
            return new Rational(Int, 1) + other;
        }

        #endregion

        #region SubWith
        public override Expression SubWith(Integer other)
        {
            return new Integer(Int - other.Int);
        }

        public override Expression SubWith(Rational other)
        {
            return new Rational(Int, 1) - other;
        }

        #endregion

        #region MulWith
        public override Expression MulWith(Integer other)
        {
            return new Integer(Int * other.Int);
        }

        public override Expression MulWith(Rational other)
        {
            return new Rational(Int, 1) * other;
        }

        #endregion

        #region ExpWith
        public override Expression ExpWith(Integer other)
        {
            return new Integer((Int64)Math.Pow(Int, other.Int));
        }

        public override Expression ExpWith(Rational other)
        {
            return new Rational(Int, 1) ^ other;
        }

        #endregion

        #region ModuloWith
        public override Expression ModWith(Integer other)
        {
            return new Integer(Int % other.Int);
        }

        public override Expression ModWith(Rational other)
        {
            return new Rational(this, new Integer(1)) % other;
        }

        #endregion

    }
}

