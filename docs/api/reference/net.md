---
title: Net
parent: API Reference
nav_order: 22
generated: true
---

# `Net`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `void Net.Download(string Url, string DestinationPath)`

Downloads a URL to a file path.

| Parameter | Type |
|-----------|------|
| `Url` | `string` |
| `DestinationPath` | `string` |

### `string Net.GetNetworkInterfaces()`

Returns network interface names on the local machine.

### `string Net.GetOpenSockets()`

Returns lines describing open TCP/UDP sockets (state, addresses, process) from the OS.

### `string Net.GetOpenSocketsOnInterface(string InterfaceName)`

Returns open socket lines bound to the given interface (empty name lists all).

| Parameter | Type |
|-----------|------|
| `InterfaceName` | `string` |

### `int Net.GetSocketProcessId(string SocketLine)`

Returns the owning process id parsed from a socket line (ss or netstat), or 0 when unknown.

| Parameter | Type |
|-----------|------|
| `SocketLine` | `string` |

### `string Net.HttpGet(string Url)`

Performs an HTTP GET and returns the response body.

| Parameter | Type |
|-----------|------|
| `Url` | `string` |

### `bool Net.Ping()`

Checks whether an endpoint responds to one ICMP echo request.

### `string Net.ResolveHost(string HostName)`

Resolves a host name to an address.

| Parameter | Type |
|-----------|------|
| `HostName` | `string` |

