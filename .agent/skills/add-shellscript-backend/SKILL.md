---
name: add-shellscript-backend
description: Add or extend a ShellScript target platform, statement transpiler, expression builder, metadata writer, class-library implementation, compiler constants, or platform capability coverage. Use when creating a new shell backend or making an existing backend support additional AST statements and API members.
---

# Add ShellScript Backend

## Establish the semantic target

1. Read `docs/LanguageSpecification.md`.
2. Read `docs/HighLevelDesign.md`.
3. Read `references/backend-checklist.md`.
4. Inventory all concrete AST classes and generic fallbacks.
5. Define which language features the target supports natively, emulates, or rejects.

Do not copy target syntax into generic compiler code.

## Implement the platform shell

Provide:

- `IPlatform` implementation and stable platform name
- Compiler constants for preprocessor checks
- Default values for all supported `DataTypes`
- Metadata/prologue writer
- Statement transpiler registration
- API implementation
- Compiler flag revisions where target constraints require them
- Application registration

Fail during compilation when a parsed construct lacks a reliable target representation.

## Cover statements deliberately

For every AST statement:

1. Implement a concrete transpiler.
2. Route it through an intentional generic fallback.
3. Or reject it with a targeted diagnostic.

Never rely on `Context.GetTranspilerForStatement` throwing `InvalidOperationException` as the capability model.

## Preserve source semantics

Pay special attention to:

- Quoting and escaping
- Numeric versus string comparison
- Boolean representation and command exit status
- Function arguments and local variables
- Function result transport
- User-visible `echo` inside value-returning functions
- Arrays and index boundaries
- Short-circuit behavior
- External command failure propagation
- Temporary/helper variable collisions

Use target-native constructs when they preserve behavior. Otherwise emit a documented helper or reject the feature.

## Implement expressions

- Separate conditional expressions from value expressions when the shell requires it.
- Pin intermediate results through `Scope.NewHelperVariable`.
- Keep precedence explicit in generated syntax.
- Avoid `eval` unless no safer representation exists.
- Treat external utilities as declared dependencies with deterministic fallbacks.

## Implement the API

For every platform-independent API member:

1. Match its signature and return type.
2. Choose inline syntax, raw syntax, native resource, or compiled ShellScript resource.
3. Emit support code lazily.
4. Declare utility requirements.
5. Test success, failure, edge values, and quoting.

## Validate

- Compile one program per supported statement family.
- Compare generated scripts against golden files.
- Execute scripts in the target environment when available.
- Verify exit codes and stdout/stderr separately.
- Add a platform capability table to the specification.
- Ensure unsupported features produce compiler diagnostics.
