# ShellScript API Documentation

## Core

### Array

#### `void` Array.Copy(`any[]` destination, `any[]` source)
Copies a source array into destination array.
```csharp
    int[] destination;
    int[] source = new int[] { 10, 20, 30, 40 };
    
    Array.Copy(destination, source);
    
    echo destination[0]; //10
    echo destination[1]; //20
    echo destination[2]; //30
    echo destination[3]; //40
```

#### `int` Array.GetLength(`any[]` array)
Returns the length of an array.
```csharp
    int[] array = new int[] { 10, 20 };
    
    echo Array.GetLength(array); //2
```


#### `void` Array.Initialize(`any[]` array)
Initializes an array with zero length.
```csharp
    int[] array;
    Array.Initialize(array);
```


### Convert

#### `T` Convert.ToInteger / ToFloat / ToNumber / ToBoolean(`any` value)
Coerces values at compile time when possible; otherwise emits `Convert_*` helpers with awk/bc/python fallbacks (same flags as `Math`).

#### `number` Convert.Parse(`string` value)
Parses a string into a number.

#### `string` Convert.ToString(`any` value)
Formats numbers and booleans as strings.


### String

Beyond `Contains` / `StartsWith` / `EndsWith`, the library includes `ToLower`, `ToUpper`, `Trim`, `GetBefore`, `GetAfter`, `IndexOf`, `Substring`, `Equals`, and `Replace`.

### Locale


### Math

Floating-point math uses generated helper functions with ordered fallbacks (`awk`, `bc`, `python`) controlled by compiler flags (`use-third-party-utilities`, `disabled-utilities`, `utility-order`, `bind-utilities-at-init`). Integer `Math.Abs` inlines to bash parameter expansion when possible.

#### `number` Math.Abs(`number` num)
Returns the absolute value of a number
```charp
    echo Math.Abs(-10); //10
```

#### `number` Math.Min(`number` a, `number` b) / Math.Max(...)
Returns the smaller or larger value.

#### `number` Math.Floor / Math.Ceiling / Math.Round / Math.Truncate(`number` num)
Rounding and integral conversion helpers.

#### `number` Math.Sqrt(`number` num) / Math.Pow(`number` base, `number` exponent)
Power and root helpers.

#### `int` Math.Sign(`number` num)
Returns `-1`, `0`, or `1`.


### Platform


### String


### User


## IO

### File

#### `bool` File.Exists / CanRead / CanWrite / CanExecute / IsFile / IsDirectory / IsLink
File metadata and permission checks (native `test`).

#### `string` File.ReadAllText(`string` path)
Reads file contents (`cat`).

#### `void` File.WriteAllText / AppendAllText(`string` path, `string` contents)
Writes or appends file contents.

#### `void` File.Delete / Copy / Move(...)
Deletes or transfers files (`rm`, `cp`, `mv`).

#### `int` File.GetLength(`string` path)
File size in bytes (`stat` with GNU/BSD fallback).

### Directory

#### `bool` Directory.Exists(`string` path)
Directory existence check.

#### `void` Directory.Create(`string` path)
Creates a directory tree (`mkdir -p`).

#### `void` Directory.Delete(`string` path) / DeleteRecursive(...)
Removes an empty directory (`rmdir`) or a directory tree (`rm -rf`).


## Network

### Net

#### `void` Net.Ping()
Forwards the call to native OS's ping command.

#### `void` Net.Download(`string` url, `string` path) / `string` Net.HttpGet(`string` url)
HTTP via `curl`.

#### `string` Net.ResolveHost(`string` host)
DNS lookup via `getent`.

## System

### OS
`GetHostName`, `GetKernelName`, `GetArchitecture`, `GetOsVersion`.

### Process
`GetCurrentId`, `GetParentId`, `Exists`, `Kill`, `Run`, `RunAndCapture`.

### Thread
`GetCurrentId` (maps to process id in bash).

## Data formats

### Ini / DotEnv / Json / Yaml / Xml
Structured configuration and serialization helpers. Json/Yaml prefer `jq`/`yq` with `python` fallbacks (compiler utility flags apply).

## Text, time, binary

### Text / Unicode / Regex / DateTime / Binary
Whitespace normalization, splitting, Unicode length (`python3`), regex match/replace (`grep`/`sed`), date formatting (`date`), base64 encode/decode.

## Diagnostics & CLI

### Log / Console
Leveled logging and stdout/stderr writers.

### Cli
Script argument access (`_SS_SCRIPT_ARGS` snapshot), flags, and flag values.

## Linux Bash Additions

The Unix-Bash platform exposes native implementations for the following APIs:

- `Environment.GetVariable`, `GetCurrentDirectory`, `GetHomeDirectory`, and `SetVariable`.
- `Path.Combine`, `GetFileName`, `GetDirectoryName`, `GetExtension`, `GetTempPath`, and `IsPathRooted`.
- `String.Contains`, `String.StartsWith`, and `String.EndsWith`.
- `File` and `Directory` IO helpers (read/write, copy, move, delete, length, mkdir).
- `File.IsDirectory` and `File.IsFile`, alongside the existing file permission and existence checks.
- `Math` helpers beyond `Abs` (min/max, rounding, pow/sqrt, sign) with utility fallbacks.
- Extended `File`/`Directory`/`Path`, `Array`, `OS`, `Process`, data-format APIs, `Log`/`Console`/`Cli`, and network helpers.
- Optional tools: `jq`, `yq`, `curl` (plus existing `awk`, `bc`, `python`).
- `User.GetUserName`, `User.IsSuperUser`, `Locale.GetCurrentLocale`, and `Net.Ping`.

Bash array operations use native expansions and loops. `Array.GetLength` emits `${#array[@]}`, while sized array initialization emits a collision-safe arithmetic loop and uses the element type's default value.
