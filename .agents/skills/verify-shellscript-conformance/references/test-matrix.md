# Conformance Test Matrix

| Area | Positive cases | Negative cases |
|---|---|---|
| Lexing | Each token, whitespace, comments, positions | Unknown characters, unterminated string/comment |
| Types | Every scalar/array, numeric widening, cast | Void value, incompatible assignment, invalid cast |
| Declarations | Defaults, constants, parameters | Duplicate/keyword names, uninitialized constant |
| Functions | Calls, defaults, nesting, return | Arity/type mismatch, missing/invalid return |
| Expressions | Precedence, unary, index, assignment | Missing operand, invalid operator/type |
| Scope | Parent lookup, block locals, helpers | Out-of-scope access, collisions |
| Branches | If/else-if/else, constants | Non-boolean condition, malformed body |
| Loops | Empty/body/condition per platform | Unsupported backend, malformed delimiters |
| Delegates | Declaration, assignment, call | Signature mismatch, unknown target |
| Arrays | Creation, length, index, API | Mixed elements, invalid index/type |
| Preprocessor | Nested branches, platform constants | Nonconstant condition, missing `#endif` |
| API | Inline and emitted methods | Invalid parameters, missing utility behavior |
| Backend | Golden code, metadata ordering | Missing transpiler, unsafe quoting |
| Runtime | stdout/stderr/status | Command failure and edge quoting |

## Minimum feature acceptance

For each new feature, add:

- One lexer test
- One parser/AST test
- One valid semantic test
- One invalid semantic test
- One generated-output test
- One runtime test when execution is deterministic
- One documentation update
