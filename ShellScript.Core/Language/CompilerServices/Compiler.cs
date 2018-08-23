using System;
using System.IO;
using System.Runtime.ExceptionServices;
using ShellScript.Core.Language.CompilerServices.Lexing;
using ShellScript.Core.Language.CompilerServices.Parsing;
using ShellScript.Core.Language.CompilerServices.Statements;
using ShellScript.Core.Language.CompilerServices.Transpiling;

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
            try
            {
                if (!File.Exists(sourceCodePath))
                {
                    throw new FileNotFoundException("Source code file not found.", sourceCodePath);
                }

                var platform = Platforms.GetPlatformByName(platformInfo);
                if (platform == null)
                {
                    throw new Exception("Invalid platform name.");
                }

                using (var inputFile = new FileStream(sourceCodePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = new StreamReader(inputFile))
                using (var outputFile =
                    new FileStream(outputFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                using (var metaOutputFile = new FileStream(
                    Path.GetFileNameWithoutExtension(outputFilePath) + "meta" + Path.GetExtension(outputFilePath),
                    FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                using (var outputWriter = new StreamWriter(outputFile))
                using (var metaWriter = new StreamWriter(metaOutputFile))
                {
                    try
                    {
                        var info = new ParserInfo(flagSemicolonRequired,
                            sourceCodePath,
                            Path.GetFileName(sourceCodePath),
                            sourceCodePath);

                        foreach (var statement in _parser.Parse(reader, info))
                        {
                            Compile(statement, outputWriter, metaWriter, platform);
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
                var dispatchInfo = ExceptionDispatchInfo.Capture(ex);
                return new CompilationResult(false)
                {
                    Exception = dispatchInfo.SourceException
                };
            }
        }

        private void Compile(
            IStatement statement,
            TextWriter outputWriter,
            TextWriter metatWriter,
            IPlatform platform
        )
        {
            var compileContext = new Context(metatWriter, outputWriter, platform);
            outputWriter.WriteLine(statement.GetType().Name);
            metatWriter.WriteLine(statement.GetType().Name);
        }
    }
}