#!/usr/bin/env python3
"""Bootstrap phase-1 lexer: emit KIND<TAB>VALUE lines (see parts/lexer.shellscript)."""

import re
import sys
from pathlib import Path

KEYWORDS = frozenset(
    """
    const void int long bool double float number string object any delegate
    if else for foreach while do loop return new include echo true false null nil
    switch case default class throw async await
    """.split()
)


def tokenize(source: str) -> list[tuple[str, str]]:
    tokens: list[tuple[str, str]] = []
    i = 0
    n = len(source)

    def append(kind: str, value: str) -> None:
        tokens.append((kind, value))

    while i < n:
        ch = source[i]
        if ch in " \t\r":
            i += 1
            continue
        if ch == "\n":
            i += 1
            continue
        if source.startswith("//", i):
            j = source.find("\n", i)
            i = n if j < 0 else j + 1
            continue
        if ch in "\"'":
            quote = ch
            j = i + 1
            while j < n and source[j] != quote:
                j += 1
            append("STRING", source[i + 1 : j])
            i = j + 1 if j < n else n
            continue
        if ch.isalpha() or ch == "_":
            j = i + 1
            while j < n and (source[j].isalnum() or source[j] == "_"):
                j += 1
            word = source[i:j]
            kind = "KEYWORD" if word in KEYWORDS else "IDENT"
            append(kind, word)
            i = j
            continue
        if ch.isdigit():
            j = i + 1
            while j < n and (source[j].isdigit() or source[j] == "."):
                j += 1
            append("NUMBER", source[i:j])
            i = j
            continue
        append("PUNCT", ch)
        i += 1

    return tokens


def main() -> int:
    path = Path(sys.argv[1])
    text = path.read_text(encoding="utf-8")
    for kind, value in tokenize(text):
        print(f"{kind}\t{value}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
