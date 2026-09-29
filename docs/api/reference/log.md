---
title: Log
parent: API Reference
nav_order: 38
generated: true
---

# `Log`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `void Log.Debug(string Message)`

Writes a debug message to stderr.

| Parameter | Type |
|-----------|------|
| `Message` | `string` |

### `void Log.Error(string Message)`

Writes an error to stderr.

| Parameter | Type |
|-----------|------|
| `Message` | `string` |

### `void Log.Info(string Message)`

Writes an info message to stderr.

| Parameter | Type |
|-----------|------|
| `Message` | `string` |

### `void Log.Warn(string Message)`

Writes a warning to stderr.

| Parameter | Type |
|-----------|------|
| `Message` | `string` |

