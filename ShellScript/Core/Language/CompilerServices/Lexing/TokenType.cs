namespace ShellScript.Core.Language.CompilerServices.Lexing
{
    public enum TokenType
    {
        NotDefined,
        Invalid,
        
        AndLogical,  //&&
        And,  //&
        OrLogical, //||
        Or, //|
        
        OpenParenthesis, //(
        CloseParenthesis, //)
        
        OpenBrace, //{
        CloseBrace, //}
        
        OpenBracket, //[
        CloseBracket, //]
        
        Dot, //.
        Comma, //,
        //DateTimeValue, //
        
        Equals, //==
        NotEquals, //!=
        GreaterEqual, //>=
        Greater, //>
        LessEqual, //<=
        Less, //<
        Assignment, //=
        
        Not, //!
        BitwiseNot, //~
        
        Minus, //-
        Plus, //+
        Asterisk, //*
        Division, // /
        BackSlash, // \
        Increment, // ++
        Decrement, // --
        Reminder, // %
        
        Throw, //throw
        Async, //async
        Await, //await
        In, //in
        NotIn, //notin
        
        If, //if
        Else, //else
        
        PreprocessorIf, //#if
        PreprocessorElseIf, //#elseif
        PreprocessorElse, //#else
        PreprocessorEndIf, //#endif
        
        For, //for
        ForEach, //foreach
        While, //while
        Do, //do
        Loop, //loop
        
        //Function, //function
        Class, //class
        Return, //return
        
        Like, //like
        NotLike, //notlike
        Call, //call
        
        DataType, //var,int,double,float,long,byte,char,object,variant,void
        Null, //null
        
        Echo,
        Number,
        StringValue1,
        StringValue2,
        True,
        False,
        
        SequenceTerminator, //;
        SequenceTerminatorNewLine, //CRLF
        Comment, // //
        MultiLineCommentOpen, // /*
        MultiLineCommentClose, // */
        
        IdentifierName,
    }
}