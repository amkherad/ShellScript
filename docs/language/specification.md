---
title: Language specification
parent: Language
nav_order: 1
---

# ShellScript Language Specification

## 1. Status and Conformance

This document describes the language implemented by the current repository. It distinguishes:

- **Implemented**: accepted by the frontend and supported by the Bash backend.
- **Frontend only**: parsed into an AST but not emitted by the Bash backend.
- **Reserved**: represented by tokens, documentation, or AST types but not usable end to end.

A conforming implementation must reject unsupported constructs rather than silently reinterpret them.

## 2. Source Files

- Conventional extension: `.shellscript`
- Text encoding: determined by the host .NET text reader
- Keywords: case-sensitive
- Whitespace: ignored outside quoted strings
- Statement terminator: `;` by default
- Physical line continuation: a trailing `\` joins the next line

Newlines do not currently produce statement terminator tokens.

## 3. Lexical Grammar

The grammar uses EBNF-like notation:

- `{ X }` means zero or more repetitions.
- `[ X ]` means optional.
- `A | B` means a choice.
- Quoted text is literal syntax.

### 3.1 Comments

```ebnf
single-line-comment = "//", { any-character-except-newline } ;
multi-line-comment  = "/*", { any-character }, "*/" ;
```

Comments do not reach the parser.

### 3.2 Identifiers

```ebnf
identifier = word-character, { word-character } ;
```

The implementation uses the regular-expression notion of `\w`. Identifiers must not collide with reserved language names in contexts where the parser checks keyword validity.

### 3.3 Numeric Literals

```ebnf
number-literal =
    [ "+" | "-" ],
    { decimal-digit },
    [ ".", decimal-digit, { decimal-digit } ],
    [ ("e" | "E"), [ "+" | "-" ], decimal-digit, { decimal-digit } ] ;
```

Whole values parse as `int`; other accepted numeric values parse as `float`.

### 3.4 String Literals

```ebnf
string-literal =
      '"', { escaped-quote | non-double-quote-character }, '"'
    | "'", { escaped-quote | non-single-quote-character }, "'" ;
```

Both quote styles produce `string`. There is no character type.

### 3.5 Boolean and Null Literals

```ebnf
boolean-literal = "true" | "false" ;
null-literal    = "null" | "nil" ;
```

The current parser represents `null`/`nil` as a string-typed constant. Programs should not rely on general nullable-value semantics.

## 4. Keywords

### 4.1 Implemented or Parsed

`const`, `void`, `int`, `long`, `bool`, `double`, `float`, `number`, `string`, `object`, `any`, `delegate`, `if`, `else`, `for`, `foreach`, `while`, `do`, `loop`, `return`, `new`, `include`, `echo`, `true`, `false`, `null`, `nil`

### 4.2 Preprocessor

`#if`, `#elseif`, `#else`, `#endif`

### 4.3 Reserved but Incomplete

`switch`, `case`, `default`, `class`, `throw`, `async`, `await`, `in`, `notin`, `like`, `notlike`, `call`

Some names appear in the parser keyword set even when their lexer token is disabled.

## 5. Type System

### 5.1 Built-in Types

| Source spelling | Semantic type | Status |
|---|---|---|
| `int`, `long` | Integer | Implemented |
| `float`, `double` | Float | Implemented |
| `number` | Numeric union of integer/float | Implemented |
| `bool` | Boolean | Implemented |
| `string` | String | Implemented |
| `void` | No value; function return only | Implemented |
| `any` | Dynamically accepted API value | Implemented |
| `object` | Class/object slot | Reserved/incomplete |
| `delegate` | Callable reference | Implemented through delegate declarations |

The README mentions `boolean`, but the lexer recognizes `bool` only.

### 5.2 Arrays

Append `[]` to supported element types:

```ebnf
array-type = scalar-type, "[", "]" ;
```

Recognized array forms include `int[]`, `long[]`, `float[]`, `double[]`, `number[]`, `string[]`, `object[]`, and `any[]`.

Arrays are represented by a type flag plus their element type.

### 5.3 Named Lookup Types

An identifier in type position becomes a lookup type. It can refer to a declared delegate or API-defined type. Full user-defined class semantics are not implemented.

### 5.4 Assignability

- Exact type matches are assignable.
- Integer values can participate in broader numeric contexts.
- `number` represents either integer or floating-point values.
- Explicit casts use `(type) expression`.
- Backend validation may reject otherwise parsed operations that are not representable.

## 6. Program Grammar

```ebnf
program = { statement } ;

statement =
      block
    | variable-declaration
    | function-declaration
    | delegate-declaration
    | assignment-or-call-statement
    | if-statement
    | while-statement
    | do-while-statement
    | foreach-statement
    | echo-statement
    | return-statement
    | include-statement
    | empty-statement ;

block = "{", { statement }, "}" ;
empty-statement = ";" ;
```

`for` and `loop` are lexed and dispatched, but are not currently usable: `for` throws `NotImplementedException`, and `loop` delegates to a reader that expects `while`.

## 7. Declarations

### 7.1 Variables

```ebnf
variable-declaration =
    type, identifier, [ "=", expression ], ";" ;
```

Examples:

```csharp
int count;
double price = 3.50;
string title = "Total";
```

An omitted initializer uses the target platform's default value.

### 7.2 Constants

```ebnf
constant-declaration =
    "const", type, identifier, "=", expression, ";" ;
```

Constants require an initializer.

```csharp
const string ErrorMessage = "Invalid value";
```

### 7.3 Functions

```ebnf
function-declaration =
    type, identifier, "(", [ parameter-list ], ")", block ;

parameter-list =
    parameter, { ",", parameter } ;

parameter =
    type, identifier, [ "=", constant-value ] ;
```

Examples:

```csharp
int add(int left, int right) {
    return left + right;
}

void print(string text = "default") {
    echo text;
}
```

Default parameter values must be constant literals accepted by the declared parameter type.

### 7.4 Delegates

```ebnf
delegate-declaration =
    "delegate", type, identifier, "(", [ parameter-list ], ")", ";" ;
```

Example:

```csharp
delegate string PathProvider();
PathProvider provider;
provider = getPath;
echo provider();
```

Delegate calls use ordinary function-call syntax.

## 8. Statements

### 8.1 Assignment and Calls

```ebnf
assignment-or-call-statement =
      assignment, ";"
    | function-call, ";" ;

assignment = assignable-expression, "=", expression ;
```

Assignment is also an expression.

### 8.2 Echo

```ebnf
echo-statement = "echo", expression, ";" ;
```

Parentheses are optional because a parenthesized expression is itself valid:

```csharp
echo "hello";
echo (value + 1);
```

Inside non-void Bash functions, the backend may redirect `echo` to an explicit device because standard output carries function results.

### 8.3 Return

```ebnf
return-statement = "return", [ expression ], ";" ;
```

- Void functions use `return;`.
- Value-returning functions use `return expression;`.
- Backend validation enforces return-type compatibility.

### 8.4 Include

```ebnf
include-statement = "include", expression, ";" ;
```

The expression must be a **string literal** path to another `.shellscript` file. Includes are only valid at the **root scope** of a file (not inside functions or blocks).

Resolution searches, in order: directories of files currently being compiled (innermost first), then directories registered on the compilation context (typically the entry file’s folder), then the process working directory.

Each included file is merged **at most once** per compilation (repeat `include` of the same resolved path is a no-op). **Circular includes** are rejected.

Helper fragments often live under a `parts/` subfolder next to the entry script; those files are not meant to be compiled alone.

### 8.5 If/Else

```ebnf
if-statement =
    "if", "(", expression, ")", block,
    { "else", "if", "(", expression, ")", block },
    [ "else", block ] ;
```

Only braced bodies are accepted.

```csharp
if (value > 0) {
    echo "positive";
} else {
    echo "not positive";
}
```

Status: implemented by the frontend and Bash backend.

### 8.6 While

```ebnf
while-statement =
    "while", "(", expression, ")", (block | ";") ;
```

Status: frontend only. The Bash platform does not register a `WhileStatement` transpiler.

### 8.7 Do/While

```ebnf
do-while-statement =
    "do", block, "while", "(", expression, ")", ";" ;
```

Status: frontend only. The Bash platform does not register a `DoWhileStatement` transpiler.

### 8.8 Foreach

```ebnf
foreach-statement =
    "foreach", "(",
    ( type, identifier | identifier ),
    "in", expression,
    ")", block ;
```

Status: frontend only. The `in` token is currently disabled in the lexer, and the Bash platform has no `ForEachStatement` transpiler; therefore source `foreach` programs do not compile end to end in this revision.

### 8.9 Unsupported Control Flow

- `for`: parser entry exists but always throws `NotImplementedException`.
- `loop`: incorrectly reuses the `while` reader and is not valid end to end.
- `switch`: reader returns no AST.
- `break` and `continue`: no tokens or statements exist.
- `throw`: token exists, statement dispatch is disabled.

## 9. Expressions

### 9.1 Primary Expressions

```ebnf
primary-expression =
      literal
    | variable-access
    | function-call
    | array-creation
    | "(", expression, ")" ;

variable-access =
    identifier, [ ".", identifier ] ;

function-call =
    [ identifier, "." ], identifier,
    "(", [ argument-list ], ")" ;

argument-list =
    expression, { ",", expression } ;
```

Only one class/member dot is parsed.

### 9.2 Arrays

Supported forms:

```ebnf
array-creation =
      "new", scalar-type, "[", expression, "]"
    | "new", array-type, "{", [ argument-list ], "}"
    | "new", scalar-type, "[", "]", "{", [ argument-list ], "}" ;

index-expression =
    expression, "[", expression, "]" ;
```

Example:

```csharp
int[] values = new int[] { 1, 2, 3 };
echo values[0];
```

Array support may require generated shell helpers.

### 9.3 Casts

```ebnf
cast-expression = "(", type, ")", expression ;
```

Nested casts are represented as nested `TypeCastStatement` nodes.

### 9.4 Operators

Implemented parser precedence, highest first:

| Order | Operators | Associativity metadata |
|---:|---|---|
| 100 | `[]` | Left-to-right |
| 80 | `++`, `--` | Contextual prefix/postfix |
| 70 | unary `-` | Right-to-left |
| 65 | `!`, `~`, cast | Right-to-left for `!`/`~`; cast metadata is left-to-right |
| 55 | `*`, `/`, `%`, `\` | Left-to-right |
| 50 | `+`, `-` | Left-to-right |
| 45 | `<`, `<=`, `>`, `>=` | Left-to-right |
| 44 | `==`, `!=` | Left-to-right |
| 39 | `&` | Left-to-right |
| 38 | `^` | Left-to-right, but `^` is not tokenized |
| 37 | `|` | Left-to-right |
| 36 | `&&` | Left-to-right |
| 35 | `||` | Left-to-right |
| 20 | `=` | Right-to-left |

Notes:

- `%` has a token kind but no lexer regex; `\` is accepted as modulus and mapped to the same operator.
- The reduction algorithm chooses the highest precedence operator from a linked list. Equal-precedence associativity should be treated cautiously until covered by tests.
- Compound assignments, shifts, conditional `?:`, and null coalescing are not implemented end to end.

### 9.5 Operator Semantics

- Numeric `+`, `-`, `*`, `/`, and modulus operate on numeric-compatible operands.
- `+` also concatenates strings.
- String repetition by integer multiplication is intended by backend expression logic.
- Relational and equality operators produce `bool`.
- `&&`, `||`, and `!` operate on boolean-compatible expressions.
- `&`, `|`, and `~` are bitwise operators.
- Prefix/postfix `++` and `--` require variable access operands.
- Assignment requires an assignable left side and compatible right side.

## 10. Preprocessor

```ebnf
preprocessor-group =
    "#if", "(", expression, ")", tokens,
    { "#elseif", "(", expression, ")", tokens },
    [ "#else", tokens ],
    "#endif" ;
```

Rules:

- Parentheses around conditions are mandatory.
- Conditions must reduce to a constant boolean.
- An unknown identifier in a preprocessor condition is treated as false.
- Directives may be nested.
- The active platform supplies compiler constants; Bash supplies `Unix = true` and `Bash = true`.

Example:

```csharp
#if (Bash)
echo "bash";
#else
echo "other";
#endif
```

## 11. Scopes and Names

- Variables and parameters are visible in their scope and descendant scopes.
- Symbol lookup walks outward through parent scopes.
- Variables, constants, functions, and prototypes share identifier reservation mechanisms.
- API members may use qualified names such as `Math.Abs`.
- Generated helper variables use an `h_` prefix and are reserved to avoid collisions.

## 12. Functions and Shell Result Semantics

Shell targets do not have a uniform typed return mechanism. The Bash backend may:

- Emit function results on standard output.
- Reserve an explicit echo stream for user-visible output.
- Store call results in helper variables.
- Inline simple functions or API calls.

Source-level programs must use `return` and `echo` according to language semantics rather than depending on generated Bash details.

## 13. Standard API

The repository defines language-level APIs in these namespaces:

- Core: `Array`, `Convert`, `Locale`, `Math`, `Platform`, `String`, `User`
- IO: `File`
- Network: `Net`

API signatures and completeness are documented in `docs/ClassLibrary.md`. API implementations can be inline, native target snippets, emitted helper functions, or embedded ShellScript resources.

## 14. Diagnostics

Source tokens carry:

- File path through parser context
- Line number
- Start/end columns

Failures are categorized as:

- Lexical/illegal syntax
- Parser syntax
- Preprocessor errors
- Identifier errors
- Type mismatch
- Invalid operator/type combinations
- Invalid function parameters
- Unsupported statement/backend mapping

## 15. Implementation Feature Matrix

| Feature | Lexer | Parser/AST | Bash emission |
|---|---:|---:|---:|
| Variables/constants | Yes | Yes | Yes |
| Functions/calls | Yes | Yes | Yes |
| Delegates | Yes | Yes | Yes |
| Arrays/indexing | Partial | Yes | Via expression/API helpers |
| `if`/`else` | Yes | Yes | Yes |
| `while` | Yes | Yes | No |
| `do while` | Yes | Yes | No |
| `foreach` | No `in` token | Yes | No |
| `for` | Yes | Stub | No |
| `switch` | Yes | Stub | Transpiler exists but unreachable |
| `echo`/`return` | Yes | Yes | Yes |
| `include` | Yes | Yes | Yes |
| Preprocessor conditionals | Yes | Yes | Compile-time only |
| Classes/objects | Disabled/partial | Stub | No |
| Exceptions | Token only | Disabled | No |
| Async/await | Disabled | No | No |
| String interpolation | No | No | No |

## 16. Conformance Guidance for New Features

A feature is complete only when all of the following exist:

1. Lexical recognition with invalid-input tests.
2. Parser production and AST representation.
3. Type, scope, and control-flow rules.
4. Backend capability declaration.
5. At least one backend implementation.
6. Positive and negative semantic tests.
7. Golden generated-output tests.
8. Documentation and feature-matrix updates.
