---
name: verify-shellscript-conformance
description: Design, add, or run ShellScript conformance tests for lexical grammar, parser behavior, static typing, scopes, constant folding, platform capability, generated shell code, and runtime results. Use when validating a language feature, documenting implementation gaps, preventing frontend/backend drift, or building regression and golden-output coverage.
---

# Verify ShellScript Conformance

## Build the matrix

1. Read `docs/LanguageSpecification.md`.
2. Read `references/test-matrix.md`.
3. Select the feature rows under test.
4. Mark each case as positive, negative, backend-specific, or runtime.
5. Record the expected compiler phase and diagnostic for failures.

Test implemented behavior, not README claims.

## Test from narrow to broad

### Lexer

- Assert token kinds, values, and positions.
- Cover longest-match conflicts and keyword boundaries.
- Cover comments, strings, exponents, arrays, and illegal characters.

### Parser and AST

- Assert concrete statement type and important fields.
- Assert precedence, grouping, and parent-child relationships.
- Cover missing delimiters and unexpected tokens.

### Semantic validation

- Cover unknown identifiers, duplicate names, invalid assignment, invalid operands, bad parameters, and void misuse.
- Verify scope lookup and helper-name collision behavior.

### Backend generation

- Compile fixtures into deterministic golden files.
- Normalize only nondeterministic metadata.
- Assert quoting, redirects, helper placement, and target-native constructs.
- Add an expected compiler error for unsupported AST statements.

### Runtime

- Execute generated scripts only in a compatible target environment.
- Assert stdout, stderr, and exit code independently.
- Include empty strings, spaces, glob characters, command failures, and nested calls.

## Organize fixtures

- Keep source fixtures under `ShellScript.MSTest/TestScripts/`.
- Use target suffixes such as `.unix-bash` for golden output.
- Use `.stdout`, `.stderr`, or explicit assertions for runtime expectations.
- Give each fixture one primary semantic purpose.

## Diagnose drift

When a test fails, identify the first incorrect layer:

1. Token recognition
2. Parser production
3. AST construction
4. Type/scope validation
5. Transpiler routing
6. Expression generation
7. API implementation
8. Runtime environment

Do not update a golden file until the generated semantic change is understood.

## Finish

- Run focused tests first.
- Run the full MSTest project if the installed .NET SDK supports the legacy target.
- If execution is blocked by toolchain age, report the exact command and failure.
- Update the specification feature matrix when tests reveal a capability gap.
