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
  function Convert_Parse() {
    awk -v a="$1" 'BEGIN { print a+0 }'
  }
elif [ $h_bc_existence -ne 0 ]
then
  function Convert_Parse() {
    echo "scale=10; $1" | bc -l
  }
elif [ $h_python_existence -ne 0 ]
then
  function Convert_Parse() {
    python3 -c 'import sys,math; a=float(sys.argv[1]); print(float(a))' "$1"
  }
else
  function Convert_Parse() {
    awk -v a="$1" 'BEGIN { print a+0 }'
  }
fi

#-------------------------------------------------------------------------------
i=42
n=`Convert_Parse "3.5"`
b=1
s="${i}"
echo "i=${i}"
echo "$n"
echo "b=${b}"
echo "$s"
Assert_Equals 42 $i
Assert_Equals 3.5 $n
if [ $b -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
Assert_Equals "42" "$s"
echo "EXAMPLE_OK:Convert"
