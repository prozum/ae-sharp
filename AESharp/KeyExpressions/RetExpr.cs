namespace AESharp.KeyExpressions
{
    public class RetExpr : Expression
    {
        public Expression Expression;

        public RetExpr(Expression expr, Scope scope)
        {
            Expression = expr;
            CurScope = scope;
        }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public override Expression Evaluate()
        {
            var res = Expression.Evaluate();

            if (res is Error)
                return res;

            CurScope.Returns.Items.Clear();
            CurScope.Returns.Items.Add(res);
            CurScope.Return.Bool = true;

            return res;
        }

        public override Expression Clone(Scope scope)
        {
            return new RetExpr(Expression.Clone(scope), scope);
        }

        public override string ToString()
        {
            return "ret " + Expression.ToString();
        }
    }
}

