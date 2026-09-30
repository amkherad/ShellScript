#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Text_NormalizeWhitespace() {
  printf '%s' "$1" | tr -s '[:space:]' ' ' | sed 's/^ //;s/ $//'
}
function Regex_IsMatch() {
  if printf '%s' "$1" | grep -Eq -- "$2"; then echo 1; else echo 0; fi
}
function Regex_Replace() {
  printf '%s' "$1" | sed -E "s|$2|$3|g"
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
normalized=`Text_NormalizeWhitespace "  a   b  "`
match=`Regex_IsMatch "abc123" "[0-9]+"`
replaced=`Regex_Replace "a-b-c" "-" "_"`
echo "$normalized"
echo "match=${match}"
echo "$replaced"
Assert_Equals "a b" "$normalized"
if [ $match -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
Assert_Equals "a_b_c" "$replaced"
echo "EXAMPLE_OK:Text"
