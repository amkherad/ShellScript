using System;
using System.IO;
using System.Runtime.ExceptionServices;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Parsing;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Lexing;

namespace ShellScript.Core.Language.Compiler
{
    public class Compiler
    {
        public CompilationResult CompileFromSource(
            TextWriter errorWriter,
            TextWriter warningWriter,
            TextWriter logWriter,
            string sourceCodePath,
            //string[] include
            string outputFilePath,
            string platformInfo,
            CompilerFlags flags
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

                flags = platform.ReviseFlags(flags);

                var outputObjPath = Path.Combine(Path.GetDirectoryName(outputFilePath), "obj");

                if (!Directory.Exists(outputObjPath))
                {
                    Directory.CreateDirectory(outputObjPath);
                }

                CompileFromSource(sourceCodePath, outputObjPath, outputFilePath, platform, flags, errorWriter,
                    warningWriter, logWriter);

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

        public static void CompileFromSource(string sourceCodePath, string outputObjPath, string outputFilePath,
            IPlatform platform, CompilerFlags flags, TextWriter errorWriter, TextWriter warningWriter,
            TextWriter logWriter)
        {
            var tempSrcPath = Path.Combine(outputObjPath,
                Path.GetFileNameWithoutExtension(outputFilePath) + ".src" + Path.GetExtension(outputFilePath) + ".bin");
            var tempMetaPath = Path.Combine(outputObjPath,
                Path.GetFileNameWithoutExtension(outputFilePath) + ".meta" + Path.GetExtension(outputFilePath) +
                ".bin");

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

                var context = new Context(platform, flags, errorWriter, warningWriter, logWriter);
                var scope = context.GeneralScope;

                platform.Api.InitializeContext(context);

                var metaInfo = context.GetMetaInfoTranspiler();
                metaInfo.WritePrologue(context, metaWriter);

                var info = new ParserContext(
                    warningWriter,
                    logWriter,
                    flags.SemicolonRequired,
                    sourceCodePath,
                    Path.GetFileName(sourceCodePath),
                    sourceCodePath);

                var parser = new Parser(context);

                foreach (var statement in parser.Parse(reader, info))
                {
                    Transpile(context, scope, statement, codeWriter, metaWriter);
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

                if (context.Flags.UseSegments)
                {
                    context.GetMetaInfoTranspiler().WriteSeparator(context, outputWriter);
                }

                while ((line = codeReader.ReadLine()) != null)
                {
                    outputWriter.WriteLine(line);
                }
            }
        }

        public static void Transpile(
            Context context,
            Scope scope,
            IStatement statement,
            TextWriter outputWriter,
            TextWriter metaWriter
        )
        {
            var transpiler = context.GetTranspilerForStatement(statement);

            if (!transpiler.Validate(context, scope, statement, out var message))
            {
                throw new CompilerException($"{message} {statement.Info}", statement.Info);
            }

            transpiler.WriteBlock(context, scope, outputWriter, metaWriter, statement);
        }
    }
}