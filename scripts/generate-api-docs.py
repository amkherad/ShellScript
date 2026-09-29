#!/usr/bin/env python3
"""Generate Jekyll API reference pages from ShellScript Core API definitions."""

from __future__ import annotations

import re
import sys
from dataclasses import dataclass, field
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[1]
LIB_ROOT = REPO_ROOT / "ShellScript" / "Core" / "Language" / "Library"
OUT_DIR = REPO_ROOT / "docs" / "api" / "reference"

TYPE_MAP = {
    "TypeDescriptor.Void": "void",
    "TypeDescriptor.String": "string",
    "TypeDescriptor.Integer": "int",
    "TypeDescriptor.Boolean": "bool",
    "TypeDescriptor.Numeric": "number",
    "TypeDescriptor.Float": "float",
    "TypeDescriptor.Array": "array",
}

GROUP_ORDER = [
    ("Core", ["Array", "Convert", "Environment", "Locale", "Math", "Platform", "String", "StringBuilder", "User"]),
    ("IO", ["Directory", "File", "Path"]),
    ("Network", ["Net"]),
    ("System", ["OS", "Process", "Thread"]),
    ("Data", ["DotEnv", "Ini", "Json", "Xml", "Yaml"]),
    ("Text & time", ["Binary", "DateTime", "Regex", "Text", "Unicode"]),
    ("Diagnostics & CLI", ["Cli", "Console", "Log"]),
]

GROUP_BY_CLASS = {}
for group, names in GROUP_ORDER:
    for name in names:
        GROUP_BY_CLASS[name] = group


@dataclass
class ApiMethod:
    name: str
    summary: str
    return_type: str
    parameters: list[tuple[str, str]] = field(default_factory=list)


@dataclass
class ApiClass:
    access_name: str
    source_files: list[Path] = field(default_factory=list)
    methods: list[ApiMethod] = field(default_factory=list)


def slugify(name: str) -> str:
    return name.lower()


def parse_type_descriptor(expr: str) -> str:
    expr = expr.strip()
    if expr in TYPE_MAP:
        return TYPE_MAP[expr]
    m = re.search(r"DataTypes\.(\w+)", expr)
    if m:
        bits = m.group(1)
        if "Array" in bits:
            return "string[]" if "String" in bits else "array"
        if bits == "String":
            return "string"
    m = re.search(r"new TypeDescriptor\(([^)]+)\)", expr)
    if m:
        inner = m.group(1)
        if "Array" in inner and "String" in inner:
            return "string[]"
        if "Array" in inner:
            return "array"
    return "unknown"


def extract_class_access_name(text: str) -> str | None:
    m = re.search(
        r'public const string ClassAccessName\s*=\s*"([^"]+)"', text
    )
    return m.group(1) if m else None


def extract_method_name(block: str) -> str | None:
    m = re.search(r"public override string Name\s*=>\s*nameof\((\w+)\)", block)
    if m:
        return m.group(1)
    m = re.search(r'public override string Name\s*=>\s*"([^"]+)"', block)
    return m.group(1) if m else None


def extract_summary(block: str) -> str:
    m = re.search(
        r"public override string Summary\s*=>\s*(.+?);",
        block,
        re.DOTALL,
    )
    if not m:
        return ""
    raw = m.group(1).strip()
    parts = re.findall(r'"([^"]*)"', raw, re.DOTALL)
    if parts:
        return " ".join("".join(parts).split())
    return " ".join(raw.split())


def extract_return_type(block: str) -> str:
    m = re.search(
        r"public override TypeDescriptor TypeDescriptor\s*=>\s*([^;]+);",
        block,
        re.DOTALL,
    )
    if not m:
        return "unknown"
    return parse_type_descriptor(m.group(1))


def extract_parameters(block: str) -> list[tuple[str, str]]:
    params: list[tuple[str, str]] = []
    for m in re.finditer(
        r"new FunctionParameterDefinitionStatement\(\s*TypeDescriptor\.(\w+),\s*\"([^\"]+)\"",
        block,
    ):
        ptype = TYPE_MAP.get(f"TypeDescriptor.{m.group(1)}", m.group(1).lower())
        params.append((ptype, m.group(2)))
    for m in re.finditer(
        r"new FunctionParameterDefinitionStatement\(\s*new TypeDescriptor\([^)]+\),\s*\"([^\"]+)\"",
        block,
    ):
        params.append(("array", m.group(1)))
    return params


def partial_class_key(text: str) -> str | None:
    m = re.search(r"public abstract partial class (Api\w+)", text)
    if m:
        return m.group(1)
    m = re.search(r"public partial class (Api\w+)", text)
    return m.group(1) if m else None


def access_name_from_key(key: str, text: str) -> str:
    access = extract_class_access_name(text)
    if access:
        return access
    if key.startswith("Api") and len(key) > 3:
        return key[3:]
    return key


def iter_api_function_bodies(text: str):
    needle = "public abstract class"
    idx = 0
    while True:
        start = text.find(needle, idx)
        if start == -1:
            return
        header_end = text.find("{", start)
        if header_end == -1:
            return
        header = text[start:header_end]
        if ": ApiBaseFunction" not in header:
            idx = start + len(needle)
            continue
        depth = 0
        i = header_end
        while i < len(text):
            ch = text[i]
            if ch == "{":
                depth += 1
            elif ch == "}":
                depth -= 1
                if depth == 0:
                    yield text[header_end + 1 : i]
                    idx = i + 1
                    break
            i += 1
        else:
            return


def parse_api_file(path: Path, registry: dict[str, ApiClass]) -> None:
    text = path.read_text(encoding="utf-8")
    key = partial_class_key(text)
    if not key:
        return
    access = access_name_from_key(key, text)
    if access not in registry:
        registry[access] = ApiClass(access_name=access)
    registry[access].source_files.append(path)

    for body in iter_api_function_bodies(text):
        name = extract_method_name(body)
        if not name:
            continue
        method = ApiMethod(
            name=name,
            summary=extract_summary(body),
            return_type=extract_return_type(body),
            parameters=extract_parameters(body),
        )
        registry[access].methods.append(method)


def format_signature(method: ApiMethod, class_name: str) -> str:
    args = ", ".join(f"{t} {n}" for t, n in method.parameters)
    return f"{method.return_type} {class_name}.{method.name}({args})"


def write_class_page(api: ApiClass, nav_order: int) -> None:
    slug = slugify(api.access_name)
    lines = [
        "---",
        f"title: {api.access_name}",
        "parent: API Reference",
        f"nav_order: {nav_order}",
        "generated: true",
        "---",
        "",
        f"# `{api.access_name}`",
        "",
        "Static class library API. Platform backends (Unix-Bash, Windows-PowerShell, Windows-Batch) "
        "implement subsets of these members.",
        "",
    ]
    if not api.methods:
        lines.append("_No methods discovered in core definitions._")
    else:
        lines.append("## Methods")
        lines.append("")
        for method in sorted(api.methods, key=lambda m: m.name):
            sig = format_signature(method, api.access_name)
            lines.append(f"### `{sig}`")
            lines.append("")
            if method.summary:
                lines.append(method.summary)
            else:
                lines.append("_No summary in source._")
            lines.append("")
            if method.parameters:
                lines.append("| Parameter | Type |")
                lines.append("|-----------|------|")
                for ptype, pname in method.parameters:
                    lines.append(f"| `{pname}` | `{ptype}` |")
                lines.append("")
    path = OUT_DIR / f"{slug}.md"
    path.write_text("\n".join(lines) + "\n", encoding="utf-8")


def write_index(classes: dict[str, ApiClass]) -> None:
    lines = [
        "---",
        "title: API Reference",
        "nav_order: 3",
        "has_children: true",
        "---",
        "",
        "# API Reference",
        "",
        "Generated from `ShellScript/Core/Language/Library` via "
        "`scripts/generate-api-docs.py`. Re-run the script after changing API definitions.",
        "",
        "```bash",
        "python3 scripts/generate-api-docs.py",
        "```",
        "",
        "Example scripts live in the repository under `Examples/Api/`.",
        "",
    ]
    nav = 10
    for group, names in GROUP_ORDER:
        lines.append(f"## {group}")
        lines.append("")
        for name in names:
            if name not in classes:
                continue
            slug = slugify(name)
            count = len(classes[name].methods)
            lines.append(f"- [{name}](reference/{slug}.html) — {count} method(s)")
            nav += 1
        lines.append("")

    # Any classes not in GROUP_ORDER
    other = sorted(
        set(classes.keys()) - {n for _, ns in GROUP_ORDER for n in ns}
    )
    if other:
        lines.append("## Other")
        lines.append("")
        for name in other:
            slug = slugify(name)
            count = len(classes[name].methods)
            lines.append(f"- [{name}](reference/{slug}.html) — {count} method(s)")
        lines.append("")

    (REPO_ROOT / "docs" / "api" / "index.md").write_text(
        "\n".join(lines) + "\n", encoding="utf-8"
    )


def main() -> int:
    if not LIB_ROOT.is_dir():
        print(f"Library path not found: {LIB_ROOT}", file=sys.stderr)
        return 1

    OUT_DIR.mkdir(parents=True, exist_ok=True)
    classes: dict[str, ApiClass] = {}

    for path in sorted(LIB_ROOT.rglob("Api*.cs")):
        parse_api_file(path, classes)

    # Deduplicate methods by name per class
    for api in classes.values():
        seen: set[str] = set()
        unique: list[ApiMethod] = []
        for m in api.methods:
            if m.name in seen:
                continue
            seen.add(m.name)
            unique.append(m)
        api.methods = unique

    nav = 10
    for _, names in GROUP_ORDER:
        for name in names:
            if name in classes:
                write_class_page(classes[name], nav)
                nav += 1
    for name in sorted(classes.keys()):
        if name not in GROUP_BY_CLASS:
            write_class_page(classes[name], nav)
            nav += 1

    write_index(classes)
    print(f"Wrote API docs for {len(classes)} classes to {OUT_DIR}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
