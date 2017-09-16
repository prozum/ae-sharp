using AESharp.Parser;

namespace AESharp.Evaluator
{
    public class ErrorData : EvalData
    {
        public string Msg;
        public Pos Position;

        public ErrorData(string err)
        {
            Msg = err;
        }

        public ErrorData(Error err)
        {
            Msg = err.ErrorMessage;
            Position = err.Position;
        }

        public ErrorData(Expression expr, string err)
        {
            Msg = err;
            Position = expr.Position;
        }

        public override string ToString()
        {
            var str = "";

            str += "[" + Position.Column;
            str += ";" + Position.Line + "]";
            str += Msg;

            return str;
        }
    }
}

