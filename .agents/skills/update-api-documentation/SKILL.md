---
name: update-api-documentation
description: >-
  Keeps ShellScript language and API documentation in sync with the compiler
  library. Use when adding, renaming, or changing APIs under
  ShellScript/Core/Language/Library, Console/Net/Process APIs, or when the user
  asks to update docs, API reference, or Jekyll documentation.
---

# Update API documentation

## When to apply

- Any edit to `ShellScript/Core/Language/Library/**` (especially `Api*.cs`).
- New platform implementations that expose public library behavior.
- User requests documentation, API reference, or Jekyll site updates.

## Workflow

1. **Regenerate API reference** (required for library changes):

```bash
python3 scripts/generate-api-docs.py
```

This writes `docs/api/index.md` and `docs/api/reference/*.md` from `Summary`, `Name`, parameters, and return types in C#.

2. **Manual language docs** — if syntax or semantics changed, update:
   - `docs/language/specification.md` (and keep `docs/LanguageSpecification.md` aligned if that file is still the editor’s source).

3. **Examples** — add or adjust `Examples/Api/<Name>/` when introducing user-facing APIs.

4. **Verify** (when Jekyll/Ruby is available):

```bash
cd docs && bundle exec jekyll build
```

5. **Do not** hand-edit generated files under `docs/api/reference/` except by re-running the script.

## Jekyll layout

| Path | Purpose |
|------|---------|
| `docs/_config.yml` | Just the Docs theme |
| `docs/index.md` | Site home |
| `docs/language/` | Language specification |
| `docs/api/` | API index + generated reference |
| `docs/design/` | Architecture |
| `docs/contributing.md` | Contributor guide |

## Checklist before finishing a library PR

- [ ] `python3 scripts/generate-api-docs.py` run and outputs committed
- [ ] New class appears on `docs/api/index.md`
- [ ] Example or app under `Examples/` if the API is non-obvious
- [ ] Snapshot exclusions updated only if the example is interactive (`SnapshotOutputTests.cs`)

## Reference

See [REFERENCE.md](REFERENCE.md) for file locations and generator behavior.
