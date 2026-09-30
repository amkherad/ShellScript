#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
#-------------------------------------------------------------------------------
remaining=3
while
[ $remaining -gt 0 ]
do
  echo "remaining = ${remaining}"
  remaining=$(($remaining - 1))
done
found=0
probe=0
while
[ $found -eq 0 ] && [ $probe -lt 10 ]
do
  if [ $probe -eq 7 ]
  then
    found=1
    echo "found at 7"
  fi
  probe=$(($probe + 1))
done
Assert_Equals 0 $remaining
if [ $found -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
