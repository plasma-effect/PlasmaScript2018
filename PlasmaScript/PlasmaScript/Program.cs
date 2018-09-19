using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasmaScript
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var lexer = new Lexer();
                var ret = lexer.Analize(Console.ReadLine());
                
                foreach(var v in ret)
                {
                    Console.WriteLine(v);
                    
                }
            }
            catch(ArgumentException exp)
            {
                Console.WriteLine(exp);
            }
        }
    }
}
