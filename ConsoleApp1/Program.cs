using System;
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
        static void Main(string[] args)
        {
            TestExtensions.RunTests();
            Console.ReadKey();

        }
    }
}
