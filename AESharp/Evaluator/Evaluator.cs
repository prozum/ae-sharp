using System;
using AESharp.SystemFunctions;
using AESharp.Values;
using Boolean = AESharp.Values.Boolean;

namespace AESharp.Evaluator
{
    public class Evaluator
    {
        public static Expression Eval(string parseString)
        {
            var parser = new Parser.Parser();
            var result = parser.Parse(parseString);

            if (!(result is Scope s)) return result.Evaluate();

            s.Global = s;

            s.SetVar("reduc", new Boolean(true));
            s.SetVar("debug", new Boolean(true));

            var scope = new Scope(s);
            s.SetVar("sys", scope);
            scope.SetVar("print", new PrintFunc(s));
            scope.SetVar("dir", new DirFunc(s));
            scope.SetVar("eval", new EvalFunc(s));
            scope.SetVar("type", new TypeFunc(s));
            scope.SetVar("clone", new CloneFunc(s));

            scope = new Scope(s);
            s.SetVar("math", scope);
            scope.SetVar("pi", new Irrational((decimal)Math.PI));
            scope.SetVar("abs", new AbsFunc(s));
            scope.SetVar("solve", new SolveFunc(s));
            scope.SetVar("sqrt", new SqrtFunc(s));
            scope.SetVar("expand", new ExpandFunc(s));
            scope.SetVar("reduce", new ReduceFunc(s));
            scope.SetVar("range", new RangeFunc(s));

            scope = new Scope(s);
            s.SetVar("trig", scope);
            scope.SetVar("deg", new Boolean(true));
            scope.SetVar("sin", new SinFunc(s));
            scope.SetVar("cos", new CosFunc(s));
            scope.SetVar("tan", new TanFunc(s));
            scope.SetVar("asin", new AsinFunc(s));
            scope.SetVar("acos", new AcosFunc(s));
            scope.SetVar("atan", new AtanFunc(s));

            scope = new Scope(s);
            s.SetVar("draw", scope);
            scope.SetVar("plot", new PlotFunc(s));
            scope.SetVar("paraplot", new ParaPlotFunc(s));
            scope.SetVar("line", new LineFunc(s));

            scope = new Scope(s);
            s.SetVar("widget", scope);
            scope.SetVar("checkbox", new CheckboxFunc(s));

            return s.Evaluate();
        }
    }
}