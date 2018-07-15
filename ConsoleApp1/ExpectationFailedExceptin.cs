using System;

namespace ConsoleApp1
{
    public class ExpectationFailedExceptin : Exception
    {
        public ExpectationFailedExceptin(string message) : base(message)
        {
        }
    }
}