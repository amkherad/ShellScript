# API documentation reference

## Source of truth

- **Definitions:** `ShellScript/Core/Language/Library/**/*.cs` — `public abstract class * : ApiBaseFunction` inside `partial class Api*`.
- **Bash implementations:** `ShellScript/Unix/Bash/Api/ClassLibrary/**`
- **Windows stubs:** `ShellScript/Windows/**/Api/ClassLibrary/**`

## Generator

- Script: `scripts/generate-api-docs.py`
- Output: `docs/api/reference/<class>.md` with Jekyll front matter (`parent: API Reference`, `generated: true`)
- Groups: Core, IO, Network, System, Data, Text & time, Diagnostics & CLI (see script `GROUP_ORDER`)

## Partial classes

Files like `ApiConsole.Terminal.cs` and `ApiNet.Sockets.cs` merge into the same API class via `partial class ApiConsole` / `ApiNet` and `ClassAccessName`.

## Legacy

- `docs/ClassLibrary.md` — historical hand-written API; do not expand; prefer generated pages.
