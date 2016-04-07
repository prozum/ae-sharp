﻿using System;
using System.Collections.Generic;

namespace AESharp
{
    public class AtanFunc : SysFunc, IInvertable
    {
        public AtanFunc() : this(null) { }
        public AtanFunc(Scope scope) : base("atan", scope)
        {
            ValidArguments = new List<ArgumentType>()
                {
                    ArgumentType.Real
                };
        }

        public override Expression Call(List args)
        {
            if (!IsArgumentsValid(args))
                return new ArgumentError(this);

            var res = args[0].Evaluate();

            var deg = GetBool("deg");

            if (res is Real)
            {
                double value = res as Real;

                if (value == 1)
                    return Constant.Deg45 * (deg ? new Irrational(1M) : Constant.DegToRad);
                if (value == 0.5)
                    return Constant.Deg26d57 * (deg ? new Irrational(1M) : Constant.DegToRad);

                return new Irrational((decimal)Math.Atan(value) * (deg ? Constant.RadToDeg.@decimal : 1)).Evaluate();
            }

            return new Error(this, "Could not take ATan of: " + args[0]);
        }

        public override Expression Reduce(List args, Scope scope)
        {
            var reduced = args.Reduce() as List;

            if (reduced[0] is Call && (reduced[0] as Call).Child.Value is TanFunc)
                return (reduced[0] as Call).Arguments[0];

            return this;
        }

        //atan[x] -> tan[other]
        public Expression InvertOn(Expression other)
        {
            var arg = new List();
            arg.Items.Add(other);
            return SysFunc.MakeFunction(arg, CurScope, "tan");
        }
    }
}

