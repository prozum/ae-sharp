﻿namespace AESharp
{
    public class Irrational : Real
    {
        public decimal _decimal;

        public Irrational(double value) : this ((decimal)value) {}
        public Irrational(decimal value)
        {
            _decimal = value;
        }

        public override decimal @decimal
        {
            get
            {
                return _decimal;
            }
        }

        public override Expression Clone(Scope scope)
        {
            return new Irrational(@decimal);
        }
    }
}

