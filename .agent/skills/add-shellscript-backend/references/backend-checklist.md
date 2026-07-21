# Backend Checklist

## Platform wiring

- Implement `IPlatform`.
- Register the platform in application startup.
- Supply compiler constants.
- Supply `MetaInfoWriter`.
- Supply `Transpilers`.
- Supply `Api`.
- Return defaults for every supported type.

## Required statement review

- Constant and variable values
- Definitions and assignments
- Blocks and scope
- Functions, parameters, and calls
- Delegates
- Arithmetic/logical/bitwise expressions
- Casts
- Arrays and indexers
- `echo` and `return`
- `if`/`else`
- Loops
- Includes

## Output safety

- Quote substitutions.
- Avoid word splitting and glob expansion.
- Preserve empty strings.
- Preserve newlines when language semantics require them.
- Distinguish command status from boolean values.
- Avoid leaking helper output into function return channels.
- Use generated identifiers reserved by `Scope`.
- Emit metadata before executable code.

## Test matrix

- Literals for every type
- Default initialization
- Nested expressions and precedence
- Function calls inside expressions
- Nested scopes and shadowing
- API calls
- Target-specific compiler constants
- Expected compile-time rejection
- Generated script syntax validation
- Runtime stdout, stderr, and exit status
