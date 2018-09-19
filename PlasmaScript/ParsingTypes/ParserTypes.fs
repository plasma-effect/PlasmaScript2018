namespace ParsingTypes
    open System.Collections.Generic;
    type Name =
        | Scope of Name * string
        | Atomic of string
    type Expr =
        | BiOperator of string * Expr * Expr
        | TriOperator of Expr * Expr * Expr
        | MonoOperator of string * Expr
        | Function of Expr * List<Expr>
        | Indexer of Expr * Expr
        | Member of Expr * string
        | Value of Name
        | NumLiteral of int64
        | StringLiteral of string
        | DoubleLiteral of double
        | CharLiteral of char
    type ValueType =
        | Atomic of string
        | Template of string * List<ValueType>
        | Void
    type Line =
        | FunctionDefine of string * List<string*ValueType> * ValueType
        | ValueDefine of List<string> * Expr
        | ValueAssign of string * Expr
        | ForeachStart of List<string> * Expr
        | IfStart of Expr
        | WhileStart of Expr
        | Expression of Expr
        | End
        | None