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
  function Math_Min() {
    awk -v a="$1" -v b="$2" 'BEGIN { print (a < b ? a : b) }'
  }
elif [ $h_bc_existence -ne 0 ]
then
  function Math_Min() {
    echo "scale=10; if ($1<$2) $1 else $2" | bc -l
  }
elif [ $h_python_existence -ne 0 ]
then
  function Math_Min() {
    python3 -c 'import sys,math; a=float(sys.argv[1]); b=float(sys.argv[2]); print(min(a,b))' "$1" "$2"
  }
else
  function Math_Min() {
    if [ "$1" -lt "$2" ]; then echo "$1"; else echo "$2"; fi
  }
fi

command -v awk > /dev/null
if [ $? -eq 0 ]
then
  h_awk_existence1=1
else
  h_awk_existence1=0
fi
command -v bc > /dev/null
if [ $? -eq 0 ]
then
  h_bc_existence1=1
else
  h_bc_existence1=0
fi
command -v python3 > /dev/null
if [ $? -eq 0 ]
then
  h_python_existence1=1
else
  h_python_existence1=0
fi
if [ $h_awk_existence1 -ne 0 ]
then
  function Math_Max() {
    awk -v a="$1" -v b="$2" 'BEGIN { print (a > b ? a : b) }'
  }
elif [ $h_bc_existence1 -ne 0 ]
then
  function Math_Max() {
    echo "scale=10; if ($1>$2) $1 else $2" | bc -l
  }
elif [ $h_python_existence1 -ne 0 ]
then
  function Math_Max() {
    python3 -c 'import sys,math; a=float(sys.argv[1]); b=float(sys.argv[2]); print(max(a,b))' "$1" "$2"
  }
else
  function Math_Max() {
    if [ "$1" -gt "$2" ]; then echo "$1"; else echo "$2"; fi
  }
fi

command -v awk > /dev/null
if [ $? -eq 0 ]
then
  h_awk_existence12=1
else
  h_awk_existence12=0
fi
command -v bc > /dev/null
if [ $? -eq 0 ]
then
  h_bc_existence12=1
else
  h_bc_existence12=0
fi
command -v python3 > /dev/null
if [ $? -eq 0 ]
then
  h_python_existence12=1
else
  h_python_existence12=0
fi
if [ $h_awk_existence12 -ne 0 ]
then
  function Math_Floor() {
    awk -v a="$1" 'BEGIN { print int(a) }'
  }
elif [ $h_bc_existence12 -ne 0 ]
then
  function Math_Floor() {
    echo "scale=10; ($1)/1" | bc -l
  }
elif [ $h_python_existence12 -ne 0 ]
then
  function Math_Floor() {
    python3 -c 'import sys,math; a=float(sys.argv[1]); print(math.floor(a))' "$1"
  }
else
  function Math_Floor() {
    echo "$1" | awk '{print int($1)}'
  }
fi

command -v awk > /dev/null
if [ $? -eq 0 ]
then
  h_awk_existence123=1
else
  h_awk_existence123=0
fi
command -v bc > /dev/null
if [ $? -eq 0 ]
then
  h_bc_existence123=1
else
  h_bc_existence123=0
fi
command -v python3 > /dev/null
if [ $? -eq 0 ]
then
  h_python_existence123=1
else
  h_python_existence123=0
fi
if [ $h_awk_existence123 -ne 0 ]
then
  function Math_Round() {
    awk -v a="$1" 'BEGIN { print int(a + 0.5 * (a < 0 ? -1 : 1)) }'
  }
elif [ $h_bc_existence123 -ne 0 ]
then
  function Math_Round() {
    echo "scale=10; ($1+0.5)/1" | bc -l
  }
elif [ $h_python_existence123 -ne 0 ]
then
  function Math_Round() {
    python3 -c 'import sys,math; a=float(sys.argv[1]); print(int(round(a)))' "$1"
  }
else
  function Math_Round() {
    echo "$1" | awk '{printf "%.0f\n", $1}'
  }
fi

#-------------------------------------------------------------------------------
abs=7
min=`Math_Min 2 9`
max=`Math_Max 2 9`
floor=`Math_Floor 3.7`
round=`Math_Round 3.5`
echo "$abs"
echo "$min"
echo "$max"
echo "$floor"
echo "$round"
Assert_Equals 7 $abs
Assert_Equals 2 $min
Assert_Equals 9 $max
Assert_Equals 3 $floor
Assert_Equals 4 $round
echo "EXAMPLE_OK:Math"
