using System;
using System.Collections.Generic;
using System.IO;
using ShellScript.Core.Language.CompilerServices.Lexing;
using ShellScript.Core.Language.CompilerServices.Parsing;
using ShellScript.Core.Language.CompilerServices.Statements;

namespace ShellScript.Core.Language.CompilerServices
{
    public class Compiler
    {
        private readonly Lexer _lexer;
        private readonly Parser _parser;

        public Compiler()
        {
            _lexer = new Lexer();
            _parser = new Parser(_lexer);
        }


        public CompilationResult CompileFromSource(
            string sourceCodePath,
            //string[] include
            string outputFilePath,
            string platformInfo,
            
            bool flagSemicolonRequired
        )
        {
            if (!File.Exists(sourceCodePath))
            {
                return new CompilationResult(false)
                {
                    Exception = new FileNotFoundException("Source code file not found.", sourceCodePath)
                };
            }

            IStatement[] statements;

            try
            {
                using (var inputFile = new FileStream(sourceCodePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = new StreamReader(inputFile))
                using (var outputFile =
                    new FileStream(outputFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(outputFile))
                {
                    try
                    {
                        var info = new ParserInfo(flagSemicolonRequired, Path.GetFileName(sourceCodePath), sourceCodePath);
                        foreach (var statement in _parser.Parse(reader, info))
                        {
                            CompileSdk(statement, writer);
                        }
                        
                        return new CompilationResult(true);
                    }
                    catch (ParserException ex)
                    {
                        return new CompilationResult(false)
                        {
                            Exception = ex
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new CompilationResult(false)
                {
                    Exception = ex
                };
            }
        }

        private void CompileSdk(
            IStatement statements,
            TextWriter writer
        )
        {
        }
    }
}