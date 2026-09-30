#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function StringBuilder_Create() {
  printf '%s' "$1"
}
function StringBuilder_ToString() {
  printf '%s' "$1"
}
function StringBuilder_GetLength() {
  echo ${#1}
}
function StringBuilder_ToString() {
  printf '%s' "$1"
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
function StringBuilder_ToString() {
  printf '%s' "$1"
}
#-------------------------------------------------------------------------------
sb=`StringBuilder_Create`
printf -v sb '%s%s' "$sb" "hello"
printf -v sb '%s%s' "$sb" " "
printf -v sb '%s%s\n' "$sb" "world"
printf -v sb '%s\n' "$sb"
printf -v sb '%s%s' "$sb" "done"
text=`StringBuilder_ToString "$sb"`
len=`StringBuilder_GetLength "$sb"`
echo "$text"
echo "len=${len}"
sb=""
printf -v sb '%s%s' "$sb" "cleared"
echo "`StringBuilder_ToString "$sb"`"
Assert_Equals 17 $len
Assert_Equals "cleared" `StringBuilder_ToString "$sb"`
echo "EXAMPLE_OK:StringBuilder"
