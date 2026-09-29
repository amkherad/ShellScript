---
title: StringBuilder
parent: API Reference
nav_order: 17
generated: true
---

# `StringBuilder`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `void StringBuilder.Append(string Value)`

Appends text to the builder (mutates the builder variable).

| Parameter | Type |
|-----------|------|
| `Value` | `string` |

### `void StringBuilder.AppendLine(string Value)`

"Appends text and a newline to the builder

| Parameter | Type |
|-----------|------|
| `Value` | `string` |

### `void StringBuilder.Clear()`

Clears the builder contents.

### `string StringBuilder.Create(string Initial)`

Creates a new string builder (empty string buffer). Pass an optional initial value.

| Parameter | Type |
|-----------|------|
| `Initial` | `string` |

### `int StringBuilder.GetLength()`

Returns the current length of the builder text.

