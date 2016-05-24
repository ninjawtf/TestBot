using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication2
{
    class Program
    {
        static int NaturalMin = 1;
        static bool IsNatural(int x)
        {
            Console.Write("IsNatural({0}) ", x);
            return x >= NaturalMin;
        }

        static readonly string EmptyString = "";
        static bool IsEmpty(string s)
        {
            Console.Write("IsEmpty({0}) ", s);
            return s == EmptyString;
        }

        static void Main(string[] args)
        {
            int x = 10;
            string s = "";
            Console.WriteLine(IsNatural(x) && IsEmpty(s));
            Console.ReadKey();
        }
    }
}
