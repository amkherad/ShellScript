#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Process_RunAndCapture() {
  eval "$1"
}
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
#-------------------------------------------------------------------------------
pid=$$
if kill -0 "$1" 2>/dev/null; then
  h_Process_Exists_Result=1
else
  h_Process_Exists_Result=0
fi
running=$h_Process_Exists_Result
captured=`Process_RunAndCapture "printf capture-ok"`
echo "running=${running}"
echo "$captured"
Assert_Equals "capture-ok" "$captured"
echo "EXAMPLE_OK:Process"
