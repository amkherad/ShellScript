---
title: Cli
parent: API Reference
nav_order: 36
generated: true
---

# `Cli`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `string Cli.GetArgument(int Index)`

Returns argument at zero-based index.

| Parameter | Type |
|-----------|------|
| `Index` | `int` |

### `int Cli.GetArgumentCount()`

Returns script argument count.

### `string Cli.GetFlagValue(string Flag)`

Returns value after --flag= or next argument.

| Parameter | Type |
|-----------|------|
| `Flag` | `string` |

### `bool Cli.HasFlag(string Flag)`

Checks whether a flag exists in script arguments.

| Parameter | Type |
|-----------|------|
| `Flag` | `string` |

