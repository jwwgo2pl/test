using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace ConsoleApp1
{
    public static class TestExtensions
    {
        public static IExpected<T> Expect<T>(this T obj)
        {
            return new Expected<T>(obj);
        }

        public static void RunTests()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var allTestClassess = assembly.GetTypes()
                .Where(type => type.GetCustomAttribute<TestClassAttribute>() != null)
                .ToList();

            foreach(var testClass in allTestClassess)
            {
                var testMethods = testClass.GetMethods().Where(mthd => mthd.GetCustomAttribute<TestMethodAttribute>() != null).ToList();
                var testObject = testClass.GetConstructors()[0].Invoke(new object[] { });

                foreach(var testMethod in testMethods)
                {
                    try
                    {
                        testMethod.Invoke(testObject, new object[] { });
                        Console.WriteLine($"Test {testMethod.Name} passed");
                    }
                    catch (Exception e)
                    {
                        var exceptionAttibute = testMethod.GetCustomAttribute<ExpectedExceptionAttribute>();
                        if(exceptionAttibute != null && e.InnerException.GetType() == exceptionAttibute.ExceptionType)
                        {
                            Console.WriteLine($"Test {testMethod.Name} passed");
                            continue;
                        }
                        else
                        {
                            Console.WriteLine($"Test {testMethod.Name} failed: {e.InnerException.Message}");
                        }
                    }
                }
            }
        }
    }
}
