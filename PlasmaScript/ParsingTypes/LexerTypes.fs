namespace ParsingTypes
    type LexerToken =
        | Double of double //���������_��
        | Number of int64 //64bit����
        | String of string //������
        | Char of char //����
        | BracketStart //[
        | BracketEnd //]
        | ParenthesisStart //(
        | ParenthesisEnd //)
        | CurlyStart //{
        | CurlyEnd //}
        | TypeSig //:
        | Name of string //���O
        | Operator of string //���Z�q
        | AssignOperator of string//������Z�q