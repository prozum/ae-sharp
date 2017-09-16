using System.Collections.Generic;
using AESharp.Values;

namespace AESharp.SystemFunctions
{
    public class CloneFunc : SysFunc
    {
        public CloneFunc() : this(null) { }
        public CloneFunc(Scope scope) : base("clone", scope)
        {
            ValidArguments = new List<ArgumentType>()
                {
                    ArgumentType.Expression
                };
        }

        public override Expression Call(List args)
        {
            return args[0].Clone();
        }
    }
}

