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
            var line = ReadLine();
            var lexer = new Lexer();
            var parser = new Parser();
            var lex = lexer.Analize(line);
            var isend = lex.Count == 0;
            foreach (var a in lex)
            {
                WriteLine(a);
            }
            WriteLine();
            WriteLine(parser.ParseExpr(lex.GetArrayEnumerator(), ref isend));
        }
    }
}
