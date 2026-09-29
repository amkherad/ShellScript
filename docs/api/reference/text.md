---
title: Text
parent: API Reference
nav_order: 34
generated: true
---

# `Text`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `string Text.NormalizeWhitespace(string Text)`

Collapses whitespace runs to single spaces and trims.

| Parameter | Type |
|-----------|------|
| `Text` | `string` |

### `string Text.Split(string Text, string Delimiter)`

Splits text by delimiter into an array.

| Parameter | Type |
|-----------|------|
| `Text` | `string` |
| `Delimiter` | `string` |

