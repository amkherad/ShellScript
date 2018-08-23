//using System;
//using System.IO;
//using ShellScript.Core.Language.CompilerServices.Transpiling;
//using ShellScript.Core.Language.CompilerServices.Statements;
//
//namespace ShellScript.Unix.Bash.PlatformTranspiler
//{
//    public class IfElseStatementTranspiler : IPlatformStatementTranspiler
//    {
//        public Type StatementType => typeof(IfElseStatement);
//        public bool CanInline(Context context, Scope scope, IStatement statement)
//        {
//            throw new NotImplementedException();
//        }
//
//        public void WriteInline(Context context, Scope scope, TextWriter writer, IStatement statement)
//        {
//            throw new NotImplementedException();
//        }
//
//        public void WriteBlock(Context context, Scope scope, TextWriter writer, IStatement statement)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}