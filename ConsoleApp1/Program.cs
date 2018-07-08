using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class FizBar
    {
        public string Bar { get; set; }
        public string Fiz { get; set; }

     

        public static int SampleMethod()
        {
            return 2 + 2;
        }
    }

    public static class Program
    {


        public static void Test_Properties()
        {
            var tested = new FizBar() { Bar = "Bar", Fiz = String.Empty };
            var expected = new { Bar = "Bar", Fiz = String.Empty };
            var p = tested.Expect().Properties().Eq(expected);
        }

        public static void Test_PropertiesWithout()
        {
            var tested = new FizBar() { Bar = "Bar", Fiz = "Fiz" };
            var expected = new { Bar = "Bar", Fiz = "Fiz2" };
            var p = tested.Expect().PropertiesWithout(x => x.Fiz).Eq(expected);
            var p2 = tested.Expect().PropertiesWithout(x => x.Bar).Eq(expected);
        }

        static void Main(string[] args)
        {
            Test_PropertiesWithout();


        }
    }

    public static class TestExtensions
    {

        public static IExpected<T> Expect<T>(this T obj)
        {
            return new ExpectedProperites<T>(obj);
        }
    }

    public interface IExpected<T>
    {
        bool Eq(object other);
        IExpected<T> Properties();
        IExpected<T> PropertiesWithout<TResult>(Func<T, TResult> func);
    }

    public class ExpectedProperites<T> : IExpected<T>
    {
        private T _testedObject;
        private Dictionary<string, PropertyInfo> _testedObjectProperties;

        public ExpectedProperites(T testedObject)
        {
            if(testedObject == null)
            {
                throw new ArgumentNullException("testedObject");
            }

            this._testedObject = testedObject;
        }

        //this can be made virtual for testing other than properties
        public virtual bool Eq(object other)
        {
            if(other == null) //|| other.GetType() != _testedObject.GetType())
            {
                throw new ArgumentNullException("other");
            }

            var _otherObjectProperties = other
                .GetType()
                .GetProperties()
                .ToDictionary(prop => prop.Name, prop => prop);


            //checking equality of intersection of properties names
            foreach(var propertyName in _testedObjectProperties.Keys
                .Where(key => _otherObjectProperties.Keys.Contains(key)))
            {
                object otherValue = _otherObjectProperties[propertyName].GetValue(other);
                object thisValue = _testedObjectProperties[propertyName].GetValue(_testedObject);

                if(otherValue == null)
                {
                    if(thisValue == null)
                    {
                        continue;
                    }
                    //other is null and this is not null - not equal, abotring
                    return false;
                }

                //other value is not equal to this value - aborting
                if (!otherValue.Equals(thisValue))
                {
                    return false;
                }
            }

            return true;
            
        }

        public IExpected<T> Properties()
        {
            _testedObjectProperties = _testedObject.GetType().GetProperties().ToDictionary(prop=>prop.Name, prop=>prop);
            return this;
        }

        public IExpected<T> PropertiesWithout<TResult>(Func<T, TResult> func)
        {
            if(_testedObjectProperties == null)
            {
                Properties();
            }

            var assembly = Assembly.GetExecutingAssembly();
            var allMethod = (from type in assembly.GetTypes()
                              from method in type.GetMethods()
                             select  method).ToList();

            var indexedMethods = allMethod.ToDictionary(method => allMethod.IndexOf(method), method => method);

            //var allMethods = this._testedObject.GetType().GetMethods().ToDictionary(meth => meth.MetadataToken, meth => meth);
            var x = Assembly.GetExecutingAssembly().GetModules().ToList().SelectMany(module => module.GetMethods()).ToArray();
            var methodInfo = indexedMethods[GetCalledFunctionId(func)];

            var propertyName = methodInfo.Name.Replace("get_", "");

            _testedObjectProperties.Remove(propertyName);

            return this;
        }

        private static int GetCalledFunctionId<TParam, TResult>(Func<TParam, TResult> func)
        {
            var x = MethodInfo.GetMethodFromHandle(func.GetMethodInfo().MethodHandle);
            var body = x.GetMethodBody();
            byte[] il = (byte[])body.GetType().GetField("m_IL", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).GetValue(body);

            //looking for callvirt method or call method
            for (int i = 0; i < il.Length; i++)
            {
                if (il[i] == 111 || il[i] == 0x28)
                {
                    //returning address for called method
                    var result = il[i + 1];
                    return result - 1;
                }
            }

            throw new Exception("No callvirt instruction found");
        }
    }
}
