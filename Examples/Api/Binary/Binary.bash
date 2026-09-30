#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Binary_ToBase64() {
  printf '%s' "$1" | base64 -w 0
}
function Binary_FromBase64() {
  printf '%s' "$1" | base64 -d
}
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
#-------------------------------------------------------------------------------
encoded=`Binary_ToBase64 "hi"`
decoded=`Binary_FromBase64 "$encoded"`
echo "$decoded"
Assert_Equals "hi" "$decoded"
echo "EXAMPLE_OK:Binary"
