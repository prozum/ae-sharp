﻿using System.Collections.Generic;
using AESharp.UnaryOperators;

namespace AESharp.Values
{
    public class List : Expression, ICallable
    {
        public List<Expression> Items;

        private const int MaxItemPrint = 10;

        public Expression this[int i]
        {
            get => Items[i];
            set => Items[i] = value;
        }

        public int Count => Items.Count;

        public override Scope CurScope
        {
            get => base.CurScope;
            set
            {
                base.CurScope = value;

                foreach (var item in Items)
                {
                    item.CurScope = value;
                }
            }
        }

        public List() : this(new List<Expression> ()) {}
        public List(List<Expression> items)
        {
            Items = items;
        }

        public override bool Visit(IVisitor v) => v.Visit(this);

        public override Expression Evaluate()
        {
            var res = new List();

            foreach (var item in Items)
            {
                res.Items.Add(item.Evaluate());
            }

            return res;
        }

        public override Expression Reduce()
        {
            var res = new List<Expression>();
            foreach (var item in Items)
            {
                res.Add(item.Reduce());
            }

            return new List(res);
        }

        public bool IsArgumentsValid(List args)
        {
            if (args.Count != 1 || !(args[0].Evaluate() is Integer))
                return false;

            return true;
        }

        public Error GetArgumentError(List args)
        {
            return new Error(this, "Valid arguments: [Integer]");
        }

        public Expression Call(List args)
        {
            var @long = (args[0].Evaluate() as Integer).Int;

            if (@long < 0)
                return new Error(this, "Cannot access with negative integer");

            int @int;
            if (@long > int.MaxValue)
                return new Error(this, "Integer is too big");
            @int = (int)@long;

            if (@int > Count - 1)
                return new Error(this, "Cannot access item " + (@int + 1).ToString() + " in list with " + Count + " items");
            return this[@int];
        }

        public override string ToString()
        {
            string str = "[";

            for (int i = 0; i < Items.Count; i++) 
            {
                if (i >= MaxItemPrint - 1)
                {
                    str += "..." + Items[Items.Count - 1].ToString();
                    break;
                }
                else
                {
                    str += Items[i].ToString ();

                    if (i < Items.Count - 1) 
                    {
                        str += ',';
                    }
                }
            }

            str += "]";

            return str;
        }

        public override bool ContainsVariable(Variable other)
        {
            foreach (var item in Items)
            {
                if (item.ContainsVariable(other))
                {
                    return true;
                }
            }

            return false;
        }

        public override Expression Clone(Scope scope)
        {
            var res = new List();

            foreach (var item in Items)
            {
                res.Items.Add(item.Clone(scope));
            }

            return res;
        }
    }
}

