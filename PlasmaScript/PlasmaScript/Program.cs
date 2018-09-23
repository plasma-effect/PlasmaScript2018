using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace PlasmaScript
{
    class Program
    {
        static void Main(string[] args)
        {
            var str = @"int sum(array<@T> ar)
{
    @T ret = 0;
    for(auto a: ar)
    {
        ret = a;
    }
    return ret;
}";
            using (var stream = new Utility.ReplaceReader("@T", "int", new System.IO.StringReader(str)))
            {
                WriteLine(stream.ReadToEnd());
            }
        }
    }
}
