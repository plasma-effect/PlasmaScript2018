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
            try
            {
                var lexer = new Lexer();
                var parser = new Parser();
                var ret = lexer.Analize(ReadLine());
                foreach(var r in ret)
                {
                    WriteLine(r);
                }
                var func = parser.Parsing(ret);
                WriteLine(func);
            }
            catch(Exception exp)
            {
                WriteLine(exp);
            }
        }
    }
}
