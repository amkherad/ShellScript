---
title: Console
parent: API Reference
nav_order: 37
generated: true
---

# `Console`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `void Console.BeginBatchWrite()`

Redirects subsequent stdout writes to an in-memory buffer until EndBatchWrite (reduces TUI flicker).

### `void Console.Clear()`

Clears the terminal screen and moves the cursor to the home position.

### `bool Console.ConsumeInterruptRequest()`

"Returns true once after Ctrl+C (SIGINT) or SIGTERM while in interactive input mode

### `void Console.DisableErrorLog()`

Stops mirroring Console.WriteError to the file set by EnableErrorLog.

### `void Console.DisableMouseReporting()`

Disables mouse reporting enabled by EnableMouseReporting.

### `void Console.EnableErrorLog(string FilePath, bool Append)`

Mirrors subsequent Console.WriteError output to a file (stderr is still written). Must be called explicitly.

| Parameter | Type |
|-----------|------|
| `FilePath` | `string` |
| `Append` | `bool` |

### `void Console.EnableMouseReporting()`

Enables SGR click/wheel mouse reporting (use with EnterInteractiveInputMode). Pair with DisableMouseReporting on exit.

### `void Console.EndBatchWrite()`

Flushes the batch buffer to the terminal in one write.

### `void Console.EnterInteractiveInputMode()`

Puts stdin in non-canonical, no-echo mode for TUI input (restored by ExitInteractiveInputMode). Ctrl+C sets a flag read by ConsumeInterruptRequest.

### `void Console.ExitInteractiveInputMode()`

Restores stdin tty settings saved by EnterInteractiveInputMode.

### `int Console.GetWindowHeight()`

Returns terminal height in rows.

### `int Console.GetWindowWidth()`

Returns terminal width in columns.

### `bool Console.IsStdinTerminal()`

Returns whether standard input is connected to an interactive terminal (required for keyboard/mouse input).

### `bool Console.IsTerminal()`

Returns whether standard output is connected to an interactive terminal.

### `void Console.MoveCursorHome()`

Moves the cursor to the top-left without clearing the screen.

### `string Console.ReadKey(bool echoKey)`

Reads a single key from the terminal without requiring Enter (echoKey shows the key as typed).

| Parameter | Type |
|-----------|------|
| `echoKey` | `bool` |

### `string Console.ReadKeyTimeout(int TimeoutMilliseconds, bool echoKey)`

"Reads a single key when available before the timeout

| Parameter | Type |
|-----------|------|
| `TimeoutMilliseconds` | `int` |
| `echoKey` | `bool` |

### `string Console.ReadLine()`

Reads a line of text from standard input (no trailing newline).

### `string Console.ReadTerminalInputTimeout(int TimeoutMilliseconds, bool echoKey)`

Reads a key or mouse event before the timeout. Returns an empty string when idle.

| Parameter | Type |
|-----------|------|
| `TimeoutMilliseconds` | `int` |
| `echoKey` | `bool` |

### `string Console.ReadText(string Prompt)`

Writes an optional prompt, then reads a line of text from standard input.

| Parameter | Type |
|-----------|------|
| `Prompt` | `string` |

### `void Console.SetCursorPosition(int Row, int Column)`

Moves the cursor to a 1-based row and column (ANSI), for TUI overlays and dialogs.

| Parameter | Type |
|-----------|------|
| `Row` | `int` |
| `Column` | `int` |

### `void Console.Write(string Message)`

Writes text to stdout without a trailing newline.

| Parameter | Type |
|-----------|------|
| `Message` | `string` |

### `void Console.WriteError(string Message)`

Writes a line to stderr. Also appends to the file set by EnableErrorLog when that feature is enabled.

| Parameter | Type |
|-----------|------|
| `Message` | `string` |

### `void Console.WriteLine(string Message)`

Writes a line to stdout.

| Parameter | Type |
|-----------|------|
| `Message` | `string` |

