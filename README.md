# ShellScript

| CI | Releases |
|----|----------|
| [![CI](https://github.com/amkherad/ShellScript/actions/workflows/ci.yml/badge.svg?branch=master)](https://github.com/amkherad/ShellScript/actions/workflows/ci.yml) | Push a **tag** to build single-file binaries for Linux, Windows, and macOS ([workflow](https://github.com/amkherad/ShellScript/actions/workflows/release.yml)) |

Cross-platform shell scripting language that **transpiles** (source-to-source compiles) `.shellscript` sources into native shell scripts. One codebase can target **Unix Bash**, **Windows PowerShell**, and **Windows Batch**, with a shared class library (`File`, `Path`, `Console`, `Net`, `Assert`, and more).

The canonical language definition is **[docs/language/specification.md](docs/language/specification.md)**. API reference pages are generated under **[docs/api/](docs/api/)** (see [Contributing](#contributing)).

## Documentation

| Topic | Location |
|--------|----------|
| Language (grammar, types, conformance) | [docs/language/specification.md](docs/language/specification.md) |
| API reference | [docs/api/index.md](docs/api/index.md) |
| Architecture | [docs/design/high-level-design.md](docs/design/high-level-design.md) |
| Contributing & docs site | [docs/contributing.md](docs/contributing.md) |
| Jekyll site (local preview) | [docs/README-docs.md](docs/README-docs.md) |

Example scripts: `Examples/Api/` (class library), `Examples/Core/` (language features), `Examples/Apps/` (larger programs), `Examples/Events/`.

## Installing

### Arch Linux

```bash
sudo pacman -S shellscript
```

The package pulls in the .NET runtime; you can also install the SDK explicitly:

```bash
sudo pacman -S dotnet-host dotnet-runtime dotnet-sdk
```

### Download binaries

[ShellScript releases](https://github.com/amkherad/ShellScript/releases)

### Build from source

Install the [.NET SDK](https://dotnet.microsoft.com/download) **8.x** (matches `ShellScript/ShellScript.csproj`).

```bash
git clone git@github.com:amkherad/ShellScript.git
cd ShellScript
dotnet build ShellScript.sln
dotnet test ShellScript.MSTest/ShellScript.MSTest.csproj
```

The compiler executable is produced under `ShellScript/bin/Debug/net8.0/` (or `Release`).

### Dev Container (Docker)

Build and test without installing the SDK on the host:

1. Install [Docker](https://docs.docker.com/get-docker/) and the [Dev Containers](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers) extension.
2. Open the repo and run **Dev Containers: Reopen in Container**.
3. In the integrated terminal:

```bash
dotnet build ShellScript.sln
dotnet test ShellScript.MSTest/ShellScript.MSTest.csproj
```

The image uses .NET SDK 8 and includes `bash`, `jq`, and `python3` for compiler and snapshot tests.

```bash
docker build -f .devcontainer/Dockerfile -t shellscript-dev .
docker run --rm -it -v "$PWD:/workspace" -w /workspace shellscript-dev \
  bash -lc "dotnet test ShellScript.MSTest/ShellScript.MSTest.csproj"
```

---

## Quick start

Write `hello.shellscript`:

```csharp
echo "Hello from ShellScript";
```

Compile for Bash (output path is optional; default is beside the source with a `.bash` extension):

```bash
dotnet run --project ShellScript/ShellScript.csproj -- \
  compile hello.shellscript Unix-Bash
bash hello.bash
```

Or compile and run in one step (Unix Bash, temporary output):

```bash
dotnet run --project ShellScript/ShellScript.csproj -- run hello.shellscript
```

---

## Language overview

ShellScript uses **C#-like** syntax tuned for shell environments: functions, `if`/`else`, loops, `echo`, `return`, delegates, arrays, and static typing. Keywords are **case-sensitive**. Whitespace is ignored outside quoted strings. Statements end with **`;`** (newlines are not statement terminators).

### Target platforms

| Platform name | Role |
|---------------|------|
| `Unix-Bash` | Primary backend; full statement and API coverage used in CI |
| `Windows-PowerShell` | Transpiler and API stubs |
| `Windows-Batch` | Transpiler and API stubs |

List installed platforms:

```bash
dotnet run --project ShellScript/ShellScript.csproj -- --platforms
```

### Types

| Type | Aliases | Notes |
|------|---------|--------|
| Integer | `int`, `long` | |
| Float | `float`, `double` | Often uses `awk` / `bc` / `python3` on Bash when needed |
| Number | `number` | Integer or floating-point |
| String | `string` | `'...'` and `"..."`; `$"..."` interpolation |
| Boolean | `bool` | `true` / `false` (not `boolean`) |
| Void | `void` | Function return only |
| Array | `T[]` | e.g. `int[]`, `string[]` |
| Delegate | `delegate` | Callable references |
| Object | `object` | Reserved / incomplete |

There is no `var` keyword; types must be written explicitly. Only integer-to-float widening is implicit; other conversions need a cast `(type)`.

### Control flow and structure

On **Unix-Bash**, the compiler currently emits (among others):

- `if` / `else`
- `while`, `do` / `while`, `foreach`, classic `for`
- `switch` / `case` / `default`
- `include "path.shellscript";` at **file root** only (see spec for resolution and `parts/` fragments)
- Preprocessor `#if` / `#elseif` / `#else` / `#endif` with **parenthesized** constant conditions

Features such as `throw`, `async`/`await`, and user-defined **classes** are reserved or incomplete—see the [feature matrix](docs/language/specification.md#15-implementation-feature-matrix) in the specification.

### Functions, `echo`, and shell semantics

Function syntax matches C#. Non-void functions must return on all paths where required by the compiler.

`echo` is a dedicated statement (parentheses optional). On Bash, inside value-returning functions, user-visible `echo` may be redirected so **stdout can carry the function result**—see [section 8.2](docs/language/specification.md#82-echo) and [section 12](docs/language/specification.md#12-functions-and-shell-result-semantics) of the specification.

```csharp
int abs(int x) {
    return x < 0 ? -x : x;
}
```

API calls such as `Math.Abs` may inline to target-native code when safe:

```csharp
int x = -23;
echo Math.Abs(x); // may become bash parameter expansion, e.g. ${x#-}
```

### Testing in examples

The **`Assert`** API (`Assert.Equals`, `Assert.True`, …) is used throughout `Examples/` for self-checking sample programs. Snapshot tests compare transpiled `.bash` output to `*.output.bash.txt` files (`dotnet test`).

### Preprocessor

Platform constants (e.g. `Bash`, `Unix` on the Bash backend) drive `#if (Bash)` branches at compile time. `#option` can set compiler flags—details in the [specification](docs/language/specification.md#10-preprocessor).

---

## Class library

Library definitions live in `ShellScript/Core/Language/Library/`. Bash implementations live under `ShellScript/Unix/Bash/Api/ClassLibrary/`.

After changing public API surface:

```bash
python3 scripts/generate-api-docs.py
```

Browse generated pages under [docs/api/](docs/api/). Legacy hand-written notes remain in [docs/ClassLibrary.md](docs/ClassLibrary.md); prefer the generated reference.

Only used APIs are emitted into the output script (plus shared helpers). You can override a generated helper by defining a function with the same name before first use.

---

## Command-line interface

Invocation: `shellscript <command> [arguments] [-switch[=value]]`

| Command | Description |
|---------|-------------|
| `help`, `-h`, `--help` | Show help |
| `--platforms` | List target platforms |
| `-v`, `--version` | Version string |
| `compile` | Compile `.shellscript` file(s); globs supported; output beside source or explicit path |
| `run` | Compile to Bash in a temp directory, execute, clean up |
| `test` | Run snapshot tests (`--snapshot`; optional globs; default `**/*.shellscript` under cwd) |
| `format` | Format `.shellscript` sources (optional globs; default indent 2 spaces) |
| `exec` | Run a source/project without writing a permanent compile artifact |
| `daemon` | Start the runtime daemon |

**Compile** requires a platform name (`Unix-Bash`, `Windows-PowerShell`, or `Windows-Batch`) as the last argument or via `--platform=...`.

```bash
dotnet run --project ShellScript/ShellScript.csproj -- \
  compile Examples/Api/File/File.shellscript Unix-Bash
```

Quote globs so the shell does not expand them before the CLI sees them:

```bash
dotnet run --project ShellScript/ShellScript.csproj -- \
  compile 'Examples/**/*.shellscript' Unix-Bash
```

---

## Contributing

Please read [docs/contributing.md](docs/contributing.md). When you change the class library, run `python3 scripts/generate-api-docs.py` and commit updates under `docs/api/`. Agent workflows are described in [AGENTS.md](AGENTS.md).

## Authors

* **Ali Mousavi Kherad** — owner

See also [contributors](https://github.com/amkherad/ShellScript/contributors).

## License

MIT — see [opensource.org/licenses/MIT](https://opensource.org/licenses/MIT).
