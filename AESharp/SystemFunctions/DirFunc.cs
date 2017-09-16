using System.Collections.Generic;
using AESharp.Values;

namespace AESharp.SystemFunctions
{
    public class DirFunc : SysFunc
    {
        public DirFunc() : this(null) { }
        public DirFunc(Scope scope) : base("dir", scope)
        {
            ValidArguments = new List<ArgumentType>()
                {
                    ArgumentType.Scope
                };
        }

        public override Expression Call(List args)
        {
            var scope = (Scope)args[0].Value;

            var list = new List();

            foreach (var @var in scope.Locals)
            {
                list.Items.Add(@var.Value);
            }

            return list;
        }
    }
}

