using System;
using System.Collections.Generic;
using AESharp.Values;

namespace AESharp.SystemFunctions
{
    public class RangeFunc : SysFunc
    {
        public RangeFunc() : this(null) { }
        public RangeFunc(Scope scope) : base("range", scope)
        {
            ValidArguments = new List<ArgumentType>()
                {
                    ArgumentType.Real,
                    ArgumentType.Real,
                    ArgumentType.Real
                };
        }

        public override Expression Call(List args)
        {
            Decimal start;
            Decimal end;
            Decimal step;

            start = args[0].Evaluate() as Real;

            end = args[1].Evaluate() as Real;

            step = args[2].Evaluate() as Real;

            var list = new List ();
            for (Decimal i = start; i <= end; i += step)
            {
                list.Items.Add(new Irrational(i));
            }

            return list;
        }
    }
}

