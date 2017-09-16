using System.Collections.Generic;
using AESharp.Values;

namespace AESharp.Evaluator
{
    public class PlotData : DrawData
    {
        public List<Real> X;
        public List<Real> Y;
        public List<Real> Z;

        public PlotData(List<Real> x, List<Real> y, List<Real> z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}

