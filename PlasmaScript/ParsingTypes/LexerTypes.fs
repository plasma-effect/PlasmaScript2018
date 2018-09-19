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
        | Keyword of string //�L�[���[�h
        | Name of string //���O
        | Operator of string //���Z�q