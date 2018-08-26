using System;
using System.IO;
using System.Runtime.ExceptionServices;
using ShellScript.Core.Language.CompilerServices.CompilerErrors;
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

                var outputPath = Path.Combine(Path.GetDirectoryName(outputFilePath), "obj");

                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }
                
                var tempSrcPath = Path.Combine(outputPath,
                    Path.GetFileNameWithoutExtension(outputFilePath) + ".src" + Path.GetExtension(outputFilePath) + ".bin");
                var tempMetaPath = Path.Combine(outputPath,
                    Path.GetFileNameWithoutExtension(outputFilePath) + ".meta" + Path.GetExtension(outputFilePath) + ".bin");

                using (var inputFile = new FileStream(sourceCodePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = new StreamReader(inputFile))

                using (var codeOutputFile = new FileStream(tempSrcPath,
                    FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                using (var metaOutputFile = new FileStream(tempMetaPath,
                    FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))

                using (var codeWriter = new StreamWriter(codeOutputFile))
                using (var metaWriter = new StreamWriter(metaOutputFile))

                using (var outputFile = new FileStream(outputFilePath,
                    FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                using (var outputWriter = new StreamWriter(outputFile))
                {
                    codeOutputFile.SetLength(0);
                    metaOutputFile.SetLength(0);

                    var context = new Context(platform);
                    var scope = context.GeneralScope;

                    var metaInfo = context.GetMetaInfoTranspiler();
                    metaInfo.WritePrologue(context, metaWriter);

                    var info = new ParserInfo(flagSemicolonRequired,
                        sourceCodePath,
                        Path.GetFileName(sourceCodePath),
                        sourceCodePath);

                    foreach (var statement in _parser.Parse(reader, info))
                    {
                        Transpile(context, scope, statement, codeWriter, metaWriter, platform);
                    }
                    
                    codeWriter.Flush();
                    metaWriter.Flush();
                    
                    codeOutputFile.Flush();
                    metaOutputFile.Flush();

                    codeOutputFile.Position = 0;
                    metaOutputFile.Position = 0;

                    var codeReader = new StreamReader(codeOutputFile);
                    var metaReader = new StreamReader(metaOutputFile);

                    outputFile.SetLength(0);
                    
                    string line;
                    while ((line = metaReader.ReadLine()) != null)
                    {
                        outputWriter.WriteLine(line);
                    }

                    while ((line = codeReader.ReadLine()) != null)
                    {
                        outputWriter.WriteLine(line);
                    }
                }

                return new CompilationResult(true);
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

        private void Transpile(
            Context context,
            Scope scope,
            IStatement statement,
            TextWriter outputWriter,
            TextWriter metaWriter,
            IPlatform platform
        )
        {
            var transpiler = context.GetTranspilerForStatement(statement);

            if (!transpiler.Validate(context, scope, statement, out var message))
            {
                throw new CompilerException(message, statement.Info);
            }

            transpiler.WriteBlock(context, scope, outputWriter, statement);
        }
    }
}