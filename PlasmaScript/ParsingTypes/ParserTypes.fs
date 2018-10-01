namespace ParsingTypes
    type ScopedName = 
        | Atomic of string
        | Scope of ScopedName * string
    type ValueType =
        | Atomic of ScopedName
        | Ref of ValueType
        | Const of ValueType
        | ConstRef of ValueType
        | Template of ScopedName*System.Collections.Generic.List<ValueType>
        | Tuple of System.Collections.Generic.List<ValueType>
    type Expr =
        | NumLiteral of int64
        | DoubleLiteral of double
        | StringLiteral of string
        | CharLiteral of char
        | FunctionExpr of Expr*System.Collections.Generic.List<Expr>
        | IndexExpr of Expr*Expr
        | TupleExpr of System.Collections.Generic.List<Expr>
        | MonoOperator of string*Expr
        | BiOperator of string*Expr*Expr
        | TriOperator of Expr*Expr*Expr
        | NameExpr of ScopedName
        | MemberExpr of Expr*string
    type Line =
        | FunctionDefine of string*System.Collections.Generic.List<ValueType*string>*ValueType
        | ReturnAction of Expr
        | ActionDefine of string*System.Collections.Generic.List<ValueType*string>
        | ValueDefine of System.Collections.Generic.List<string>*Expr
        | ForDefine of System.Collections.Generic.List<string>*Expr
        | WhileDefine of Expr
        | BreakAction
        | IfDefine of Expr
        | ElifDefine of Expr
        | ElseAction
        | CoroutineDefine of string*System.Collections.Generic.List<ValueType*string>*ValueType
        | CoroutineAction of string*System.Collections.Generic.List<ValueType*string>
        | YieldReturn of Expr
        | YieldBreak
        | Expression of Expr
        | NoneLine