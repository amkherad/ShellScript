---
title: Assert
parent: API Reference
nav_order: 39
generated: true
---

# `Assert`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `void Assert.Fail(string Message)`

Unconditionally fails the test with a message.

| Parameter | Type |
|-----------|------|
| `Message` | `string` |

### `void Assert.False(bool Condition, string Message)`

Fails when the condition is true.

| Parameter | Type |
|-----------|------|
| `Condition` | `bool` |
| `Message` | `string` |

### `void Assert.NotEquals(void Expected, void Actual, string Message)`

Fails when the expected and actual values are equal.

| Parameter | Type |
|-----------|------|
| `Expected` | `void` |
| `Actual` | `void` |
| `Message` | `string` |

### `void Assert.True(bool Condition, string Message)`

Fails when the condition is false.

| Parameter | Type |
|-----------|------|
| `Condition` | `bool` |
| `Message` | `string` |

