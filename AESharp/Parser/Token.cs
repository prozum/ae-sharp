namespace AESharp.Parser
{
    public enum TokenKind
    {
        Identifier,
        True,
        False,
        Null,
        Self,

        If,
        Elif,
        Else,
        For,
        In,
        While,
        Ret,
        Import,
        Global,
        Class,

        Text,

        Integer,
        Decimal,
        ImagInt,
        ImagDec,

        Tilde,
        Assign,
        Equal,
        BoolEqual,
        LessEqual,
        GreatEqual,
        NotEqual,
        Less,
        Great,
        Add,
        Sub,
        Mul,
        Div,
        Mod,
        Exp,
        Neg,
        And,
        Or,
        
        ParentStart,
        ParentEnd,
        SquareStart,
        SquareEnd,
        CurlyStart,
        CurlyEnd,

        Comma,
        Colon,
        Semicolon,
        Dot,
        Hash,

        NewLine,
        EndOfString,
        None,
        Unknown
    }

    public struct Pos
    {
        public int I;

        public int Line;
        public int Column;

        public Pos(int i = 0, int line = 1, int column = 0)
        {
            this.I = i;
            Line = line;
            Column = column;
        }
    }

    public class Token
    {
        public TokenKind Kind;
        public string Value;
        public Pos Position;

        public Token(TokenKind kind, char value, Pos pos) : this(kind, value.ToString(), pos) {}
        public Token(TokenKind kind, string value, Pos pos)
        {
            Kind = kind;
            Value = value;
            Position = pos;
            Position.I++;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}

