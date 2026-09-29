---
title: Binary
parent: API Reference
nav_order: 31
generated: true
---

# `Binary`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `string Binary.FromBase64(string Base64Data)`

Decodes base64 to a raw string.

| Parameter | Type |
|-----------|------|
| `Base64Data` | `string` |

### `string Binary.ToBase64(string Data)`

Encodes raw bytes (string) to base64.

| Parameter | Type |
|-----------|------|
| `Data` | `string` |

