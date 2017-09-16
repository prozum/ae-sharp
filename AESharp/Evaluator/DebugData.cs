namespace AESharp.Evaluator
{
    public class DebugData : EvalData
    {
        public string Msg;

        public DebugData(string msg)
        {
            this.Msg = msg;
        }

        public override string ToString()
        {
            return Msg;
        }
    }
}

