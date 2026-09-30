#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
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
#-------------------------------------------------------------------------------
digits=`Regex_IsMatch "item42" "[0-9]+"`
cleaned=`Regex_Replace "a1b2c3" "[0-9]" ""`
echo "digits=${digits}"
echo "$cleaned"
if [ $digits -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
Assert_Equals "abc" "$cleaned"
echo "EXAMPLE_OK:Regex"
