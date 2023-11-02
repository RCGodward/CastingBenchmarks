using BenchmarkDotNet.Running;
using System;
using System.Linq;

namespace Casting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double v1 = 1.3477424851005059E+19;
            double v2 = 1.3477424851005061E+19;

            BenchmarkRunner.Run(new Type[]
            {
                //typeof(UlongToUint),
                typeof(ManyUlongsToUints),
                //typeof(UlongToDouble)
            });
        }
    }
}