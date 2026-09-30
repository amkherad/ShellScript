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
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
#-------------------------------------------------------------------------------
#! Void greet
#\param $1 String - name
function greet() {
  echo "Hello, ${1}!" > /dev/tty
}
#! Integer add
#\param $1 Integer - a
#\param $2 Integer - b
function add() {
  echo $(($1 + $2))
}
#! Integer maxOf
#\param $1 Integer - a
#\param $2 Integer - b
function maxOf() {
  if [ $1 -ge $2 ]
  then
    echo $1
  fi
  echo $2
}
echo "Hello, ShellScript!"
echo "$((2 + 3))"
echo "`awk "BEGIN {print (10 + 0)}"`"
echo "`maxOf 4 9`"
Assert_Equals 5 $(($((2 + 3))))
Assert_Equals 10 `awk "BEGIN {print (10 + 0)}"`
Assert_Equals 9 `maxOf 4 9`
