---
name: evolve-shellscript-language
description: Design, implement, or repair ShellScript language features across lexical syntax, parser productions, AST nodes, type checking, scope rules, platform transpilers, tests, and specifications. Use for new keywords, statements, expressions, operators, types, control flow, diagnostics, or when an advertised feature is incomplete across compiler layers.
---

# Evolve ShellScript Language

## Start with the contract

1. Read `docs/LanguageSpecification.md`.
2. Read `docs/HighLevelDesign.md`.
3. Read `references/change-map.md`.
4. Locate all existing representations of the feature with `rg`.
5. Classify the feature as implemented, frontend-only, reserved, or new.

Do not infer support from the README or from the existence of one token, AST class, or transpiler.

## Define semantics before editing

Write a short implementation contract covering:

- Concrete syntax and precedence
- AST shape
- Static types and conversions
- Scope and mutability rules
- Control-flow behavior
- Constant-folding behavior
- Backend-independent semantics
- Unsupported or target-dependent cases

Prefer rejecting an unrepresentable construct over silently changing its meaning.

## Implement vertically

Change only the layers the feature requires, but verify every layer:

1. **Lexer**: tokens, ordered regexes, boundaries, positions, invalid input.
2. **Parser**: production, delimiters, AST parents, source information.
3. **Semantic model**: lookup, assignability, operator validity, return/control-flow rules.
4. **AST**: traversable children and stable statement information.
5. **Backend contract**: transpiler selection or expression fallback.
6. **Bash backend**: validation, quoting, helper variables, result transport.
7. **API**: platform-independent signature and target implementation when relevant.
8. **Tests**: lexer/parser failures, type failures, generated output, runtime behavior when practical.
9. **Docs**: specification grammar, semantics, and feature matrix.

## Preserve architecture

- Keep target syntax out of lexer, parser, AST, and generic semantic code.
- Keep API discovery separate from API emission.
- Reserve generated identifiers through `Scope`.
- Preserve `StatementInfo` and parent-child relationships.
- Add a concrete transpiler or an intentional generic fallback.
- Avoid enabling dormant syntax until Bash can emit it or the platform explicitly rejects it.

## Expression changes

For an operator or expression form:

1. Add tokenization with longest-match ordering.
2. Define precedence and associativity.
3. Update expression parsing/reduction.
4. Update `StatementHelpers.GetDataType` and assignability paths.
5. Add constant evaluation where deterministic.
6. Update Bash conditional/default expression builders.
7. Test parentheses, equal-precedence chains, unary/binary ambiguity, and invalid operands.

Treat the current linked-list reducer as fragile. Prefer a focused parser refactor if correct associativity cannot be added safely.

## Control-flow changes

Verify:

- Braced versus embedded statement rules
- Child scope creation
- Condition type
- Empty body behavior
- Return-path analysis
- Generated shell status handling
- `echo` behavior inside value-returning functions

## Finish

- Run the narrowest relevant tests, then the full test project if available.
- Compile representative `.shellscript` programs and inspect output.
- Update the feature matrix honestly.
- Report incomplete platforms or deferred semantics explicitly.
