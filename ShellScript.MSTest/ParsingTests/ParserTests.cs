using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellScript.Core.Language.CompilerServices.Parsing;

namespace ShellScript.MSTest.ParsingTests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void ParseFunction()
        {
            var code = "function test(int x) { echo; }";
            var parser = new Parser();

            var statements = parser.Parse(new StringReader(code), new ParserInfo(true, "", "")).ToList();

            GC.KeepAlive(statements);

            Assert.IsNotNull(statements);
        }
        
        [TestMethod]
        public void ParseEvaluation()
        {
            var code = "var x = (46 * 34) - 23 / 4";
            var parser = new Parser();

            var statements = parser.Parse(new StringReader(code), new ParserInfo(true, "", "")).ToList();

            GC.KeepAlive(statements);

            Assert.IsNotNull(statements);
        }
    }
}

git add ShellScript.Core/Language/CompilerServices/Statements/Operators/AdditionOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/ArithmeticOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/BitwiseAndOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/BitwiseOrOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/DecrementOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/DivisionOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/EqualOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/GreaterEqualOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/GreaterOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/IOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/IncrementOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/LessEqualOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/LessOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/LogicalAndOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/LogicalOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/LogicalOrOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/ModulusOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/MultiplicationOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/NotEqualOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/ReminderOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/SubtractionOperator.cs
git add ShellScript.Core/Language/CompilerServices/Statements/Operators/XorOperator.cs