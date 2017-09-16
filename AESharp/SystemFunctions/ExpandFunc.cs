using System.Collections.Generic;
using AESharp.Values;

namespace AESharp.SystemFunctions
{
    public class ExpandFunc : SysFunc
    {
        public ExpandFunc() : this(null) { }
        public ExpandFunc(Scope scope)
            : base("expand", scope)
        {
            ValidArguments = new List<ArgumentType>()
                {
                    ArgumentType.Expression
                };
        }

        public override Expression Call(List args)
        {
            return args[0].Expand();
        }
    }
}

