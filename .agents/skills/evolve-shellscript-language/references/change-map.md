# Language Change Map

| Concern | Primary locations |
|---|---|
| Tokens and lexical rules | `ShellScript/Core/Language/Compiler/Lexing/TokenType.cs`, `Lexer.cs` |
| Parser dispatch | `ShellScript/Core/Language/Compiler/Parsing/Parser.cs` |
| Grammar readers | `ShellScript/Core/Language/Compiler/Parsing/Parser.Readers.cs` |
| AST | `ShellScript/Core/Language/Compiler/Statements/` |
| Operators | `ShellScript/Core/Language/Compiler/Statements/Operators/` |
| Type inference/assignability | `StatementHelpers.cs`, `TypeDescriptor.cs`, `DataTypes.cs` |
| Symbols/scopes | `Transpiling/Scope.cs`, `FunctionInfo.cs`, `VariableInfo.cs` |
| Transpiler routing | `Transpiling/Context.cs`, `IPlatform*.cs` |
| Bash emission | `ShellScript/Unix/Bash/PlatformTranspiler/` |
| Bash expressions | `ShellScript/Unix/Bash/PlatformTranspiler/ExpressionBuilders/` |
| Language API | `ShellScript/Core/Language/Library/` |
| Bash API | `ShellScript/Unix/Bash/Api/ClassLibrary/` |
| Compiler pipeline | `Compiler.cs`, `CompilerFlags.cs` |
| Tests | `ShellScript.MSTest/` |
| Specification | `docs/LanguageSpecification.md` |
| Architecture | `docs/HighLevelDesign.md` |

## Completion checklist

- The lexer emits every required token and rejects illegal characters.
- The parser accepts valid syntax and rejects malformed delimiters.
- AST traversal reaches every semantic child.
- Type rules reject invalid operands and assignments.
- Scope rules prevent collisions and resolve parents correctly.
- Every advertised platform has a defined capability.
- Bash output preserves quoting, exit status, and function-result semantics.
- Golden output and negative tests cover the feature.
- Documentation matches executable behavior.
