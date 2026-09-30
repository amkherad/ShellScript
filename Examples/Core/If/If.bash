#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
#-------------------------------------------------------------------------------
score=85
grade=""
if [ $score -ge 90 ]
then
  grade="A"
elif [ $score -ge 80 ]
then
  grade="B"
elif [ $score -ge 70 ]
then
  grade="C"
else
  grade="F"
fi
echo "Score ${score} => grade ${grade}"
if [ $score -gt 0 ] && [ $score -le 100 ]
then
  echo "Score is in valid range."
fi
Assert_Equals "B" "$grade"
if [ $score -gt 0 ] && [ $score -le 100 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
