﻿using AESharp.UnaryOperators;

namespace AESharp.BinaryOperators
{
    public class Assign : BinaryOperator
    {
        public override string Identifier => ":=";
        public override int Priority => 0;

        public Assign() { }
        public Assign(Expression left, Expression right, Scope scope) : base(left, right) 
        {
            CurScope = scope;
        }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public override Expression Evaluate()
        {
            string identifier;
            Scope scope;
            Expression expr;

            Expression res;

            // Find Variable & Scope
            if (Left is Dot)
            {
                scope = (Left as Dot).VariabelScope;

                if (scope == null)
                    return new Error(Left.ToString() + " is not a valid scope");

                res = (Left as Dot).Right;
            }
            else
            {
                res = Left;
                scope = CurScope;
            }

            if (res is Error)
                return res;

            // Find Identifier & Expression
            if (res is Variable)
            {
                identifier = (res as Variable).Identifier;
                expr = Right.Evaluate();
            }
            else if (res is Call)
            {
                var call = (Call)res;

                if (call.Child is Variable)
                {
                    identifier = (call.Child as Variable).Identifier;
                    expr = new VarFunc(identifier, Right, call.Arguments, scope);
                }
                else
                    return new Error(call.Child, "is not a variable");
            }
            else
                return new Error(res, " is not a variable");

            if (expr is Error)
                return expr;

            scope.SetVar(identifier, expr);

            return expr;
        }

        protected override Expression ExpandHelper(Expression left, Expression right)
        {
            return new Assign(left, right, CurScope);
        }

        public override Expression Clone(Scope scope)
        {
            return new Assign(Left.Clone(scope), Right.Clone(scope), scope);
        }

        internal override Expression CurrectOperator()
        {
            return new Assign(Left.CurrectOperator(), Right.CurrectOperator(), CurScope);
        }
    }
}

