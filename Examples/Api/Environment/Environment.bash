#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Environment_SetVariable() {
  export "$1=$2"
}
function Environment_GetVariable() {
  printf '%s' "${!1-}"
}
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
#-------------------------------------------------------------------------------
cwd=${PWD}
home=${HOME}
Environment_SetVariable "SS_EXAMPLE_MARKER" "set"
marker=`Environment_GetVariable "SS_EXAMPLE_MARKER"`
if [ -z "$cwd" ]; then
  h_String_IsNullOrEmpty_Result=1
else
  h_String_IsNullOrEmpty_Result=0
fi
cwdEmpty=$h_String_IsNullOrEmpty_Result
if [ -z "$home" ]; then
  h_String_IsNullOrEmpty_Result1=1
else
  h_String_IsNullOrEmpty_Result1=0
fi
homeSet=$((! $h_String_IsNullOrEmpty_Result1))
echo "marker=${marker}"
echo "cwdEmpty=${cwdEmpty}"
echo "homeSet=${homeSet}"
Assert_Equals "set" "$marker"
if ! [ $cwdEmpty -ne 0 ]; then :; else printf '%s\n' "Assert.False failed." >&2; return 1 2>/dev/null || exit 1; fi
if [ $homeSet -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
echo "EXAMPLE_OK:Environment"
