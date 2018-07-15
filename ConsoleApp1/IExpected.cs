using System;

namespace ConsoleApp1
{
    public interface IExpected<T>
    {
        void Eq(object other);
        IExpected<T> Properties();
        IExpected<T> PropertiesWithout<TResult>(Func<T, TResult> func);
        void IsGreater(T other);
        IExpected<T> Not();
    }
}
