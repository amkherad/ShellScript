---
title: Json
parent: API Reference
nav_order: 28
generated: true
---

# `Json`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `string Json.GetPath(string JsonText, string Path)`

Reads a value using a jq-style path.

| Parameter | Type |
|-----------|------|
| `JsonText` | `string` |
| `Path` | `string` |

### `bool Json.IsValid(string JsonText)`

Checks whether text is valid JSON.

| Parameter | Type |
|-----------|------|
| `JsonText` | `string` |

### `string Json.PrettyPrint(string JsonText)`

Formats JSON text.

| Parameter | Type |
|-----------|------|
| `JsonText` | `string` |

