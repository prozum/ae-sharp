using System.Collections.Generic;

namespace AESharp.Parser
{
    public static class Scanner
    {
        private const char Eos = (char)0;

        private static char[] _chars;
        private static Pos _position;

        private static Error _error; 

        public static char CharNext(bool eat = true)
        {
            if (_position.I < _chars.Length)
            {
                if (eat)
                {
                    _position.Column++;
                    return _chars[_position.I++];
                }
                return _chars[_position.I];
            }
            else
                return Eos;
        }

        public static Queue<Token> Tokenize(string tokenString, out Error error)
        {
            Token tok;
            error = null;

            var res = new Queue<Token> ();
            _chars = tokenString.ToCharArray();
            _position = new Pos();

            do
            {
                tok = ScanNext();

                if (_error != null)
                {
                    error = _error;
                    return null;
                }

                res.Enqueue(tok);
            }
            while (tok.Kind != TokenKind.EndOfString);

            return res;
        }

        private static Token ScanNext()
        {
            SkipWhitespace();

            char @char = CharNext();

            switch (@char)
            {
                case Eos:
                    return new Token(TokenKind.EndOfString, "END_OF_STRING", _position);
                
                case '\n':
                    _position.Line++;
                    _position.Column = 0;
                    return new Token(TokenKind.NewLine, "NEW_LINE", _position);

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return ScanNumber(@char);
                
                case '"':
                case '\'':
                    return ScanText(@char);
                
                case '~':
                    return new Token(TokenKind.Tilde, "~", _position);
                case '+':
                    return new Token(TokenKind.Add, "+", _position);
                case '-':
                    return new Token(TokenKind.Sub, "-", _position);
                case '*':
                    return new Token(TokenKind.Mul, "*", _position);
                case '/':
                    return new Token(TokenKind.Div, "/", _position);
                case '%':
                    return new Token(TokenKind.Mod, "%", _position);
                case '^':
                    return new Token(TokenKind.Exp, "^", _position);
                case '&':
                    return new Token(TokenKind.And, "&", _position);
                case '|':
                    return new Token(TokenKind.Or, "|", _position);
                case '=':
                    if (CharNext(false) == '=')
                    {
                        CharNext(true);
                        return new Token(TokenKind.BoolEqual, "==", _position);
                    }
                    else
                        return new Token(TokenKind.Equal, "=", _position);
                case '<':
                    if (CharNext(false) == '=')
                    {
                        CharNext(true);
                        return new Token(TokenKind.LessEqual, "<=", _position);
                    }
                    else
                        return new Token(TokenKind.Less, "<", _position);
                case '>':
                    if (CharNext(false) == '=')
                    {
                        CharNext(true);
                        return new Token(TokenKind.GreatEqual, ">=", _position);
                    }
                    else
                        return new Token(TokenKind.Great, ">", _position);
                case ':':
                    if (CharNext(false) == '=')
                    {
                        CharNext(true);
                        return new Token(TokenKind.Assign, ":=", _position);
                    }
                    else
                        return new Token(TokenKind.Colon, ":", _position);
                case '!':
                    if (CharNext(false) == '=')
                    {
                        CharNext(true);
                        return new Token(TokenKind.NotEqual, "!=", _position);
                    }
                    else
                        return new Token(TokenKind.Neg, "!", _position);

                case '(':
                    return new Token(TokenKind.ParentStart, "(", _position);
                case ')':
                    return new Token(TokenKind.ParentEnd, ")", _position);
                case '[':
                    return new Token(TokenKind.SquareStart, "[", _position);
                case ']':
                    return new Token(TokenKind.SquareEnd, "]", _position);
                case '{':
                    return new Token(TokenKind.CurlyStart, "{", _position);
                case '}':
                    return new Token(TokenKind.CurlyEnd, "}", _position);
                
                case ',':
                    return new Token(TokenKind.Comma, ",", _position);
                case ';':
                    return new Token(TokenKind.Semicolon, ";", _position);
                case '.':
                    return new Token(TokenKind.Dot, ".", _position);
                case '#':
                    return new Token(TokenKind.Hash, "#", _position);
                
                default:
                    if (char.IsLetter(@char))
                        return ScanIdentifier(@char);
                    else
                        return new Token(TokenKind.Unknown, @char, _position);
            }
        }

        private static Token ScanNumber(char @char)
        {
            TokenKind kind = TokenKind.Integer;
            string number = @char.ToString();
            Pos startPos = _position;


            var cur = CharNext(false);
            while (char.IsDigit(cur) || cur == '.')
            {
                CharNext();
                number += cur;

                if (cur == '.')
                {
                    //More than one Seperator. Error!
                    if (kind == TokenKind.Decimal)
                    {
                        ReportError("Decimal with more than one seperator");
                        return null;
                    }
                    kind = TokenKind.Decimal;
                }

                cur = CharNext(false);
            }

            if (cur == 'i')
            {
                kind = kind == TokenKind.Integer ? TokenKind.ImagInt : TokenKind.ImagDec;
                CharNext();
            }

            return new Token(kind, number, startPos);
        }

        private static string ExtractSubText(char endChar)
        {
            string res = "";

            char subChar;
            if (endChar == '"')
                subChar = '\'';
            else
                subChar = '"';
                
            do
            {
                var cur = CharNext(true);
                switch (cur)
                {
                    case '"':
                    case '\'':
                        if (cur == endChar)
                            return res;
                        else
                            res += subChar + ExtractSubText(cur) + subChar;
                        break;
                    case Eos:
                        ReportError("Missing end of string");
                        return "";
                    default:
                        res += cur;
                        break;
                }
            }
            while (true);
        }

        private static Token ScanText(char @char)
        {
            var startPos = _position;
            return new Token(TokenKind.Text, ExtractSubText(@char), startPos);
        }

        private static Token ScanIdentifier(char @char)
        {
            string identifier = @char.ToString();
            Pos startPos = _position;

            var cur = CharNext(false);
            while (char.IsLetterOrDigit (cur))
            {
                CharNext(true);
                identifier += cur;
                cur = CharNext(false);
            }

            identifier = identifier.ToLower();

            switch (identifier)
            {
                case "true":
                    return new Token(TokenKind.True, identifier, startPos);
                case "false":
                    return new Token(TokenKind.False, identifier, startPos);
                case "null":
                    return new Token(TokenKind.Null, identifier, startPos);
                case "if":
                    return new Token(TokenKind.If, identifier, startPos);
                case "elif":
                    return new Token(TokenKind.Elif, identifier, startPos);
                case "else":
                    return new Token(TokenKind.Else, identifier, startPos);
                case "for":
                    return new Token(TokenKind.For, identifier, startPos);
                case "in":
                    return new Token(TokenKind.In, identifier, startPos);
                case "while":
                    return new Token(TokenKind.While, identifier, startPos);
                case "ret":
                    return new Token(TokenKind.Ret, identifier, startPos);
                case "import":
                    return new Token(TokenKind.Import, identifier, startPos);
                case "global":
                    return new Token(TokenKind.Global, identifier, startPos);
                case "class":
                    return new Token(TokenKind.Class, identifier, startPos);
                case "and":
                    return new Token(TokenKind.And, identifier, startPos);
                case "or":
                    return new Token(TokenKind.Or, identifier, startPos);
                case "self":
                    return new Token(TokenKind.Self, identifier, startPos);
                default:
                    return new Token(TokenKind.Identifier, identifier, startPos);
            }
        }

        private static void SkipWhitespace()
        {
            while (char.IsWhiteSpace (CharNext(false)) && CharNext(false) != '\n') 
            {
                CharNext(true);
            }
        }

        public static void ReportError(string msg)
        {
            var error = new Error("Scanner: " + msg);
            error.Position = _position;
            _error = error;
        }
    }
}

