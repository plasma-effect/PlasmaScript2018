using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ParsingTypes.LexerToken;
using static PlasmaScript.Utility;
using ParsingTypes;

namespace PlasmaScript
{
    public class Parser
    {
        string[][] leftbiop;//左結合二項演算子
        string[] monop;//一項演算子
        string[] typekeyword;//ref const const_ref

        public Parser()
        {
            this.leftbiop = new string[][]
            {
                new string[]{"*","/","%"},
                new string[]{"+","-"},
                new string[]{">>","<<"},
                new string[]{"<",">","<=",">="},
                new string[]{"!=","=="},
                new string[]{"&"},
                new string[]{"^"},
                new string[]{"|"},
                new string[]{"&&"},
                new string[]{"||"}
            };
            this.monop = new string[]
            {
                "++","--","~","!","-","+"
            };
            this.typekeyword = new string[]
            {
                "ref","const","const_ref"
            };
        }
        
        public ParsingTypes.ValueType ParseType(ArrayEnumerator<LexerToken> ts, ref bool isend)
        {
            if (isend)
            {
                throw MakeException("型名がありません");
            }
            var top = ts.GetNext(out isend);
            if(top is Name n)
            {
                var ret = ParseScopedName(ts, ref isend, ScopedName.NewAtomic(n.Item));
                if (isend)
                {
                    if (ret is ScopedName.Atomic b && this.typekeyword.Contains(b.Item)) 
                    {
                        throw MakeException("型名に使えないキーワードが含まれています");
                    }
                    return ParsingTypes.ValueType.NewAtomic(ret);
                }
                var next = ts.GetNext(out isend);
                if(next != CurlyStart)
                {
                    ts.MovePrev(out isend);
                    return ParsingTypes.ValueType.NewAtomic(ret);
                }
                var list = new List<ParsingTypes.ValueType>();
                while (!isend)
                {
                    list.Add(ParseType(ts, ref isend));
                    if (isend)
                    {
                        throw MakeException("\"{\" が閉じられていません");
                    }
                    next = ts.GetNext(out isend);
                    if (next == CurlyEnd)
                    {
                        if (ret is ScopedName.Atomic b && this.typekeyword.Contains(b.Item))
                        {
                            if (list.Count() != 1)
                            {
                                throw MakeException("\"{  }\"内の型が多すぎます");
                            }
                            if (b.Item =="ref")
                            {
                                return ParsingTypes.ValueType.NewRef(list[0]);
                            }
                            else if (b.Item == "const")
                            {
                                return ParsingTypes.ValueType.NewConst(list[0]);
                            }
                            else if (b.Item == "const_ref")
                            {
                                return ParsingTypes.ValueType.NewConstRef(list[0]);
                            }
                        }
                        return ParsingTypes.ValueType.NewTemplate(ret, list);
                    }
                    else if(next is Operator op && op.Item == ",")
                    {
                        continue;
                    }
                    else
                    {
                        throw MakeException("不明なトークンが含まれています");
                    }
                }
                throw MakeException("\"{\" が閉じられていません");
            }
            else if(top == BracketStart)
            {
                var list = new List<ParsingTypes.ValueType>();
                while (true)
                {
                    list.Add(ParseType(ts, ref isend));
                    if (isend)
                    {
                        throw MakeException("[ が閉じられていません");
                    }
                    top = ts.GetNext(out isend);
                    if(IsComma(top))
                    {
                        continue;
                    }
                    else if (top == BracketEnd)
                    {
                        return ParsingTypes.ValueType.NewTuple(list);
                    }
                    else
                    {
                        throw MakeException("不明なトークンが含まれています");
                    }
                }
            }
            throw MakeException("型名でないトークンです");
        }

        private ScopedName ParseScopedName(ArrayEnumerator<LexerToken> ts, ref bool isend, ScopedName ret)
        {
            if (isend)
            {
                return ret;
            }
            while (!isend)
            {
                var next = ts.GetNext(out isend);
                if (next is Operator op && op.Item == "::")
                {
                    if (isend)
                    {
                        throw MakeException("名前として成立しません");
                    }
                    next = ts.GetNext(out isend);
                    if (next is Name m)
                    {
                        ret = ScopedName.NewScope(ret, m.Item);
                    }
                    else
                    {
                        throw MakeException("型名として成立しません");
                    }
                }
                else
                {
                    ts.MovePrev(out isend);
                    break;
                }
            }
            return ret;
        }

        public Expr ParseExpr(ArrayEnumerator<LexerToken> ts, ref bool isend)
        {
            return ParseRightBiop(ts, ref isend);
        }

        private Expr ParseRightBiop(ArrayEnumerator<LexerToken> ts, ref bool isend)
        {
            var left = ParseLeftBiop(ts, ref isend, this.leftbiop.Length - 1);
            if (isend)
            {
                return left;
            }
            var top = ts.GetNext(out isend);
            if(top is AssignOperator op)
            {
                if (isend)
                {
                    throw MakeException("式として成立しません");
                }
                return Expr.NewBiOperator(op.Item, left, ParseRightBiop(ts, ref isend));
            }
            else
            {
                ts.MovePrev(out isend);
            }
            return left;
        }

        private Expr ParseLeftBiop(ArrayEnumerator<LexerToken> ts, ref bool isend, int dep)
        {
            if (dep == -1)
            {
                return ParseMonop(ts, ref isend);
            }
            var ret = ParseLeftBiop(ts, ref isend, dep - 1);
            while (true)
            {
                if (isend)
                {
                    return ret;
                }
                var top = ts.GetNext(out isend);
                if(top is Operator op && this.leftbiop[dep].Contains(op.Item))
                {
                    if (isend)
                    {
                        throw MakeException("式として成立しません");
                    }
                    ret = Expr.NewBiOperator(op.Item, ret, ParseLeftBiop(ts, ref isend, dep - 1));
                }
                else
                {
                    ts.MovePrev(out isend);
                    return ret;
                }
            }
        }

        private Expr ParseMonop(ArrayEnumerator<LexerToken> ts, ref bool isend)
        {
            if (isend)
            {
                throw MakeException("式として成立しません");
            }
            var top = ts.GetNext(out isend);
            if (top is Operator op && this.monop.Contains(op.Item))
            {
                return Expr.NewMonoOperator(op.Item, ParseMonop(ts, ref isend));
            }
            ts.MovePrev(out isend);
            return ParseFunction(ts, ref isend);
        }

        private Expr ParseFunction(ArrayEnumerator<LexerToken> ts, ref bool isend)
        {
            Expr ret;
            var top = ts.GetNext(out isend);
            if (top is Operator op && op.Item == "?")
            {
                top = ts.GetNext(out isend);
                if (top == ParenthesisStart)
                {
                    var list = ParseFunctionCall(ts, out isend, out top);
                    if (list.Count != 3)
                    {
                        throw MakeException("(  )の中の要素が3ではありません");
                    }
                    else
                    {
                        ret = Expr.NewTriOperator(list[0], list[1], list[2]);
                    }
                }
                else
                {
                    throw MakeException("式として成立しません");
                }
            }
            else
            {
                ts.MovePrev(out isend);
                ret = ParseAtomic(ts, ref isend);
            }
            if (isend)
            {
                return ret;
            }
            while (true)
            {
                if (isend)
                {
                    break;
                }
                top = ts.GetNext(out isend);
                
                if(top == ParenthesisStart)
                {
                    ret = Expr.NewFunctionExpr(ret, ParseFunctionCall(ts, out isend, out top));
                }
                else if (top == BracketStart)
                {
                    top = ParseIndexCall(ts, ref isend, ref ret);
                }
                else
                {
                    ts.MovePrev(out isend);
                    break;
                }
            }
            return ret;
        }

        private LexerToken ParseIndexCall(ArrayEnumerator<LexerToken> ts, ref bool isend, ref Expr ret)
        {
            LexerToken top;
            ret = Expr.NewIndexExpr(ret, ParseExpr(ts, ref isend));
            if (isend)
            {
                throw MakeException("[ が閉じられていません");
            }
            top = ts.GetNext(out isend);
            if (top != BracketEnd)
            {
                throw MakeException("[ が閉じられていません");
            }

            return top;
        }

        private List<Expr> ParseFunctionCall(ArrayEnumerator<LexerToken> ts, out bool isend, out LexerToken top)
        {
            var list = new List<Expr>();
            top = ts.GetNext(out isend);
            if (top != ParenthesisEnd)
            {
                ts.MovePrev(out isend);
                while (true)
                {
                    list.Add(ParseExpr(ts, ref isend));
                    if (isend)
                    {
                        throw MakeException("( が閉じられていません");
                    }
                    top = ts.GetNext(out isend);
                    if (IsComma(top))
                    {
                        continue;
                    }
                    else if (top == ParenthesisEnd)
                    {
                        break;
                    }
                    else
                    {
                        throw MakeException("式として成立しません");
                    }
                }
            }
            return list;
        }

        private Expr ParseAtomic(ArrayEnumerator<LexerToken> ts, ref bool isend)
        {
            var top = ts.GetNext(out isend);
            if (top == ParenthesisStart)
            {
                return ParseParenthesis(ts, ref isend, out top);
            }
            else if (top == BracketStart)
            {
                return ParseTupleExpr(ts, ref isend, out top);
            }
            else
            {
                switch (top)
                {
                    case Number num:
                        return Expr.NewNumLiteral(num.Item);
                    case LexerToken.Double db:
                        return Expr.NewDoubleLiteral(db.Item);
                    case LexerToken.String st:
                        return Expr.NewStringLiteral(st.Item);
                    case LexerToken.Char c:
                        return Expr.NewCharLiteral(c.Item);
                    case Name n:
                        return Expr.NewNameExpr(ParseScopedName(ts, ref isend, ScopedName.NewAtomic(n.Item)));
                }
                throw MakeException("式として成立しません");
            }
        }

        private Expr ParseTupleExpr(ArrayEnumerator<LexerToken> ts, ref bool isend, out LexerToken top)
        {
            var list = new List<Expr>();
            while (true)
            {
                list.Add(ParseExpr(ts, ref isend));
                top = ts.GetNext(out isend);
                if (IsComma(top))
                {
                    continue;
                }
                else if (top == BracketEnd)
                {
                    return Expr.NewTupleExpr(list);
                }
                else
                {
                    throw MakeException("式として成立しません");
                }
            }
        }

        private Expr ParseParenthesis(ArrayEnumerator<LexerToken> ts, ref bool isend, out LexerToken top)
        {
            var ret = ParseExpr(ts, ref isend);
            if (isend)
            {
                throw MakeException("( が閉じられていません");
            }
            top = ts.GetNext(out isend);
            if (top != ParenthesisEnd)
            {
                throw MakeException("( が閉じられていません");
            }
            return ret;
        }

        private bool IsComma(LexerToken token)
        {
            return token is Operator op && op.Item == ",";
        }

        public Line ParseLine(ArrayEnumerator<LexerToken> ts, ref bool isend)
        {
            if (isend)
            {
                return Line.NoneLine;
            }
            var top = ts.GetNext(out isend);
            if(top is Name n)
            {
                switch (n.Item)
                {
                    case "function":
                        return ParseFunctionDefine(ts, ref isend);
                    case "return":
                        return ParseReturnAction(ts, ref isend);
                }
            }
        }

        private Line ParseFunctionDefine(ArrayEnumerator<LexerToken> ts, ref bool isend)
        {

        }

        private Line ParseReturnAction(ArrayEnumerator<LexerToken> ts, ref bool isend)
        {
            var expr = ParseExpr(ts, ref isend);
            if (!isend)
            {
                throw MakeException("余計な文字列が含まれています");
            }
            return Line.NewReturnAction(expr);
        }

        private Line ParseActionDefine(ArrayEnumerator<LexerToken> ts, ref bool isend)
        {

        }

        private Line ParseValueDefine(ArrayEnumerator<LexerToken> ts, ref bool isend)
        {

        }

        private Line ParseForDefine(ArrayEnumerator<LexerToken> ts, ref bool isend)
        {

        }

        private Line ParseWhileDefine(ArrayEnumerator<LexerToken> ts, ref bool isend)
        {

        }
    
        private Line ParseIfDefine(ArrayEnumerator<LexerToken> ts, ref bool isend)
        {

        }

        private Line ParseElifDefine(ArrayEnumerator<LexerToken> ts, ref bool isend)
        {

        }

        private Line ParseCoroutineDefine(ArrayEnumerator<LexerToken> ts, ref bool isend)
        {

        }

        private Line ParseCoroutineAction(ArrayEnumerator<LexerToken> ts, ref bool isend)
        {

        }

        private Line ParseYieldReturn(ArrayEnumerator<LexerToken> ts,ref bool isend)
        {

        }
    }
}
