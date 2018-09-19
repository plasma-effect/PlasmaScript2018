using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using static ParsingTypes.LexerToken;

namespace PlasmaScript
{
    public class Lexer
    {
        Regex doublereg;
        Regex int64reg;
        Regex stringreg;
        Regex charreg;
        Regex namereg;
        Regex twowordopregs;
        Regex onewordopregs;
        Regex pararegs;

        string[] keywords;

        public Lexer()
        {
            this.doublereg = new Regex(@"(?<value>\d+)\.(?<value2>\d*)");
            this.int64reg = new Regex(@"(?<value>\d+)");
            this.stringreg = new Regex(@"""(?<value>([^""\\]|\\.)*)""");
            this.charreg = new Regex(@"\'(?<value>.)\'");
            this.namereg = new Regex(@"(?<value>\w+)");
            this.twowordopregs = new Regex(@"(\+\+)|(\-\-)|(\:\:)|(\<\<)|(\>\>)|(\<\=)|(\>\=)|(\=\=)|(\!\=)|(\&\&)|(\|\|)");
            this.onewordopregs = new Regex(@"\.|\,|\?|\+|\-|\~|\!|\*|\/|\%|\<|\>|\&|\||\^");
            this.pararegs = new Regex(@"\[|\]|\(|\)");

            this.keywords = new string[]
            {
                "for", "foreach", "while", "if", "end", "let", "function",
                "int","int64_t","double","char","string","mod_t",
                "array","dual_array","set","map","priority_queue","segtree","bit"
            };
        }

        public List<ParsingTypes.LexerToken> Analize(string line, int index = 0, int end = -1, List<ParsingTypes.LexerToken> ret = null)
        {
            if (ret is null)
            {
                ret = new List<ParsingTypes.LexerToken>();
            }
            if (end == -1)
            {
                end = line.Length;
            }

            {
                var match = this.doublereg.Match(line, index, end - index);
                if (match.Success)
                {
                    double value = long.Parse(match.Groups["value"].Value);
                    var val2 = match.Groups["value2"].Value;
                    var value2 = val2 == "" ? 0 : long.Parse(val2) / Math.Pow(10.0, val2.Length);
                    return Next(NewDouble(value + value2), match, line, index, end, ret);
                }
            }
            {
                var match = this.int64reg.Match(line, index, end - index);
                if (match.Success)
                {
                    var value = long.Parse(match.Groups["value"].Value);
                    return Next(NewNumber(value), match, line, index, end, ret);
                }
            }
            {
                var match = this.stringreg.Match(line, index, end - index);
                if (match.Success)
                {
                    return Next(NewString(match.Groups["value"].Value), match, line, index, end, ret);
                }
            }
            {
                var match = this.charreg.Match(line, index, end - index);
                if (match.Success)
                {
                    return Next(NewChar(match.Groups["value"].Value[0]), match, line, index, end, ret);
                }
            }
            {
                var match = this.namereg.Match(line, index, end - index);
                if (match.Success)
                {
                    var name = match.Groups["value"].Value;
                    if (this.keywords.Contains(name))
                    {
                        return Next(NewKeyword(name), match, line, index, end, ret);
                    }
                    else
                    {
                        return Next(NewName(name), match, line, index, end, ret);
                    }
                }
            }
            {
                var match = this.twowordopregs.Match(line, index, end - index);
                if (match.Success)
                {
                    return Next(NewOperator(match.Value), match, line, index, end, ret);
                }
            }
            {
                var match = this.onewordopregs.Match(line, index, end - index);
                if (match.Success)
                {
                    return Next(NewOperator(match.Value), match, line, index, end, ret);
                }
            }
            {
                var match = this.pararegs.Match(line, index, end - index);
                if (match.Success)
                {
                    switch (match.Value[0])
                    {
                        case '(':
                            return Next(ParenthesisStart, match, line, index, end, ret);
                        case ')':
                            return Next(ParenthesisEnd, match, line, index, end, ret);
                        case '[':
                            return Next(BracketStart, match, line, index, end, ret);
                        case ']':
                            return Next(BracketEnd, match, line, index, end, ret);
                    }
                }
            }

            if (line.Substring(index, end - index).All(c => c == ' '))
            {
                return ret;
            }
            else
            {
                throw new ArgumentException($"{index} 不明なトークンが含まれています");
            }
        }

        public List<ParsingTypes.LexerToken> Next(ParsingTypes.LexerToken next, Match match, string line, int index, int end, List<ParsingTypes.LexerToken> ret)
        {
            Analize(line, index, match.Index, ret);
            ret.Add(next);
            return Analize(line, match.Index + match.Length, end, ret);
        }
    }
}
