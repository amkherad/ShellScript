#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Unicode_GetLength() {
  python3 -c 'import sys; print(len(sys.argv[1]))' "$1"
}
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
#-------------------------------------------------------------------------------
len=`Unicode_GetLength "hi"`
echo "len=${len}"
Assert_Equals 2 $len
echo "EXAMPLE_OK:Unicode"
