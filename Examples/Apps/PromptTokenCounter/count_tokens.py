#!/usr/bin/env python3
"""Count tokens for major LLM providers (best-effort; tiktoken when installed)."""

from __future__ import annotations

import argparse
import json
import math
import sys
from pathlib import Path

# chars-per-token fallbacks when no provider library is available (documented estimates).
_HEURISTICS = {
    "openai": 4.0,
    "openai-o200k": 4.0,
    "anthropic": 3.45,
    "google": 4.0,
    "meta": 3.85,
    "mistral": 3.8,
    "xai": 4.0,
    "cohere": 4.2,
    "amazon": 4.0,
}

_OPENAI_ENCODINGS = {
    "openai": "cl100k_base",
    "openai-o200k": "o200k_base",
    "xai": "o200k_base",
}


def _read_text(args: argparse.Namespace) -> str:
    if args.text is not None:
        return args.text
    if args.file is not None:
        return Path(args.file).read_text(encoding="utf-8")
    if not sys.stdin.isatty():
        return sys.stdin.read()
    raise SystemExit("Provide --text, --file, or stdin.")


def _heuristic_count(text: str, platform: str) -> int:
    cpt = _HEURISTICS.get(platform, 4.0)
    return max(1, int(math.ceil(len(text) / cpt)))


def _tiktoken_count(text: str, encoding_name: str) -> int | None:
    try:
        import tiktoken  # type: ignore
    except ImportError:
        return None
    try:
        enc = tiktoken.get_encoding(encoding_name)
    except Exception:
        return None
    return len(enc.encode(text))


def count_tokens(text: str, platform: str) -> tuple[int, str]:
    platform = platform.lower().strip()
    if platform not in _HEURISTICS:
        raise SystemExit(
            "Unknown platform '"
            + platform
            + "'. Supported: "
            + ", ".join(sorted(_HEURISTICS.keys()))
        )

    if platform in _OPENAI_ENCODINGS:
        enc = _OPENAI_ENCODINGS[platform]
        n = _tiktoken_count(text, enc)
        if n is not None:
            return n, "tiktoken:" + enc

    # Anthropic: optional anthropic SDK tokenizer (if user installed anthropic package).
    if platform == "anthropic":
        try:
            from anthropic import Anthropic  # type: ignore

            client = Anthropic()
            if hasattr(client, "count_tokens"):
                n = client.count_tokens(text)
                return int(n), "anthropic-sdk"
        except Exception:
            pass

    n = _heuristic_count(text, platform)
    return n, "heuristic:" + platform


def main() -> None:
    parser = argparse.ArgumentParser(description="Count LLM prompt tokens by platform.")
    parser.add_argument(
        "--platform",
        default="openai",
        help="openai, openai-o200k, anthropic, google, meta, mistral, xai, cohere, amazon",
    )
    parser.add_argument("--text", help="Prompt text (alternative to --file / stdin)")
    parser.add_argument("--file", help="Path to UTF-8 prompt file")
    parser.add_argument("--json", action="store_true", help="Emit JSON object")
    args = parser.parse_args()

    text = _read_text(args)
    tokens, method = count_tokens(text, args.platform)

    if args.json:
        print(
            json.dumps(
                {
                    "platform": args.platform.lower(),
                    "tokens": tokens,
                    "method": method,
                    "characters": len(text),
                }
            )
        )
    else:
        print(tokens)
        print("method=" + method)
        print("characters=" + str(len(text)))


if __name__ == "__main__":
    main()
