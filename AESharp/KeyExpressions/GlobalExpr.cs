using AESharp.Values;

namespace AESharp.KeyExpressions
{
    public class GlobalExpr : Expression
    {
        private readonly Expression _expression;

        public GlobalExpr(Expression expr, Scope scope)
        {
            _expression = expr;
            CurScope = scope;
        }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public override Expression Evaluate()
        {
            if (_expression is List)
            {
                foreach (var expr in (_expression as List).Items)
                {
                    if (expr is Error)
                        return expr;

                    if (expr is Variable)
                        CurScope.Globals.Add((expr as Variable).Identifier);
                    else
                        return new Error(_expression, "contains Non-Variables");
                }
            }

            if (_expression is Variable)
                CurScope.Globals.Add((_expression as Variable).Identifier);
            else
                return new Error(_expression, "is not at Variable");

            return Constant.Null;
        }

        public override Expression Clone(Scope scope)
        {
            return new GlobalExpr(_expression.Clone(scope), scope);
        }

        public override string ToString()
        {
            return "global " + _expression.ToString();
        }
    }
}

