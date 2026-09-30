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
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
#-------------------------------------------------------------------------------
#Resolve third-party utility backends once (keeps hot paths branch-free).
command -v awk > /dev/null
if [ $? -eq 0 ]
then
  h_awk_existence=1
else
  h_awk_existence=0
fi
command -v bc > /dev/null
if [ $? -eq 0 ]
then
  h_bc_existence=1
else
  h_bc_existence=0
fi
command -v python3 > /dev/null
if [ $? -eq 0 ]
then
  h_python_existence=1
else
  h_python_existence=0
fi
if [ $h_awk_existence -ne 0 ]
then
  function Convert_ToInteger() {
    awk -v a="$1" 'BEGIN { printf "%.0f\n", a+0 }'
  }
elif [ $h_bc_existence -ne 0 ]
then
  function Convert_ToInteger() {
    echo "scale=10; ($1)/1" | bc -l
  }
elif [ $h_python_existence -ne 0 ]
then
  function Convert_ToInteger() {
    python3 -c 'import sys,math; a=float(sys.argv[1]); print(int(float(a)))' "$1"
  }
else
  function Convert_ToInteger() {
    echo $(( ${1%.*} ))
  }
fi

#-------------------------------------------------------------------------------
a=17
b=5
echo "$(($a + $b))"
echo "$(($a - $b))"
echo "$(($a * $b))"
echo "$(($a / $b))"
echo "$(($a % $b))"
echo "$(($a == $b))"
echo "$(($a != $b))"
echo "$(($a < $b))"
echo "$(($a >= $b))"
echo "0"
echo "1"
echo "1"
fromInt=$a
truncated=`Convert_ToInteger $fromInt`
label=""
if [ $a -gt 10 ]
then
  label="large"
else
  label="small"
fi
echo "a is ${label}"
echo "--------------------------------------------------------------------------------"
Assert_Equals 22 $(($(($a + $b))))
Assert_Equals 12 $(($(($a - $b))))
Assert_Equals "large" "$label"
