using System;

namespace ConsoleApp1
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void Test_Eq()
        {
            6.Expect().Eq(6);
        }

        [TestMethod]
        [ExpectedException(typeof(ExpectationFailedExceptin))]
        public void Test_Eq_Fails()
        {
            6.Expect().Eq(5);
        }

        [TestMethod]
        public void Test_Greater()
        {
            10.Expect().IsGreater(6);
        }

        [TestMethod]
        [ExpectedException(typeof(ExpectationFailedExceptin))]
        public void Test_Greater_Fails()
        {
            (-10).Expect().IsGreater(4);
        }

        [TestMethod]
        public void Test_NotEq()
        {
            10.Expect().Not().Eq(9);
        }

        [TestMethod]
        [ExpectedException(typeof(ExpectationFailedExceptin))]
        public void Test_NotEq_Fails()
        {
            10.Expect().Not().Eq(10);
        }

        //[TestMethod]
        //public void Test_Raise_Error()
        //{
        //    (new Action(() => { throw new Exception(); })).Expect().RaiseError();
        //}

        //[TestMethod]
        //[ExpectedException(typeof(ExpectationFailedExceptin))]
        //public void Test_Raise_Error_Fails()
        //{
        //    (new Action(() => FizBar.SampleMethod())).Expect().RaiseError();
        //}

        [TestMethod]
        public void Test_Properties()
        {
            var tested = new FizBar() { Bar = "Bar", Fiz = String.Empty };
            var expected = new { Bar = "Bar", Fiz = String.Empty };
            tested.Expect().Properties().Eq(expected);
        }

        [TestMethod]
        [ExpectedException(typeof(ExpectationFailedExceptin))]
        public void Test_Properties_Fails()
        {
            var tested = new FizBar() { Bar = "Bar", Fiz = String.Empty };
            var expected = new { Bar = "Bar2", Fiz = String.Empty };
            tested.Expect().Properties().Eq(expected);
        }

        [TestMethod]
        public void Test_PropertiesWithout()
        {
            var tested = new FizBar() { Bar = "Bar", Fiz = "Fiz" };
            var expected = new { Bar = "Bar", Fiz = "Fiz2" };
            tested.Expect().PropertiesWithout(x => x.Fiz).Eq(expected);
        }

        [TestMethod]
        [ExpectedException(typeof(ExpectationFailedExceptin))]
        public void Test_PropertiesWithout_Fails()
        {
            var tested = new FizBar() { Bar = "Bar", Fiz = "Fiz" };
            var expected = new { Bar = "Bar2", Fiz = "Fiz2" };
            tested.Expect().PropertiesWithout(x => x.Fiz).Eq(expected);
        }
    }
}
