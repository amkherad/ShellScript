#!/usr/bin/env python3
"""Bootstrap phase-1 parser: token file -> IR (STMT\\tECHO\\t...)."""

import sys
from pathlib import Path


def parse_tokens(lines: list[str]) -> list[str]:
    ir: list[str] = []
    i = 0
    while i < len(lines):
        line = lines[i].rstrip("\n")
        if not line:
            i += 1
            continue
        parts = line.split("\t", 1)
        if len(parts) != 2:
            i += 1
            continue
        kind, value = parts
        if kind == "KEYWORD" and value == "echo":
            j = i + 1
            while j < len(lines):
                p = lines[j].split("\t", 1)
                if len(p) != 2:
                    j += 1
                    continue
                k2, v2 = p
                if k2 == "STRING":
                    ir.append(f"STMT\tECHO\t{v2}")
                    break
                if k2 == "IDENT":
                    ir.append(f"STMT\tECHO\t${v2}")
                    break
                if k2 == "PUNCT" and v2 == ";":
                    break
                j += 1
        i += 1
    return ir


def main() -> int:
    path = Path(sys.argv[1])
    lines = path.read_text(encoding="utf-8").splitlines()
    for row in parse_tokens(lines):
        print(row)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
