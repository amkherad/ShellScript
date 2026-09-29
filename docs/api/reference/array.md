---
title: Array
parent: API Reference
nav_order: 10
generated: true
---

# `Array`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `void Array.Add(void Value)`

Appends a value to an array.

| Parameter | Type |
|-----------|------|
| `Value` | `void` |

### `void Array.Clear()`

Removes all elements from an array.

### `bool Array.Contains(void Value)`

Checks whether array contains a value.

| Parameter | Type |
|-----------|------|
| `Value` | `void` |

### `void Array.Copy(array Destination, array Source)`

Copies an array to another one.

| Parameter | Type |
|-----------|------|
| `Destination` | `array` |
| `Source` | `array` |

### `int Array.GetLength()`

Returns the length of the array.

### `int Array.IndexOf(void Value)`

Returns index of value or -1.

| Parameter | Type |
|-----------|------|
| `Value` | `void` |

### `void Array.Initialize(array Array, int Length)`

Initializes an array with zeros.

| Parameter | Type |
|-----------|------|
| `Array` | `array` |
| `Length` | `int` |

### `void Array.Reverse()`

Reverses an array in place.

