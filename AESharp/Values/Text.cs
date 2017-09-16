using AESharp.SystemFunctions;

namespace AESharp.Values
{
    public class Text : Expression
    {
        public string String;

        public Text(string value = "")
        {
            String = value;
        }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public static implicit operator string(Text t)
        {
            return t.String;
        }

        public override Expression Evaluate()
        {
            return this;
        }

        public override Expression Clone(Scope scope)
        {
            return new Text(String);
        }

        public override string ToString()
        {
            return String;
        }

        public override bool CompareTo(Expression other)
        {
            if (other is Text)
            {
                return (String.CompareTo((other as Text).String) == 0) ? true : false;
            }
            if (other is TypeFunc)
            {
                var text = (other as TypeFunc).Evaluate();

                if (text is Text)
                {
                    return (String.CompareTo((text as Text).String) == 0) ? true : false;
                }
            }

            return false;
        }

        public override Expression AddWith(Text other)
        {
            return new Text(String + other);
        }

        public override Expression AddWith(Integer other)
        {
            return new Text(String + other.ToString());
        }

        public override Expression AddWith(Rational other)
        {
            return new Text(String + other.ToString());
        }

        public override Expression AddWith(Irrational other)
        {
            return new Text(String + other.ToString());
        }

        public override Expression AddWith(Complex other)
        {
            return new Text(String + other.ToString());
        }

        public override Expression AddWith(Boolean other)
        {
            return new Text(String + other.ToString());
        }
    }
}

