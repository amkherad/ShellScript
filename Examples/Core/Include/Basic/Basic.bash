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
#! Integer Double
#\param $1 Integer - n
function Double() {
  echo $(($1 + $1))
}
#! Integer Triple
#\param $1 Integer - n
function Triple() {
  echo $((($1 + $1) + $1))
}
echo "double 3 = $((3 + 3))"
echo "triple 2 = $(((2 + 2) + 2))"
Assert_Equals 6 $(($((3 + 3))))
Assert_Equals 6 $(($(((2 + 2) + 2))))
