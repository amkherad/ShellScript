---
title: Regex
parent: API Reference
nav_order: 33
generated: true
---

# `Regex`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `bool Regex.IsMatch(string Input, string Pattern)`

Checks whether input matches a regular expression.

| Parameter | Type |
|-----------|------|
| `Input` | `string` |
| `Pattern` | `string` |

### `string Regex.Replace(string Input, string Pattern, string Replacement)`

Replaces regex matches in input.

| Parameter | Type |
|-----------|------|
| `Input` | `string` |
| `Pattern` | `string` |
| `Replacement` | `string` |

