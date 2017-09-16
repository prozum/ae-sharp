using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using AESharp.BinaryOperators;
using AESharp.KeyExpressions;
using AESharp.UnaryOperators;
using AESharp.Values;
using Boolean = AESharp.Values.Boolean;

namespace AESharp.Parser
{

    public enum ParseContext
    {
        List,
        ScopeSingle,
        ScopeMulti,
        ScopeClass,
        ScopeGlobal,
        If,
        For,
        While,
        Colon,
        Parenthesis
    }

    public class Parser
    {
        private readonly Stack<Scope> _scopeStack = new Stack<Scope>();
        private readonly Stack<ParseContext> _contextStack = new Stack<ParseContext>();

        private readonly Stack<Queue<Expression>> _exprStack = new Stack<Queue<Expression>>();
        private readonly Stack<Queue<PrefixOperator>> _prefixStack = new Stack<Queue<PrefixOperator>>();
        private readonly Stack<Queue<PostfixOperator>> _postfixStack = new Stack<Queue<PostfixOperator>>();
        private readonly Stack<Queue<BinaryOperator>> _binaryStack = new Stack<Queue<BinaryOperator>>();

        private Evaluator.Evaluator _evaluator;
        private Scope CurScope => _scopeStack.Peek();
        private ParseContext CurContext => _contextStack.Peek();

        private Queue<Expression> CurExprStack => _exprStack.Peek();
        private Queue<PrefixOperator> CurPrefixStack => _prefixStack.Peek();
        private Queue<PostfixOperator> CurPostfixStack => _postfixStack.Peek();
        private Queue<BinaryOperator> CurBinaryStack => _binaryStack.Peek();

        private readonly Token _eos = new Token(TokenKind.EndOfString, "END_OF_STRING", new Pos());

        private Queue<Token> _tokens;
        private Token CurToken => _tokens.Count > 0 ? _tokens.Peek() : _eos;

        private bool Error;
        private bool _expectPrefix = true;

        public Expression Parse(string parseString)
        {
            _tokens = Scanner.Tokenize(parseString, out var error);
            if (Error)
                return error;
                
            var result = ParseScope(false, true);

            if (Error) Clear();

            return result;
        }

        #region Peek Eat

        public bool Peek()
        {
            return _tokens.Count > 0;
        }

        public bool Peek(TokenKind kind)
        {
            return CurToken.Kind == kind;
        }

        public bool Eat()
        {
            if (_tokens.Count > 0)
                _tokens.Dequeue();

            return _tokens.Count > 0;
        }

        public bool Eat(TokenKind kind)
        {
            if (_tokens.Count > 0 && _tokens.Peek().Kind == kind)
            {
                _tokens.Dequeue();
                return true;
            }

            return false;
        }

        #endregion

        public void Clear()
        {
            _scopeStack.Clear();
            _contextStack.Clear();
            _exprStack.Clear();
            _prefixStack.Clear();
            _postfixStack.Clear();
            _binaryStack.Clear();
        }

        public Expression ParseScope(bool shared = false, bool global = false, bool @class = false)
        {
            while (Eat(TokenKind.NewLine));

            if (global)
            {
                _contextStack.Push(ParseContext.ScopeGlobal);
                _scopeStack.Push(new Scope());
            }
            else if (@class)
            {
                _contextStack.Push(ParseContext.ScopeClass);
                _scopeStack.Push(new Class(CurScope));

                while (Eat(TokenKind.NewLine));

                if (!Eat(TokenKind.CurlyStart))
                    return ReportError("Missing { bracket");
            }
            else if (Eat(TokenKind.CurlyStart))
            {
                _contextStack.Push(ParseContext.ScopeMulti);
                _scopeStack.Push(new Scope(CurScope, shared));
            }
            else
            {
                _contextStack.Push(ParseContext.ScopeSingle);
                _scopeStack.Push(new Scope(CurScope, shared));
            }

            ParseExpressions();
            if (Error)
                return CurScope.Error;

            if ((CurContext == ParseContext.ScopeMulti || CurContext == ParseContext.ScopeClass)  && !Eat(TokenKind.CurlyEnd))
                return ReportError("Missing } bracket");
                
            _contextStack.Pop();
            return _scopeStack.Pop();
        }

        public void ParseExpressions()
        {
            while (true)
            {
                while (Eat(TokenKind.NewLine));
                CurScope.Expressions.Add(ParseExpr());
                while (Eat(TokenKind.NewLine));

                switch (CurToken.Kind)
                {
                    case TokenKind.CurlyEnd:
                    case TokenKind.Elif:
                    case TokenKind.Else:
                    case TokenKind.EndOfString:
                        return;

                    case TokenKind.Semicolon:
                        Eat();
                        break;
                }

                if (CurContext == ParseContext.ScopeSingle)
                    return;

                if (Error)
                    return;
            }
        }

        public Expression ParseExpr(bool eat = false)
        {
            bool done = false;

            if (eat)
                Eat();

            _exprStack.Push(new Queue<Expression>());
            _prefixStack.Push(new Queue<PrefixOperator>());
            _postfixStack.Push(new Queue<PostfixOperator>());
            _binaryStack.Push(new Queue<BinaryOperator>());

            _expectPrefix = true;

            while (!done)
            {
                switch (CurToken.Kind)
                {
                    case TokenKind.EndOfString:
                        done = true;
                        break;
                    case TokenKind.NewLine:
                        done |= CurContext != ParseContext.Parenthesis;
                        break;

                    case TokenKind.Semicolon:
                        if (CurContext == ParseContext.List)
                            ReportError("Unexpected ';' in " + CurContext);
                        else
                            done = true;
                        break;
                    case TokenKind.Comma:
                        if (CurContext == ParseContext.List)
                            done = true;
                        else
                            ReportError("Unexpected ',' in " + CurContext);
                        Eat();
                        break;

                    case TokenKind.If:
                        done = true;
                        SetupExpr(ParseIf(), false);
                        break;
                    case TokenKind.For:
                        SetupExpr(ParseFor(), false);
                        break;
                    case TokenKind.While:
                        SetupExpr(ParseWhile(), false);
                        break;
                    case TokenKind.Ret:
                        done = true;
                        SetupExpr(new RetExpr(ParseExpr(true), CurScope), false);
                        break;
                    case TokenKind.Import:
                        done = true;
                        SetupExpr(new ImportExpr(ParseExpr(true), CurScope), false);
                        break;
                    case TokenKind.Global:
                        done = true;
                        SetupExpr(new GlobalExpr(ParseExpr(true), CurScope), false);
                        break;
                    case TokenKind.Class:
                        done = true;
                        Eat();
                        SetupExpr(ParseScope(false, false, true), false);
                        break;
                    case TokenKind.Colon:
                        if (CurContext == ParseContext.Colon)
                            done = true;
                        else
                            ReportError("Unexpected ':' in " + CurContext);
                        break;
                    
                    case TokenKind.Elif:
                        done = true;
                        break;
                    case TokenKind.Else:
                        done = true;
                        break;


                    case TokenKind.Integer:
                    case TokenKind.Decimal:
                    case TokenKind.ImagInt:
                    case TokenKind.ImagDec:
                        SetupExpr(ParseNumber(), true);
                        break;
                    
                    case TokenKind.Text:
                        SetupExpr(new Text(CurToken.Value),true);
                        break;
                    
                    case TokenKind.Identifier:
                        SetupExpr(new Variable(CurToken.Value, CurScope), true);
                        break;

                    case TokenKind.True:
                        SetupExpr(new Boolean(true),true);
                        break;
                    case TokenKind.False:
                        SetupExpr(new Boolean(false),true);
                        break;
                    case TokenKind.Null:
                        SetupExpr(new Null(),true);
                        break;
                    case TokenKind.Self:
                        SetupExpr(new Self(CurScope),true);
                        break;

                    case TokenKind.ParentStart:
                        SetupExpr(ParseParenthesis(),false);
                        break;
                    case TokenKind.SquareStart:
                        SetupExpr(ParseList(),false);
                        break;
                    case TokenKind.CurlyStart:
                        if (CurScope is Class)
                            SetupExpr(ParseScope(true), false);
                        else
                            SetupExpr(ParseScope(), false);
                        break;

                    case TokenKind.Hash:
                        ParseComment();
                        break;

                    case TokenKind.ParentEnd:
                        if (CurContext == ParseContext.Parenthesis)
                            done = true;
                        else
                            ReportError("Unexpected ')' in " + CurContext);
                        break;
                    case TokenKind.SquareEnd:
                        if (CurContext == ParseContext.List)
                            done = true;
                        else
                            ReportError("Unexpected ']' in " + CurContext);
                        break;
                    case TokenKind.CurlyEnd:
                        if (CurContext == ParseContext.ScopeMulti || CurContext == ParseContext.ScopeClass || CurContext == ParseContext.ScopeSingle)
                            done = true;
                        else
                            ReportError("Unexpected '}' in " + CurContext);
                        break;
                    
                    case TokenKind.Tilde:
                        if (_expectPrefix)
                            SetupPrefixOp(new Referation());
                        else
                            ReportError(CurToken + " is not supported as binary operator");
                        break;

                    case TokenKind.Add:
                        if (!_expectPrefix) // Ignore Unary +
                            SetupBinaryOp(new Add());
                        break;
                    case TokenKind.Sub:
                        if (_expectPrefix)
                            SetupPrefixOp(new Minus());
                        else
                            SetupBinaryOp(new Sub());
                        break;
                    case TokenKind.Neg:
                        if (_expectPrefix)
                            SetupPrefixOp(new Negation());
                        else
                            ReportError(CurToken + " is not supported as binary operator");
                        break;
                    case TokenKind.Mul:
                        if (_expectPrefix)
                            ReportError(CurToken + " is not supported as unary operator");
                        else
                            SetupBinaryOp(new Mul());
                        break;
                    case TokenKind.Div:
                        if (_expectPrefix)
                            ReportError(CurToken + " is not supported as unary operator");
                        else
                            SetupBinaryOp(new Div());
                        break;
                    case TokenKind.Mod:
                        if (_expectPrefix)
                            ReportError(CurToken + " is not supported as unary operator");
                        else
                            SetupBinaryOp(new Mod());
                        break;
                    case TokenKind.Exp:
                        if (_expectPrefix)
                            ReportError(CurToken + " is not supported as unary operator");
                        else
                            SetupBinaryOp(new Exp());
                        break;

                    case TokenKind.Assign:
                        if (_expectPrefix)
                            ReportError(CurToken + " is not supported as unary operator");
                        else
                        {
                            SetupBinaryOp(new Assign());
                            while (Eat(TokenKind.NewLine)); // Allow assignment on new line
                        }
                        break;
                    case TokenKind.Equal:
                        if (_expectPrefix)
                            ReportError(CurToken + " is not supported as unary operator");
                        else
                            SetupBinaryOp(new Equal());
                        break;
                    case TokenKind.BoolEqual:
                        if (_expectPrefix)
                            ReportError(CurToken + " is not supported as unary operator");
                        else
                            SetupBinaryOp(new BooleanEqual());
                        break;
                    case TokenKind.NotEqual:
                        if (_expectPrefix)
                            ReportError(CurToken + " is not supported as unary operator");
                        else
                            SetupBinaryOp(new NotEqual());
                        break;
                    case TokenKind.LessEqual:
                        if (_expectPrefix)
                            ReportError(CurToken + " is not supported as unary operator");
                        else
                            SetupBinaryOp(new LesserEqual());
                        break;
                    case TokenKind.GreatEqual:
                        if (_expectPrefix)
                            ReportError(CurToken + " is not supported as unary operator");
                        else
                            SetupBinaryOp(new GreaterEqual());
                        break;
                    case TokenKind.Less:
                        if (_expectPrefix)
                            ReportError(CurToken + " is not supported as unary operator");
                        else
                            SetupBinaryOp(new Lesser());
                        break;
                    case TokenKind.Great:
                        if (_expectPrefix)
                            ReportError(CurToken + " is not supported as unary operator");
                        else
                            SetupBinaryOp(new Greater());
                        break;
                    case TokenKind.And:
                        if (_expectPrefix)
                            ReportError(CurToken + " is not supported as unary operator");
                        else
                            SetupBinaryOp(new And());
                        break;
                    case TokenKind.Or:
                        if (_expectPrefix)
                            ReportError(CurToken + " is not supported as unary operator");
                        else
                            SetupBinaryOp(new Or());
                        break;

                    case TokenKind.Dot:
                        if (_expectPrefix)
                            ReportError(CurToken + " is not supported as unary operator");
                        else
                            SetupBinaryOp(new Dot());
                        break;

                    default:
                        ReportError("Unexpected '" + CurToken.ToString() + "'");
                        break;
                }

                if (Error)
                {
                    _prefixStack.Pop();
                    _postfixStack.Pop();
                    _exprStack.Pop();
                    _binaryStack.Pop();
                    return null;
                }
            }
                
            _prefixStack.Pop();
            _postfixStack.Pop();
            return CreateAst(_exprStack.Pop(), _binaryStack.Pop());
        }

        public void ParsePostfix()
        {
            while (Peek(TokenKind.SquareStart))
            {
                var args = ParseList();
                if (Error)
                    return;

                var op = new Call(args, CurScope);
                if (Error)
                    return;

                op.Position = CurToken.Position;
                op.CurScope = CurScope;
                CurPostfixStack.Enqueue(op);
            }
        }

        public void SetupExpr(Expression expr, bool eat)
        {
            if (Error)
                return;

            if (eat)
                Eat();

            expr.Position = CurToken.Position;
            expr.CurScope = CurScope;

            ParsePostfix();

            if (Error)
                return;

            while (CurPostfixStack.Count > 0)
            {
                var postfix = CurPostfixStack.Dequeue();
                postfix.Child = expr;
                expr = postfix;
            }

            while (CurPrefixStack.Count > 0)
            {
                var prefix = CurPrefixStack.Dequeue();
                prefix.Child = expr;
                expr = prefix;
            }

            CurExprStack.Enqueue(expr);
            _expectPrefix = false;

            if (CurExprStack.Count != CurBinaryStack.Count + 1)
                ReportError("Missing operator");
        }

        public void SetupPrefixOp(PrefixOperator op)
        {
            Eat();

            op.Position = CurToken.Position;
            op.CurScope = CurScope;

            CurPrefixStack.Enqueue(op);
        }

        public void SetupBinaryOp(BinaryOperator op)
        {
            Eat();

            op.Position = CurToken.Position;
            op.CurScope = CurScope;

            CurBinaryStack.Enqueue(op);
            _expectPrefix = true;

            if (CurExprStack.Count != CurBinaryStack.Count)
                ReportError("Missing operand");
        }

        public Expression CreateAst(Queue<Expression> exprs, Queue<BinaryOperator> biops)
        {
            Expression left;
            BinaryOperator curOp, nextOp, top, cmpOp;

            if (exprs.Count == 0)
                return Constant.Null;

            if (exprs.Count == 1 && biops.Count == 0)
                return exprs.Dequeue();

            if (exprs.Count != 1 + biops.Count)
                return ReportError("Missing operand");
               

            top = biops.Peek();
            left = exprs.Dequeue();

            while (biops.Count > 0)
            {
                curOp = biops.Dequeue();
                curOp.Left = left;

                if (biops.Count > 0)
                {
                    nextOp = biops.Peek();

                    if (curOp.Priority >= nextOp.Priority)
                    {
                        curOp.Right = exprs.Dequeue();

                        cmpOp = curOp;
                        while (cmpOp.Parent != null)
                        {
                            if (nextOp.Priority > (cmpOp.Parent as BinaryOperator).Priority)
                            {
                                break;
                            }
                            else
                            {
                                cmpOp = cmpOp.Parent as BinaryOperator;
                            }
                        }

                        if (cmpOp.Parent == null)
                        {
                            top = nextOp;
                            left = cmpOp;
                        }
                        else
                        {
                            left = (cmpOp.Parent as BinaryOperator).Right;
                            (cmpOp.Parent as BinaryOperator).Right = nextOp;
                        }
                    }
                    else
                    {
                        left = exprs.Dequeue();

                        curOp.Right = nextOp;
                    }
                }
                else
                {
                    curOp.Right = exprs.Dequeue();
                }
            }

            return top;
        }

        public Expression ParseNumber()
        {
            Int64 intRes;
            decimal decRes;

            switch (CurToken.Kind)
            {
                case TokenKind.Integer:
                    if (Int64.TryParse(CurToken.Value, out intRes))
                        return new Integer(intRes);
                    else
                        return ReportError("Int overflow");
                
                case TokenKind.Decimal:
                    if (decimal.TryParse(CurToken.Value, out decRes))
                        return new Irrational(decRes);
                    else
                        return ReportError("Decimal overflow");
                case TokenKind.ImagInt:
                    if (Int64.TryParse(CurToken.Value, out intRes))
                        return new Complex(new Integer(0), new Integer(intRes));
                    else
                        return ReportError("Imaginary int overflow");
                
                case TokenKind.ImagDec:
                    if (decimal.TryParse(CurToken.Value, out decRes))
                        return new Complex(new Integer(0), new Irrational(decRes));
                    else
                        return ReportError("Imaginary decimal overflow");
                
                default:
                    throw new Exception("Wrong number token");
            }
        }

        public Expression ParseIf()
        {
            Expression cond;
            Expression expr;

            Eat();

            _contextStack.Push(ParseContext.If);
            var ifexpr = new IfExpr(CurScope);

            cond = ParseColon();
            if (Error)
                return null;
            ifexpr.Conditions.Items.Add(cond);

            expr = ParseScope(true);
            if (Error)
                return null;
            ifexpr.Expressions.Items.Add(expr);

            while (Eat(TokenKind.NewLine));
            while (Eat(TokenKind.Elif))
            {
                cond = ParseColon();
                if (Error)
                    return null;
                ifexpr.Conditions.Items.Add(cond);

                expr = ParseScope(true);
                if (Error)
                    return null;
                ifexpr.Expressions.Items.Add(expr);

                while (Eat(TokenKind.NewLine));
            }

            while (Eat(TokenKind.NewLine));
            if (Eat(TokenKind.Else))
            {
                Eat(TokenKind.Colon); // Optional colon

                expr = ParseScope(true);
                if (Error)
                    return null;
                ifexpr.Expressions.Items.Add(expr);
            }

            _contextStack.Pop();
            return ifexpr;
        }

        public Expression ParseFor()
        {
            Eat();

            _contextStack.Push(ParseContext.For);
            var forexpr = new ForExpr(CurScope);

            if (Peek(TokenKind.Identifier))
            {
                forexpr.Var = CurToken.Value;
                Eat();
            }
            else
                return ReportError("For: Missing symbol");

            if (!Eat(TokenKind.In))
                return ReportError("For: Missing in");

            forexpr.List = ParseColon();
            if (Error)
                return null;

            forexpr.ForScope = ParseScope();
            if (Error)
                return null;

            _contextStack.Pop();
            return forexpr;
        }

        public Expression ParseWhile()
        {
            Expression expr;
            Eat();

            _contextStack.Push(ParseContext.While);
            var whileexpr = new WhileExpr(CurScope);

            whileexpr.Condition = ParseColon();
            if (Error)
                return null;

            expr = ParseScope();
            if (Error)
                return null;
            whileexpr.Condition.CurScope = whileexpr.WhileScope = (Scope)expr;

            _contextStack.Pop();
            return whileexpr;
        }

        public Expression ParseColon()
        {
            _contextStack.Push(ParseContext.Colon);
            var res = ParseExpr();
            _contextStack.Pop();

            if (!Eat(TokenKind.Colon))
                return ReportError("Missing :");

            return res;
        }

        public List ParseList()
        {
            var list = new List();

            Eat();
            _contextStack.Push(ParseContext.List);

            while (_tokens.Count > 0)
            {
                if (!(Eat(TokenKind.Comma) || Eat(TokenKind.NewLine)))
                    list.Items.Add(ParseExpr());

                if (Error)
                    return list;

                if (Eat(TokenKind.SquareEnd))
                    break;

                if (Peek(TokenKind.EndOfString))
                {
                    ReportError("Missing ] bracket");
                    break;
                }
            }

            _contextStack.Pop();

            if (list.Items.Count == 1 && list.Items[0] is Null)
                list.Items.Clear();

            return list;
        }

        public Expression ParseParenthesis()
        {
            Eat();
            _contextStack.Push(ParseContext.Parenthesis);

            Expression parent = ParseExpr();

            if (Error)
                return null;

            if (!Eat(TokenKind.ParentEnd))
                ReportError("Missing ) bracket");

            _contextStack.Pop();

            return parent;
        }

        public void ParseComment()
        {
            do
            {
                Eat();
            }
            while (CurToken.Kind != TokenKind.NewLine && CurToken.Kind != TokenKind.EndOfString);
        }

        public Expression ReportError(string msg)
        {
            var error = new Error(msg);
            error.Position = CurToken.Position;

            Error = true;

            return null;
        }
    }
}

