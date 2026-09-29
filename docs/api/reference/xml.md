---
title: Xml
parent: API Reference
nav_order: 29
generated: true
---

# `Xml`

Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) implement subsets of these members.

## Methods

### `string Xml.GetPath(string XmlText, string XPath)`

Reads text from XML using an XPath expression.

| Parameter | Type |
|-----------|------|
| `XmlText` | `string` |
| `XPath` | `string` |

### `bool Xml.IsValid(string XmlText)`

Checks whether text is well-formed XML.

| Parameter | Type |
|-----------|------|
| `XmlText` | `string` |

