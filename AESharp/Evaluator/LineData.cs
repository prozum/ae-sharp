namespace AESharp.Evaluator
{
    public class LineData : DrawData
    {
        public decimal X1;
        public decimal Y1;

        public decimal X2;
        public decimal Y2;

        public LineData(decimal x1, decimal y1, decimal x2, decimal y2)
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.X2 = x2;
            this.Y2 = y2;
        }
    }
}

