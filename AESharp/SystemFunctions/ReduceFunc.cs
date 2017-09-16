using System.Collections.Generic;
using AESharp.Values;

namespace AESharp.SystemFunctions
{
    public class ReduceFunc : SysFunc
    {
        public ReduceFunc() : this(null) { }
        public ReduceFunc(Scope scope) : base("reduce", scope)
        {
            ValidArguments = new List<ArgumentType>()
                {
                    ArgumentType.Expression
                };
        }

        public override Expression Call(List args)
        {
            return args[0].ReduceCurrectOp();
        }
    }
}

