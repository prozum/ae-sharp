namespace AESharp.Evaluator
{
    public class PrintData : EvalData
    {
        public string Msg;

        public PrintData(string msg)
        {
            this.Msg = msg;
        }

        public override string ToString()
        {
            return Msg;
        }
    }
}

