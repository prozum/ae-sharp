﻿using System;
using System.Collections.Generic;
using AESharp.Values;

namespace AESharp.SystemFunctions
{
    public class AbsFunc : SysFunc
    {
        public AbsFunc() : this(null) { }
        public AbsFunc(Scope scope) : base("abs", scope)
        {
            ValidArguments = new List<ArgumentType>()
                {
                    ArgumentType.Expression
                };
        }

        public override Expression Call(List args)
        {
            var res = args[0].Evaluate();

            if (res is INegative)
            {
                if ((res as INegative).IsNegative())
                    return (res as INegative).ToNegative();
                else
                    return res;
            }

            if (res is Complex)
            {
                var c = res as Complex;

                return new Irrational(Math.Sqrt(Math.Pow((double)Math.Abs(c.Real.Decimal),2) + Math.Pow((double)Math.Abs(c.Imag.Decimal),2)));
            }

            return new Error(this, "Could not take Abs of: " + args[0]);
        }
    }
}

