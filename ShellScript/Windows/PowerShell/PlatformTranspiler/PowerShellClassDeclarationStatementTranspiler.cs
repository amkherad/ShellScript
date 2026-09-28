using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShellScript.Core.Language.Compiler;
using ShellScript.Core.Language.Compiler.CompilerErrors;
using ShellScript.Core.Language.Compiler.Statements;
using ShellScript.Core.Language.Compiler.Transpiling;
using ShellScript.Core.Language.Compiler.Transpiling.BaseImplementations;
using ShellScript.Core.Language.Library;

namespace ShellScript.Windows.PowerShell.PlatformTranspiler
{
    public class PowerShellClassDeclarationStatementTranspiler : StatementTranspilerBase, IPlatformStatementTranspiler
    {
        public override Type StatementType => typeof(ClassDeclarationStatement);

        public override bool CanInline(Context context, Scope scope, IStatement statement) => false;

        public override void WriteInline(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            TextWriter nonInlinePartWriter, IStatement statement)
        {
            throw new NotSupportedException();
        }

        public override void WriteBlock(Context context, Scope scope, TextWriter writer, TextWriter metaWriter,
            IStatement statement)
        {
            if (!(statement is ClassDeclarationStatement classDecl)) throw new InvalidOperationException();

            if (scope.IsIdentifierExists(classDecl.Name))
            {
                throw new IdentifierNameExistsCompilerException(classDecl.Name, classDecl.Info);
            }

            var fields = new Dictionary<string, TypeDescriptor>();
            foreach (var field in classDecl.Fields)
            {
                if (fields.ContainsKey(field.Name))
                {
                    throw new CompilerException($"Duplicate field '{field.Name}' in class '{classDecl.Name}'.",
                        field.Info);
                }

                fields.Add(field.Name, field.TypeDescriptor);
            }

            FunctionStatement constructor = null;
            foreach (var method in classDecl.Methods)
            {
                if (method.IsConstructor)
                {
                    if (constructor != null)
                    {
                        throw new CompilerException($"Multiple constructors in class '{classDecl.Name}'.", method.Info);
                    }

                    constructor = method;
                }
            }

            var classInfo = new ClassInfo(classDecl.Name, fields, constructor);
            scope.RegisterUserClass(classInfo);

            foreach (var method in classDecl.Methods)
            {
                var methodTranspiler = context.GetTranspilerForStatement(method);
                var validateScope = CreateMethodValidationScope(scope, method);
                if (!methodTranspiler.Validate(context, validateScope, method, out var message))
                {
                    throw new CompilerException($"{message} {method.Info}", method.Info);
                }

                methodTranspiler.WriteBlock(context, scope, writer, metaWriter, method);
            }

            scope.IncrementStatements();
        }

        private static Scope CreateMethodValidationScope(Scope scope, FunctionStatement method)
        {
            var validateScope = scope.BeginNewScope(ScopeType.MethodRoot);

            if (!string.IsNullOrEmpty(method.ClassName) && method.IsInstanceMethod)
            {
                validateScope.SetConfig(c => c.InstanceUsesNameref, "true");
                validateScope.ReserveNewParameter(
                    ObjectModelHelpers.UserClass(method.ClassName),
                    ObjectModelHelpers.ThisKeyword,
                    PowerShellObjectModel.SelfNameref);
            }

            if (method.Parameters != null)
            {
                foreach (var parameter in method.Parameters)
                {
                    validateScope.ReserveNewVariable(parameter.TypeDescriptor, parameter.Name);
                }
            }

            return validateScope;
        }
    }
}
