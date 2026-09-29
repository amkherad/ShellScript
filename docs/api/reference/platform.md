---
title: Platform
parent: API Reference
nav_order: 15
generated: true
---

# `Platform`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `void Platform.Call(string RawCommand)`

Takes a string and execute it as a void-result platform-dependent shell command.

| Parameter | Type |
|-----------|------|
| `RawCommand` | `string` |

### `string Platform.GetScriptDirectory()`

Returns the absolute directory containing the running script (Unix-Bash: directory of the compiled .bash file).

