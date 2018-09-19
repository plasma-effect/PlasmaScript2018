namespace ParsingTypes
    type LexerToken =
        | Double of double
        | Number of int64
        | String of string
        | Char of char
        | BracketStart
        | BracketEnd
        | ParenthesisStart
        | ParenthesisEnd
        | Keyword of string
        | Name of string
        | Operator of string;;