#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
function Assert_NotEquals() {
  if [[ "$1" != "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.NotEquals failed: both values were "%s"\n' "$1" >&2; fi
  return 1 2>/dev/null || exit 1
}
#-------------------------------------------------------------------------------
Assert_Equals 1 1
if [ 1 -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
Assert_NotEquals "x" "y"
echo "ASSERT_OK"
