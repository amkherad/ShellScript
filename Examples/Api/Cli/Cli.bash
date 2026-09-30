#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
_SS_SCRIPT_ARGS=("$@")
function Cli_HasFlag() {
  local f="$1" a
  for a in "${_SS_SCRIPT_ARGS[@]}"; do [ "$a" = "$f" ] && echo 1 && return; done
  echo 0
}
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
#-------------------------------------------------------------------------------
count=${#_SS_SCRIPT_ARGS[@]}
first=${_SS_SCRIPT_ARGS[0]}
verbose=`Cli_HasFlag "--verbose"`
echo "count=${count}"
echo "$first"
echo "verbose=${verbose}"
if [ $count -ge 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
if [ $count -gt 0 ]
then
  Assert_Equals "run" "$first"
  if [ $verbose -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
fi
echo "EXAMPLE_OK:Cli"
