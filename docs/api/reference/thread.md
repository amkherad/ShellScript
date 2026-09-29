---
title: Thread
parent: API Reference
nav_order: 25
generated: true
---

# `Thread`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `int Thread.GetCurrentId()`

Returns current thread identifier (process id in bash).

### `void Thread.Sleep(int Milliseconds)`

Suspends the current thread for the given number of milliseconds.

| Parameter | Type |
|-----------|------|
| `Milliseconds` | `int` |

