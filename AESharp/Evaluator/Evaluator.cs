﻿using System;

namespace AESharp
{
    public class Evaluator : Scope
    {
        public Parser Parser;

        public static Expression Eval(string parseString)
        {
            var eval = new Evaluator();

            eval.Parse(parseString);

            return eval.Evaluate();
        }

        public Evaluator()
        {
            Parser = new Parser(this);
            Global = this;

            SetVar("reduc", new Boolean(true));
            SetVar("debug", new Boolean(true));

            var scope = new Scope(this);
            SetVar("sys", scope);
            scope.SetVar("print", new PrintFunc(this));
            scope.SetVar("dir", new DirFunc(this));
            scope.SetVar("eval", new EvalFunc(this));
            scope.SetVar("type", new TypeFunc(this));
            scope.SetVar("clone", new CloneFunc(this));

            scope = new Scope(this);
            SetVar("math", scope);
            scope.SetVar("pi", new Irrational((decimal)Math.PI));
            scope.SetVar("abs", new AbsFunc(this));
            scope.SetVar("solve", new SolveFunc(this));
            scope.SetVar("sqrt", new SqrtFunc(this));
            scope.SetVar("expand", new ExpandFunc(this));
            scope.SetVar("reduce", new ReduceFunc(this));
            scope.SetVar("range", new RangeFunc(this));

            scope = new Scope(this);
            SetVar("trig", scope);
            scope.SetVar("deg", new Boolean(true));
            scope.SetVar("sin", new SinFunc(this));
            scope.SetVar("cos", new CosFunc(this));
            scope.SetVar("tan", new TanFunc(this));
            scope.SetVar("asin", new AsinFunc(this));
            scope.SetVar("acos", new AcosFunc(this));
            scope.SetVar("atan", new AtanFunc(this));

            scope = new Scope(this);
            SetVar("draw", scope);
            scope.SetVar("plot", new PlotFunc(this));
            scope.SetVar("paraplot", new ParaPlotFunc(this));
            scope.SetVar("line", new LineFunc(this));

            scope = new Scope(this);
            SetVar("widget", scope);
            scope.SetVar("checkbox", new CheckboxFunc(this));
        }

        public void Parse(string parseString)
        {
            Expressions.Clear();
            SideEffects.Clear();
            Error = null;
            Parser.Parse(parseString);
        }
    }
}