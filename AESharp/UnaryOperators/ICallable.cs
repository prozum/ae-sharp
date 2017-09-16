using AESharp.Values;

namespace AESharp.UnaryOperators
{
    public interface ICallable
    {
        bool IsArgumentsValid(List args);

        Error GetArgumentError(List args);

        Expression Call(List args);
    }
}

