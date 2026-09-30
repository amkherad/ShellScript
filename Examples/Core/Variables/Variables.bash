#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
#-------------------------------------------------------------------------------
count=10
big=1000000
ratio=3.5
precise=0.015
either=42
fractional=3.14
greeting="Hello ShellScript"
ready=1
count=$(($count + 1))
greeting="count is ${count}"
echo "$greeting"
echo "$ready"
Assert_Equals 11 $count
if [ $ready -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
