#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
#-------------------------------------------------------------------------------
i=0
while
[ $i -lt 5 ]
do
  echo "for i = ${i}"
  ((i++))
done
n=0
n=10
while
[ $n -gt 0 ]
do
  echo "countdown ${n}"
  ((n--))
done
Assert_Equals 0 $n
