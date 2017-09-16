using AESharp.BinaryOperators;
using AESharp.KeyExpressions;
using AESharp.UnaryOperators;
using AESharp.Values;

namespace AESharp
{
    public interface IVisitor
    {
        bool Visit(Add add);
        bool Visit(And add);
        bool Visit(Assign add);
        bool Visit(BooleanEqual add);
        bool Visit(Div add);
        bool Visit(Dot add);
        bool Visit(Equal add);
        bool Visit(Exp add);
        bool Visit(Greater add);
        bool Visit(GreaterEqual add);
        bool Visit(Lesser add);
        bool Visit(LesserEqual add);
        bool Visit(Mod add);
        bool Visit(Mul add);
        bool Visit(NotEqual add);
        bool Visit(Or add);
        bool Visit(Sub add);
        bool Visit(Scope add);
        bool Visit(Self add);
        bool Visit(Boolean add);
        bool Visit(Integer add);
        bool Visit(Irrational add);
        bool Visit(List add);
        bool Visit(Rational add);
        bool Visit(Call add);
        bool Visit(Referation add);
        bool Visit(Variable add);
        bool Visit(Error add);
        bool Visit(ForExpr add);
        bool Visit(GlobalExpr add);
        bool Visit(IfExpr add);
        bool Visit(RetExpr add);
        bool Visit(ImportExpr add);
        bool Visit(Null add);
        bool Visit(Text add);
        bool Visit(Complex add);
        bool Visit(WhileExpr add);
        bool Visit(Minus add);
        bool Visit(Negation add);
    }
}