using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ParsingTypes.LexerToken;
using ParsingTypes;

namespace PlasmaScript
{
    public class Parser
    {
        List<string>[] biop;
        string[] monop;

        public Parser()
        {
            this.biop = new List<string>[10];
            this.biop[9] = new List<string> { "||" };
            this.biop[8] = new List<string> { "&&" };
            this.biop[7] = new List<string> { "|" };
            this.biop[6] = new List<string> { "^" };
            this.biop[5] = new List<string> { "&" };
            this.biop[4] = new List<string> { "==", "!=" };
            this.biop[3] = new List<string> { "<=", ">=", "<", ">" };
            this.biop[2] = new List<string> { "<<", ">>" };
            this.biop[1] = new List<string> { "+", "-" };
            this.biop[0] = new List<string> { "*", "/", "%" };
            this.monop = new string[] { "~", "!", "+", "-", "++", "--" };
        }

        public Line Parsing(List<LexerToken> line)
        {
            if (line.Count == 0)
            {
                return Line.None;
            }
            if (line[0] is Keyword keyword)
            {
                var item = keyword.Item;
                if (item == "function")
                {
                    return FunctionParse(line);
                }
                else if (item == "let")
                {
                    return LetParse(line);
                }
                else if (item == "for")
                {

                }
                else if (item == "foreach")
                {

                }
                else if (item == "while")
                {

                }
            }
            throw new Exception("工事中");
        }

        private Line FunctionParse(List<LexerToken> line)
        {
            if (line.Count < 4)
            {
                throw new ArgumentException($"構文が間違っています");
            }
            if (line[1] is LexerToken.Name n && line[2] == ParenthesisStart) 
            {
                var fname = n.Item;
                var args = new List<Tuple<string, ParsingTypes.ValueType>>();
                var index = 3;
                var ret = ParsingTypes.ValueType.Void;
                try
                {
                    while (true)
                    {
                        if (line[index] is LexerToken.Name a)
                        {
                            var name = a.Item;
                            ++index;
                            if (line[index] != TypeSig)
                            {
                                throw new ArgumentException($"型名を追加してください");
                            }
                            ++index;
                            args.Add(Tuple.Create(name, TypeParse(line, ref index)));
                            if (line[index] == ParenthesisEnd)
                            {
                                ++index;
                                break;
                            }
                            else if (line[index] is Operator op && op.Item == ",")
                            {
                                ++index;
                                continue;
                            }
                            else
                            {
                                throw new ArgumentException("不明なトークンが含まれています");
                            }
                        }
                    }
                    if (index != line.Count)
                    {
                        if (line[index] != TypeSig)
                        {
                            throw new ArgumentException("不明なトークンが含まれています");
                        }
                        ++index;
                        ret = TypeParse(line, ref index);
                        if (index != line.Count)
                        {
                            throw new ArgumentException("不明なトークンが含まれています");
                        }
                    }
                    return Line.NewFunctionDefine(n.Item, args, ret);
                }
                catch (IndexOutOfRangeException)
                {
                    throw new ArgumentException("構文が間違っています");
                }
            }
            else
            {
                throw new ArgumentException($"関数名ではありません");
            }
        }

        private Line LetParse(List<LexerToken> line)
        {
            try
            {
                var val = new List<string>();
                var index = 1;
                if (line[index] == BracketStart)
                {
                    ++index;
                    while (true)
                    {
                        if(line[index] is LexerToken.Name n)
                        {
                            ++index;
                            val.Add(n.Item);
                        }
                        if(line[index] is Operator op && op.Item == ",")
                        {
                            ++index;
                            continue;
                        }
                        else if (line[index] == ParenthesisEnd)
                        {
                            ++index;
                            break;
                        }
                        else
                        {
                            throw new ArgumentException("変数定義が間違っています");
                        }
                    }
                }
                else if(line[index] is LexerToken.Name n)
                {
                    ++index;
                    val.Add(n.Item);
                }
                if (val.Count == 0)
                {
                    throw new ArgumentException("変数定義が間違っています");
                }
                if(line[index] is Operator ope && ope.Item == "=")
                {
                    ++index;
                    return Line.NewValueDefine(val, ExprParse(line, ref index));
                }
                throw new ArgumentException("異常な式です");
            }
            catch (IndexOutOfRangeException)
            {
                throw new ArgumentException("構文が間違っています");
            }
        }

        private ParsingTypes.ValueType TypeParse(List<LexerToken> line, ref int index)
        {
            if(line[index] is LexerToken.Name name)
            {
                ++index;
                if (line[index] == CurlyStart)
                {
                    ++index;
                    var list = new List<ParsingTypes.ValueType>();
                    while (index != line.Count)
                    {
                        list.Add(TypeParse(line, ref index));
                        if (line[index] == CurlyEnd)
                        {
                            ++index;
                            break;
                        }
                        else if (line[index] is Operator op && op.Item == ",") 
                        {
                            ++index;
                            continue;
                        }
                        else
                        {
                            throw new ArgumentException("型名として認識されません");
                        }
                    }
                    return ParsingTypes.ValueType.NewTemplate(name.Item, list);
                }
                else
                {
                    return ParsingTypes.ValueType.NewAtomic(name.Item);
                }
            }
            else
            {
                throw new ArgumentException("型名として認識されません");
            }
        }
        
        private Expr ExprParse(List<LexerToken> line, ref int index)
        {
            return BiOperatorParse(line, ref index);
        }

        private Expr BiOperatorParse(List<LexerToken> line, ref int index, int rank = 9)
        {
            if (rank == -1)
            {
                return MonoOperatorParse(line, ref index);
            }
            var expr = BiOperatorParse(line, ref index, rank - 1);
            while (index != line.Count) 
            {
                if(line[index] is Operator op && this.biop[rank].Contains(op.Item))
                {
                    ++index;
                    expr = Expr.NewBiOperator(op.Item, expr, BiOperatorParse(line, ref index, rank - 1));
                }
                else
                {
                    break;
                }
            }
            return expr;
        }

        private Expr MonoOperatorParse(List<LexerToken> line,ref int index)
        {
            if(line[index] is Operator op && this.monop.Contains(op.Item))
            {
                ++index;
                return Expr.NewMonoOperator(op.Item, MonoOperatorParse(line, ref index));
            }
            return FunctionExprParse(line, ref index);
        }

        private Expr FunctionExprParse(List<LexerToken> line, ref int index)
        {
            if(line[index] is Operator op && op.Item == "?")
            {
                ++index;
                return TriOperatorParse(line, ref index);
            }
            var expr = BaseParse(line, ref index);
            while (index != line.Count)
            {
                if (line[index] is Operator op2 && op2.Item == "." && line[index + 1] is LexerToken.Name name) 
                {
                    index += 2;
                    expr = Expr.NewMember(expr, name.Item);
                }
                else
                {
                    break;
                }
            }
            while (index != line.Count)
            {
                if (line[index] == ParenthesisStart)
                {
                    ++index;
                    var arg = new List<Expr>();
                    if (line[index] == ParenthesisEnd)
                    {
                        ++index;
                    }
                    else
                    {
                        arg.Add(ExprParse(line, ref index));
                        while (index != line.Count)
                        {
                            if(line[index] == ParenthesisEnd)
                            {
                                ++index;
                                break;
                            }
                            else if(line[index] is Operator op3 && op3.Item == ",")
                            {
                                ++index;
                                arg.Add(ExprParse(line, ref index));
                            }
                            else
                            {
                                throw new ArgumentException("式が異常です");
                            }
                        }
                    }
                    expr = Expr.NewFunction(expr, arg);
                }
                else if(line[index] == BracketStart)
                {
                    ++index;
                    expr = Expr.NewIndexer(expr, ExprParse(line, ref index));
                    if(line[index] == BracketEnd)
                    {
                        ++index;
                    }
                    else
                    {
                        throw new ArgumentException("式が異常です");
                    }
                }
                else
                {
                    break;
                }
            }
            return expr;
        }

        private Expr TriOperatorParse(List<LexerToken> line,ref int index)
        {
            if(line[index] == ParenthesisStart)
            {
                ++index;
                var cond = ExprParse(line, ref index);
                if(line[index] is Operator op && op.Item == ",")
                {
                    ++index;
                    var t = ExprParse(line, ref index);
                    if(line[index]is Operator op2 && op2.Item == ",")
                    {
                        ++index;
                        var f = ExprParse(line, ref index);
                        if (line[index] == ParenthesisEnd)
                        {
                            ++index;
                            return Expr.NewTriOperator(cond, t, f);
                        }
                    }
                }
            }
            throw new ArgumentException("異常な式です");
        }

        private Expr BaseParse(List<LexerToken> line, ref int index)
        {
            if(line[index] == ParenthesisStart)
            {
                ++index;
                var ret = ExprParse(line, ref index);
                if (line[index] == ParenthesisEnd)
                {
                    ++index;
                    return ret;
                }
            }
            else if(line[index] is LexerToken.Name n)
            {
                ++index;
                var name = ParsingTypes.Name.NewAtomic(n.Item);
                while (index != line.Count)
                {
                    if (line[index] is Operator op && op.Item == "::" && line[index + 1] is LexerToken.Name next)
                    {
                        index += 2;
                        name = ParsingTypes.Name.NewScope(name, next.Item);
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                return Expr.NewValue(name);
            }
            else if(line[index] is Number num)
            {
                ++index;
                return Expr.NewNumLiteral(num.Item);
            }
            else if(line[index] is LexerToken.Double dob)
            {
                ++index;
                return Expr.NewDoubleLiteral(dob.Item);
            }
            else if(line[index] is LexerToken.Char ch)
            {
                ++index;
                return Expr.NewCharLiteral(ch.Item);
            }
            else if(line[index] is LexerToken.String str)
            {
                ++index;
                return Expr.NewStringLiteral(str.Item);
            }
            throw new ArgumentException("異常な式です");
        }
    }
}
