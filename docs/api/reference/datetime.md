---
title: DateTime
parent: API Reference
nav_order: 32
generated: true
---

# `DateTime`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `string DateTime.Format(int UnixSeconds, string Format)`

Formats Unix seconds with a date format string.

| Parameter | Type |
|-----------|------|
| `UnixSeconds` | `int` |
| `Format` | `string` |

### `string DateTime.Now()`

Returns local time formatted string.

### `int DateTime.ToUnixTime()`

Returns current Unix timestamp in seconds.

### `string DateTime.UtcNow()`

Returns UTC time formatted string.

