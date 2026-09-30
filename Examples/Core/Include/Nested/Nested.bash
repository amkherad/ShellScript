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
#! String Prefix
function Prefix() {
  echo "Hello"
}
#! Integer Add
#\param $1 Integer - a
#\param $2 Integer - b
function Add() {
  echo $(($1 + $2))
}
#! String Greeting
function Greeting() {
  echo "Hello from nested include"
}
echo "Hello from nested include"
echo "sum: $((4 + 5))"
h_str_arg="Hello from nested include"
Assert_Equals "Hello from nested include" "$h_str_arg"
Assert_Equals 9 $(($((4 + 5))))
