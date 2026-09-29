# Documentation site

This folder is a [Jekyll](https://jekyllrb.com/) site using the [Just the Docs](https://just-the-docs.github.io/just-the-docs/) theme.

## Build locally

```bash
cd docs
bundle install
bundle exec jekyll serve
```

## API reference generation

API pages under `api/reference/` are generated from C# definitions:

```bash
python3 scripts/generate-api-docs.py
```

Run this after changing files under `ShellScript/Core/Language/Library/`. The Cursor skill `.agents/skills/update-api-documentation/` describes the full workflow for agents.

## Legacy markdown

`ClassLibrary.md` at the docs root is a hand-maintained snapshot; prefer the generated API reference and re-run the generator when the library changes.
