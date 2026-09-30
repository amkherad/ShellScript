#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Environment_GetVariable() {
  printf '%s' "${!1-}"
}
function Path_Combine() {
  if [[ -z $1 ]]; then
        printf '%s' "$2"
  elif [[ -z $2 ]]; then
        printf '%s' "$1"
  elif [[ $1 == */ ]]; then
        printf '%s%s' "$1" "${2#/}"
  else
        printf '%s/%s' "$1" "${2#/}"
  fi
}
function File_WriteAllText() {
  printf '%s' "$2" > "$1"
}
function DotEnv_Get() {
  grep -E "^$2=" "$1" | tail -n1 | cut -d= -f2-
}
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
#-------------------------------------------------------------------------------
root=`Environment_GetVariable "SHELLSCRIPT_EXAMPLE_ROOT"`
envFile=`Path_Combine "$root" ".env"`
File_WriteAllText "$envFile" "API_TOKEN=abc123"
token=`DotEnv_Get "$envFile" "API_TOKEN"`
echo "$token"
Assert_Equals "abc123" "$token"
echo "EXAMPLE_OK:DotEnv"
