﻿using System;

namespace AESharp
{
    public static class Constant
    {
        // these are used so many times for simplifying the expressions that they are just made constants
        public static readonly Integer MinusOne = new Integer(-1);
        public static readonly Integer Zero = new Integer(0);
        public static readonly Integer One = new Integer(1);
        public static readonly Integer Two = new Integer(2);
        public static readonly Rational Half = new Rational(One, Two);
        public static readonly Rational Deg26d57 = new Rational(new Integer(2657), new Integer(100));
        public static readonly Integer Deg30 = new Integer(30);
        public static readonly Integer Deg45 = new Integer(45);
        public static readonly Integer Deg60 = new Integer(60);
        public static readonly Integer Deg90 = new Integer(90);

        public static readonly Null Null = new Null();
        
        public static readonly Irrational DegToRad = new Irrational(Math.PI / 180);
        public static readonly Irrational RadToDeg = new Irrational(180 / Math.PI);
    }
}

