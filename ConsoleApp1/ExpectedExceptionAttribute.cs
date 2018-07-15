using System;

namespace ConsoleApp1
{

    public class ExpectedExceptionAttribute : Attribute
    {
        public Type ExceptionType { get; private set; }

        public ExpectedExceptionAttribute(Type exceptionType)
        {
            this.ExceptionType = exceptionType;
        }
    }
}