---
title: Home
nav_order: 1
---

# ShellScript

ShellScript is a cross-platform language that transpiles to native shell scripts (Unix Bash, Windows PowerShell, Windows Batch) with a shared class library.

## Documentation

| Section | Description |
|---------|-------------|
| [Language](language/) | Syntax, types, control flow, and conformance |
| [API Reference](api/) | Class library (`Array`, `File`, `Console`, `Net`, …) |
| [Design](design/high-level-design) | Architecture and compiler overview |
| [Contributing](contributing) | How to build and contribute |

## Quick links

- Repository [README](https://github.com/amkherad/ShellScript/blob/master/README.md)
- Example scripts: `Examples/Api/`, `Examples/Core/`, `Examples/Apps/`
- Regenerate API pages: `python3 scripts/generate-api-docs.py`

## Local preview (Jekyll)

```bash
cd docs
bundle install
bundle exec jekyll serve --livereload
```

Open `http://127.0.0.1:4000`.
