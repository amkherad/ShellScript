---
title: Yaml
parent: API Reference
nav_order: 30
generated: true
---

# `Yaml`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `string Yaml.GetPath(string YamlText, string Path)`

Reads a value from YAML using a dotted path.

| Parameter | Type |
|-----------|------|
| `YamlText` | `string` |
| `Path` | `string` |

### `bool Yaml.IsValid(string YamlText)`

Checks whether text is valid YAML.

| Parameter | Type |
|-----------|------|
| `YamlText` | `string` |

