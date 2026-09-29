---
title: String
parent: API Reference
nav_order: 16
generated: true
---

# `String`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `int String.Compare(string Other)`

"Compares two strings lexicographically

| Parameter | Type |
|-----------|------|
| `Other` | `string` |

### `int String.CompareIgnoreCase(string Other)`

"Compares two strings case-insensitively

| Parameter | Type |
|-----------|------|
| `Other` | `string` |

### `bool String.Contains(string Value, string Search)`

Checks whether a string contains a value.

| Parameter | Type |
|-----------|------|
| `Value` | `string` |
| `Search` | `string` |

### `bool String.ContainsIgnoreCase(string Search)`

Checks whether a string contains a value (case-insensitive).

| Parameter | Type |
|-----------|------|
| `Search` | `string` |

### `bool String.EndsWith(string Value, string Search)`

Checks whether a string ends with a value.

| Parameter | Type |
|-----------|------|
| `Value` | `string` |
| `Search` | `string` |

### `string String.GetAfter(string Delimiter)`

Returns the substring after the first occurrence of a delimiter.

| Parameter | Type |
|-----------|------|
| `Delimiter` | `string` |

### `string String.GetBefore(string Delimiter)`

Returns the substring before the first occurrence of a delimiter.

| Parameter | Type |
|-----------|------|
| `Delimiter` | `string` |

### `int String.GetLength()`

Returns the length of the string. (may vary depending on current locale)

### `int String.IndexOf(string Value)`

Returns the index of a substring, or -1 when not found.

| Parameter | Type |
|-----------|------|
| `Value` | `string` |

### `bool String.IsNullOrEmpty()`

Checks whether a string is null or empty.

### `bool String.IsNullOrWhiteSpace()`

Checks whether a string is null or only contains spaces.

### `string String.Join(string Separator, array Values)`

Joins string array elements with a separator.

| Parameter | Type |
|-----------|------|
| `Separator` | `string` |
| `Values` | `array` |

### `int String.LastIndexOf(string Value)`

Returns the last index of a substring, or -1 when not found.

| Parameter | Type |
|-----------|------|
| `Value` | `string` |

### `string String.PadLeft(int TotalWidth, string PadCharacter)`

Left-pads a string to a minimum width (space by default).

| Parameter | Type |
|-----------|------|
| `TotalWidth` | `int` |
| `PadCharacter` | `string` |

### `string String.PadRight(int TotalWidth, string PadCharacter)`

Right-pads a string to a minimum width (space by default).

| Parameter | Type |
|-----------|------|
| `TotalWidth` | `int` |
| `PadCharacter` | `string` |

### `string String.Repeat(int Count)`

Repeats a string a number of times.

| Parameter | Type |
|-----------|------|
| `Count` | `int` |

### `string String.Replace(string OldValue, string NewValue)`

Replaces occurrences of a substring.

| Parameter | Type |
|-----------|------|
| `OldValue` | `string` |
| `NewValue` | `string` |

### `string String.Split(string Delimiter)`

Splits a string by delimiter into a string array.

| Parameter | Type |
|-----------|------|
| `Delimiter` | `string` |

### `bool String.StartsWith(string Value, string Search)`

Checks whether a string starts with a value.

| Parameter | Type |
|-----------|------|
| `Value` | `string` |
| `Search` | `string` |

### `string String.Substring(int StartIndex, int Length)`

"Returns a substring starting at an index

| Parameter | Type |
|-----------|------|
| `StartIndex` | `int` |
| `Length` | `int` |

### `string String.ToLower()`

Returns the string in lowercase.

### `string String.ToUpper()`

Returns the string in uppercase.

### `string String.Trim()`

Removes leading and trailing whitespace.

### `string String.TrimEnd()`

Removes trailing whitespace.

### `string String.TrimStart()`

Removes leading whitespace.

