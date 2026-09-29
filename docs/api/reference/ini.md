---
title: Ini
parent: API Reference
nav_order: 27
generated: true
---

# `Ini`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `string Ini.GetValue(string FilePath, string Section, string Key)`

Reads a value from an INI file.

| Parameter | Type |
|-----------|------|
| `FilePath` | `string` |
| `Section` | `string` |
| `Key` | `string` |

### `void Ini.SetValue(string FilePath, string Section, string Key, string Value)`

Sets a value in an INI file.

| Parameter | Type |
|-----------|------|
| `FilePath` | `string` |
| `Section` | `string` |
| `Key` | `string` |
| `Value` | `string` |

