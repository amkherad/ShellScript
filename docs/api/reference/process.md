---
title: Process
parent: API Reference
nav_order: 24
generated: true
---

# `Process`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `bool Process.Exists(int ProcessId)`

Checks whether a process id exists.

| Parameter | Type |
|-----------|------|
| `ProcessId` | `int` |

### `int Process.GetCurrentId()`

Returns the current process id.

### `string Process.GetName(int ProcessId)`

Returns the short command name for a process id, or empty when unknown.

| Parameter | Type |
|-----------|------|
| `ProcessId` | `int` |

### `int Process.GetParentId()`

Returns the parent process id.

### `void Process.Kill(int ProcessId)`

Sends SIGTERM to a process.

| Parameter | Type |
|-----------|------|
| `ProcessId` | `int` |

### `void Process.Run(string Command)`

Runs a shell command.

| Parameter | Type |
|-----------|------|
| `Command` | `string` |

### `string Process.RunAndCapture(string Command)`

Runs a command and returns combined stdout.

| Parameter | Type |
|-----------|------|
| `Command` | `string` |

