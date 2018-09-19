namespace ParsingTypes
    type LexerToken =
        | Double of double //浮動小数点数
        | Number of int64 //64bit整数
        | String of string //文字列
        | Char of char //文字
        | BracketStart //[
        | BracketEnd //]
        | ParenthesisStart //(
        | ParenthesisEnd //)
        | CurlyStart //{
        | CurlyEnd //}
        | TypeSig //:
        | Keyword of string //キーワード
        | Name of string //名前
        | Operator of string //演算子