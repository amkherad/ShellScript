---
title: High-level design
parent: Design
nav_order: 1
---

# ShellScript High-Level Design

## 1. Purpose

ShellScript is a statically typed, C-like source language designed for portable shell automation. The compiler accepts `.shellscript` source, validates it against a platform-independent type and statement model, and emits a target shell program.

The current repository implements a Unix Bash backend. The architecture anticipates additional platforms through interfaces, but no Windows Batch backend is present in this revision.

## 2. Design Goals

- Present familiar C-style syntax for shell-oriented programs.
- Detect invalid identifiers, types, calls, and operators before execution.
- Hide shell-specific result passing, quoting, helper variables, and utility selection.
- Prefer native target-shell constructs over generic runtime helpers.
- Emit API support code only when an API member is used.
- Keep the frontend independent of target-platform syntax.
- Permit compile-time platform selection with preprocessor constants.

## 3. Non-Goals and Current Constraints

- ShellScript is a transpiler, not a virtual machine or bytecode runtime.
- Runtime behavior ultimately inherits target-shell and external-utility constraints.
- Classes, object construction, `switch`, `for`, exceptions, async operations, and string interpolation are not implemented by the current parser/backend.
- `foreach`, `while`, and `do while` parse into AST nodes, but the Bash platform does not register transpilers for them.
- Newlines are not statement terminators in the current lexer; semicolons are required by default.

## 4. System Context

```text
.shellscript source
        |
        v
      Lexer
        |
        v
Preprocessor token proxy
        |
        v
      Parser
        |
        v
Typed statement/expression tree
        |
        v
Platform validation + transpilers
        |
        +------> metadata/prologue stream
        |
        +------> generated code stream
                       |
                       v
                 final shell script
```

The compiler writes metadata and executable code to temporary object files, then concatenates them into the final output. This lets lazily requested API functions, helpers, and prologue content appear before executable statements.

## 5. Major Components

### 5.1 Command-Line Layer

`Program` initializes available platforms and dispatches commands. `CompileCommand` validates arguments, creates default compiler flags, applies supported switches, and invokes `Compiler`.

Primary inputs:

- Source path
- Output path
- Platform name
- Compiler switches

Primary output:

- Generated shell script or a captured compilation exception

### 5.2 Compiler Orchestrator

`Compiler` owns the end-to-end pipeline:

1. Resolve the target platform.
2. Let the platform revise compiler flags.
3. Create metadata and source object streams.
4. Initialize the compilation context and API.
5. Parse source statements incrementally.
6. Select a statement transpiler.
7. Validate and emit each statement.
8. Combine metadata and executable code.
9. Run optional **generated-code post-processors** (formatting, etc.) registered on the target platform.

Post-processing is controlled by `CompilerFlags` (for example `--format-generated-code` runs the built-in Bash indent formatter on Unix-Bash). Implement `IGeneratedCodePostProcessor` and return it from `IPlatform.GeneratedCodePostProcessors` to add platform-specific steps.

Compilation is streaming at the top level: parsed statements are transpiled one at a time rather than collected into a complete compilation unit.

### 5.3 Lexer

`Lexer` converts text into positioned tokens.

Responsibilities:

- Recognize keywords, punctuation, operators, identifiers, numeric literals, and quoted strings.
- Remove single-line and multi-line comments.
- Ignore whitespace outside strings.
- Join physical lines ending in `\`.
- Attach line and column positions for diagnostics.

Important characteristics:

- Token matching is regex-based and ordered.
- Unknown characters are currently skipped rather than emitted as invalid tokens.
- The lexer contains token kinds for planned features that are disabled or not consumed by the parser.

### 5.4 Preprocessor

`PreProcessorParser` wraps the token enumerator and conditionally exposes tokens to the parser.

Supported directives:

- `#if (expression)`
- `#elseif (expression)`
- `#else`
- `#endif`

Conditions are parsed as ordinary expressions and reduced through the evaluation transpiler. A constant boolean selects the active branch. Missing identifiers evaluate as false, enabling platform checks such as `#if (Bash)`.

### 5.5 Parser and AST

`Parser` is a hand-written recursive reader over a peeking token enumerator.

It produces statement objects for:

- Blocks
- Variable and constant definitions
- Functions and parameters
- Delegates
- Assignments and calls
- `if`/`else`
- Loop forms
- `echo`, `return`, and `include`
- Arrays and indexers
- Arithmetic, logical, bitwise, cast, and assignment expressions

Expressions are first collected as a linked list of operands and operators. The parser repeatedly selects the highest-order operator and replaces it with an AST node. Operator classes therefore define precedence directly.

### 5.6 Type System and Semantic Model

`TypeDescriptor` represents built-in, array, delegate/class, and lookup types. `Scope` tracks:

- Reserved identifiers
- Variables and parameters
- Constants
- Functions and prototypes
- Scoped compiler configuration
- Generated helper variables

Semantic validation is distributed between statement helpers and platform transpilers. It includes identifier lookup, assignability, function parameter matching, operator compatibility, and void-value restrictions.

### 5.7 Platform Abstraction

`IPlatform` supplies:

- Platform name
- Compiler constants
- API implementation
- Statement transpilers
- Metadata writer
- Default values
- Flag revisions

`Context` maps concrete AST node types to target transpilers. Expression subclasses can fall back to a generic expression transpiler.

To add a platform:

1. Implement `IPlatform`.
2. Implement metadata generation.
3. Register transpilers for every supported AST statement.
4. Implement or adapt the class-library API.
5. Define compiler constants and default values.
6. Register the platform during application startup.

### 5.8 Bash Backend

The Bash backend implements:

- Variable definitions and assignments
- Generic expression emission
- Function definitions and calls
- Delegates
- `if`/`else`
- `echo`
- `return`
- Includes
- Metadata/prologue generation

Expression builders choose conditional or default Bash expression strategies. They may pin intermediate results into generated helper variables when a shell expression cannot safely remain inline.

### 5.9 Class Library

The API layer exposes typed language-level classes such as:

- `Array`
- `Convert`
- `Locale`
- `Math`
- `Platform`
- `String`
- `User`
- `File`
- `Net`

An API method can produce:

- Inline target syntax
- Raw emitted target code
- A native resource
- A ShellScript resource compiled into the output

The platform API initializes symbols in the compilation context and emits implementations lazily.

## 6. Data and Control Flow

### 6.1 Parsing Flow

1. Read source line.
2. Remove comments and whitespace.
3. Generate positioned tokens.
4. Filter tokens through active preprocessor states.
5. Dispatch from the first token to a statement reader.
6. Build nested statement and expression nodes.

### 6.2 Transpilation Flow

1. Resolve the transpiler from the statement runtime type.
2. Validate the statement in the current scope.
3. Reserve or resolve language symbols.
4. Reduce constants and API calls when possible.
5. Generate helper variables or helper functions when required.
6. Write declarations/helpers to metadata.
7. Write executable syntax to code output.

### 6.3 Scope Flow

- The context owns one general scope.
- Block and function transpilers create child scopes.
- Symbol lookup walks from the current scope to parents.
- Generated helper identifiers are reserved in the owning scope.
- Platform/API compiler constants are reserved before parsing source.

## 7. Extension Strategy

### 7.1 Adding Syntax

Change all affected layers:

1. Add or enable token recognition.
2. Add parser production and AST representation.
3. Define type and control-flow rules.
4. Add transpiler contracts or fallback behavior.
5. Implement each supported backend.
6. Add lexer, parser, semantic, and golden-output tests.
7. Update `docs/LanguageSpecification.md`.

### 7.2 Adding an Operator

1. Add a token and lexer pattern.
2. Add an `IOperator` implementation with precedence and associativity.
3. Extend expression reduction.
4. Extend type inference and assignability.
5. Extend backend expression builders.
6. Test precedence, invalid operands, constants, and generated output.

### 7.3 Adding an API Member

1. Define the platform-independent API contract.
2. Register its signature and return type.
3. Implement target-specific generation.
4. Declare third-party utility requirements where needed.
5. Add language-level and generated-output tests.
6. Update `docs/ClassLibrary.md`.

## 8. Quality Attributes

### Portability

Frontend constructs should not encode Bash syntax. Platform-specific behavior belongs behind `IPlatform`, transpilers, expression builders, and API implementations.

### Correctness

Static validation should reject programs that cannot be represented reliably on a target. If semantics differ by platform, document the difference and require an explicit platform API escape hatch rather than silently changing behavior.

### Readability

Generated scripts use optional comments, metadata segments, stable helper names, and target-native constructs.

### Efficiency

The compiler performs constant reduction, optional inlining, lazy API emission, and native-operation selection. External utilities are used only when the target shell lacks a reliable primitive.

### Diagnosability

Tokens and statements retain source location information. New features should preserve the original file, line, and column in lexer, parser, and compiler exceptions.

## 9. Known Architectural Risks

- Parser and backend capability are not represented by one explicit feature matrix.
- Some lexer tokens and AST classes exist without complete parser/backend support.
- Expression reduction reads associativity metadata but selects operators primarily by precedence.
- Unknown source characters can be silently discarded by the lexer.
- Top-level streaming complicates forward references and whole-program analysis.
- API and Bash-specific references leak into otherwise generic context code.
- Tests cover compilation snapshots more than parser and semantic edge cases.

## 10. Recommended Evolution

1. Introduce a checked feature matrix per platform.
2. Make the lexer fail on unknown characters.
3. Replace linked-list expression reduction with a Pratt or precedence-climbing parser.
4. Separate parse, bind/type-check, optimize, and emit phases.
5. Collect a compilation unit before emission to support prototypes and whole-program diagnostics.
6. Add backend conformance tests generated from the language specification.
7. Implement loop transpilers before advertising loop support.
8. Remove or gate unfinished syntax until each layer supports it.
