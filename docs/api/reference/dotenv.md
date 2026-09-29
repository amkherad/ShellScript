---
title: DotEnv
parent: API Reference
nav_order: 26
generated: true
---

# `DotEnv`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `string DotEnv.Get(string FilePath, string Key)`

Reads a variable from a .env file without exporting it.

| Parameter | Type |
|-----------|------|
| `FilePath` | `string` |
| `Key` | `string` |

### `void DotEnv.Load(string FilePath)`

Loads KEY=VALUE pairs from a .env file into the environment.

| Parameter | Type |
|-----------|------|
| `FilePath` | `string` |

