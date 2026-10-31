#!/usr/bin/env python3
"""Bootstrap phase-1 code generator: IR -> bash body (no shebang)."""

import sys
from pathlib import Path


def emit_bash(ir_lines: list[str]) -> str:
    out: list[str] = []
    for line in ir_lines:
        line = line.rstrip("\n")
        if not line:
            continue
        parts = line.split("\t")
        if len(parts) >= 3 and parts[0] == "STMT" and parts[1] == "ECHO":
            payload = parts[2]
            if payload.startswith("$"):
                var = payload[1:]
                out.append(f'echo "${{{var}}}"')
            else:
                out.append(f'echo "{payload}"')
    return "\n".join(out) + ("\n" if out else "")


def main() -> int:
    path = Path(sys.argv[1])
    lines = path.read_text(encoding="utf-8").splitlines()
    sys.stdout.write(emit_bash(lines))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
