---
title: Contributing
nav_order: 5
---

# Contributing

Thank you for contributing to ShellScript.

## Development setup

1. Install the [.NET SDK](https://dotnet.microsoft.com/download) (8.x matches the current solution).
2. Clone the repository and build:

```bash
dotnet build ShellScript.sln
dotnet test ShellScript.MSTest/ShellScript.MSTest.csproj
```

3. Optional: use the Dev Container described in the root `README.md`.

## Documentation

- **Language** — edit `docs/language/specification.md` (or the source `docs/LanguageSpecification.md` and sync).
- **API** — change `ShellScript/Core/Language/Library/**/*.cs`, then run:

```bash
python3 scripts/generate-api-docs.py
```

- Preview the docs site:

```bash
cd docs && bundle install && bundle exec jekyll serve
```

Agents should follow `.agents/skills/update-api-documentation/SKILL.md` when touching the class library.

## Stray `ESC[0m` file in the repo root

If a zero-byte or tiny file appears whose name is an ANSI reset sequence (`\033[0m`), it comes from **bash interpreting unquoted `${CReset}` / color variables as a redirect**. Recompile with a current compiler (echo arguments are always quoted) and remove the file:

```bash
rm -f $'\033[0m'
```

## Pull requests

- Keep changes focused; match existing code style.
- Run tests before opening a PR.
- Update generated API markdown when public API definitions change.

## Code of conduct

Be respectful and constructive in issues and reviews.
