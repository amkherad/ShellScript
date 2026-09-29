---
title: Path
parent: API Reference
nav_order: 21
generated: true
---

# `Path`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `string Path.Combine()`

Combines two path components with one slash.

### `string Path.GetDirectoryName()`

Returns the directory component of a path.

### `string Path.GetExtension()`

Returns the final extension including the leading dot, or an empty string.

### `string Path.GetFileName()`

Returns the final component of a path.

### `string Path.GetFullPath()`

Returns the absolute path.

### `string Path.GetTempPath()`

Returns the directory for temporary files.

### `bool Path.IsPathRooted()`

Checks whether a path is absolute.

