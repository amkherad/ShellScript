---
title: Environment
parent: API Reference
nav_order: 12
generated: true
---

# `Environment`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `string Environment.GetCurrentDirectory()`

Returns the current working directory.

### `string Environment.GetHomeDirectory()`

Returns the current user home directory.

### `string Environment.GetVariable()`

Returns the value of an environment variable or an empty string when it is unset.

### `void Environment.SetVariable(string Value)`

Sets an environment variable for the current process.

| Parameter | Type |
|-----------|------|
| `Value` | `string` |

