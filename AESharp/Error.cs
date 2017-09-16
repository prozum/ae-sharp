﻿using AESharp.Parser;
using AESharp.SystemFunctions;

namespace AESharp
{
    public class Error : Expression
    {
        public string ErrorMessage;

        public Error()
        {
        }

        public Error(string msg) 
        {
            ErrorMessage = msg;
        }

        public Error(Pos pos, string msg)
        {
            Position = pos;
            ErrorMessage = msg;
        }

        public Error (Expression expr, string msg)
        {
            if (expr is Variable)
                ErrorMessage = (expr as Variable).Identifier + ": " + msg;
            else
                ErrorMessage = expr.GetType().Name + ": " + msg;

            Position = expr.Position;
        }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public override string ToString()
        {
            return ErrorMessage;
        }

        public override Expression Evaluate()
        {
            return this;
        }

        public override bool CompareTo(Expression other)
        {
            return false;
        }

        public override Expression Clone(Scope scope)
        {
            return this;
        }
    }

    public class ArgumentError: Error
    {
        public ArgumentError(SysFunc func) : base(func, "valid arguments: ")
        {
            ErrorMessage += "[";
            for(int i = 0; i < func.ValidArguments.Count; i++)
            {
                ErrorMessage += func.ValidArguments[i].ToString();

                if (i < func.ValidArguments.Count -1) 
                {
                    ErrorMessage += ',';
                }
            }
            ErrorMessage += "]";
        }
    }
}

