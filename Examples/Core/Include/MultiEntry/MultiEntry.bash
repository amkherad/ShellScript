#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
#-------------------------------------------------------------------------------
#! Integer FeetToMeters
#\param $1 Integer - feet
function FeetToMeters() {
  echo $(($1 * 3))
}
#! String FormatMeters
#\param $1 Integer - meters
function FormatMeters() {
  echo "length="${1}"m"
}
m=$((10 * 3))
echo "length="${m}"m"
Assert_Equals 30 $m
h_str_arg="length=${m}m"
Assert_Equals "length=30m" "$h_str_arg"
