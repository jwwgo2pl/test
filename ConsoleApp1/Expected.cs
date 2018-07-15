using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleApp1
{
    public class Expected<T> : IExpected<T>
    {
        private Dictionary<string, PropertyInfo> _testedObjectProperties;
        protected T _testedObject;
        private bool _negateResult;

        public Expected(T testedObject)
        {
            if(testedObject == null)
            {
                throw new ArgumentNullException("testedObject");
            }

            this._testedObject = testedObject;
        }

        //this can be made virtual for testing other than properties
        public void Eq(object other)
        {
            if(other == null) //|| other.GetType() != _testedObject.GetType())
            {
                throw new ArgumentNullException("other");
            }

            bool result = true;

            if(_testedObjectProperties == null)
            {
                result = _testedObject.Equals(other);
                if (_negateResult)
                {
                    result = !result;
                }

                if(result == false)
                {
                    throw new ExpectationFailedExceptin("Eq failed");
                }

                return;
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
                    result = false;
                    break;
                }

                //other value is not equal to this value - aborting
                if (!otherValue.Equals(thisValue))
                {
                    result = false;
                    break;
                }
            }

            if (_negateResult)
            {
                result = !result;
            }

            if(result == false)
            {
                throw new ExpectationFailedExceptin("Eq failed");
            }    
        }

        public void IsGreater(T other)
        {
            if(other == null)
            {
                throw new ArgumentNullException("other");
            }

            if(!(this._testedObject is IComparable))
            {
                throw new Exception("Values are not comparable");
            }

            var result = ((IComparable)_testedObject).CompareTo(other) > 0;
            if (_negateResult)
            {
                result = !result;
            }

            if(result == false)
            {
                throw new ExpectationFailedExceptin("IsGreater failed");
            }
        }

        public IExpected<T> Not()
        {
            this._negateResult = true;
            return this;
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
            var allMethods = (from type in assembly.GetTypes()
                              from method in type.GetMethods()
                              select  method).OrderByDescending(m => m.MetadataToken)
                              .ToList();

            var methodToken = GetCalledFunctionToken(func);
            var methodInfo = allMethods.First(m => m.MetadataToken == methodToken);
            
            var propertyName = methodInfo.Name.Replace("get_", "");

            _testedObjectProperties.Remove(propertyName);

            return this;
        }

        private static int GetCalledFunctionToken<TParam, TResult>(Func<TParam, TResult> func)
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
                    return il[i + 1] + ( ToInt((il[i + 2])) << 8) + (ToInt(il[i+3]) << 16) + (ToInt(il[i+4]) << 24);
                }
            }

            throw new Exception("No callvirt instruction found");
        }

        private static int ToInt(byte v)
        {
            return (int)v;
        }
    }
}
