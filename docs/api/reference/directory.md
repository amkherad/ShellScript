---
title: Directory
parent: API Reference
nav_order: 19
generated: true
---

# `Directory`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `void Directory.Copy(string DestinationPath)`

Recursively copies a directory.

| Parameter | Type |
|-----------|------|
| `DestinationPath` | `string` |

### `void Directory.Create()`

Creates a directory and any parent directories.

### `void Directory.Delete()`

Deletes an empty directory.

### `void Directory.DeleteRecursive()`

Recursively deletes a directory and its contents.

### `bool Directory.Exists()`

Checks whether a directory exists.

### `string Directory.GetDirectories()`

Lists subdirectories.

### `string Directory.GetFiles()`

Lists files in a directory (non-recursive).

### `void Directory.Move(string DestinationPath)`

Moves or renames a directory.

| Parameter | Type |
|-----------|------|
| `DestinationPath` | `string` |

