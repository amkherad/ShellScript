#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
#-------------------------------------------------------------------------------
attempt=0
success=0
while :
do
  attempt=$(($attempt + 1))
  echo "attempt ${attempt}"
  if [ $attempt -ge 3 ]
  then
    success=1
  fi
  if ! [ $success -eq 0 ]; then
    break
  fi
done
echo "done"
if [ $success -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
Assert_Equals 3 $attempt
