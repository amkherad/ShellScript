---
title: File
parent: API Reference
nav_order: 20
generated: true
---

# `File`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `void File.AppendAllText(string Contents)`

Appends text to a file.

| Parameter | Type |
|-----------|------|
| `Contents` | `string` |

### `bool File.CanExecute()`

Checks whether a file has execute permission.

### `bool File.CanRead()`

Checks whether a file has read permission.

### `bool File.CanWrite()`

Checks whether a file has write permission.

### `void File.Copy(string DestinationPath)`

Copies a file to a new location.

| Parameter | Type |
|-----------|------|
| `DestinationPath` | `string` |

### `void File.CreateSymbolicLink(string LinkPath)`

Creates a symbolic link.

| Parameter | Type |
|-----------|------|
| `LinkPath` | `string` |

### `void File.Delete()`

Deletes a file.

### `bool File.Exists()`

Checks whether a file exists.

### `int File.GetLastWriteTime()`

Returns last modification time as Unix seconds.

### `int File.GetLength()`

Returns the size of a file in bytes.

### `bool File.IsDirectory()`

Checks whether a path is a directory.

### `bool File.IsFile()`

Checks whether a path is a regular file.

### `bool File.IsLink()`

Checks whether a file exists and is a link to another file.

### `void File.Move(string DestinationPath)`

Moves or renames a file.

| Parameter | Type |
|-----------|------|
| `DestinationPath` | `string` |

### `string File.ReadAllBytesBase64()`

Reads a file and returns base64-encoded contents.

### `string File.ReadAllLines()`

Reads all lines of a file.

### `string File.ReadAllText()`

Reads the entire contents of a file.

### `void File.WriteAllBytesBase64(string Base64Data)`

Writes base64-encoded binary data to a file.

| Parameter | Type |
|-----------|------|
| `Base64Data` | `string` |

### `void File.WriteAllLines(void Lines)`

Writes lines to a file.

| Parameter | Type |
|-----------|------|
| `Lines` | `void` |

### `void File.WriteAllText(string Contents)`

Creates or overwrites a file with the given text.

| Parameter | Type |
|-----------|------|
| `Contents` | `string` |

