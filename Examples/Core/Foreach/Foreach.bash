#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
#-------------------------------------------------------------------------------
colors=("red" "green" "blue")
for color in "${colors[@]}"
do
  echo "$color"
done
values=(2 4 6 8)
sum=0
for value in "${values[@]}"
do
  sum=$(($sum + $value))
done
echo "sum = ${sum}"
Assert_Equals 20 $sum
