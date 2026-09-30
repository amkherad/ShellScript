#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Array_IndexOf() {
  local n="$1" v="$2" i=0 len
  eval "len=\${#"$n"[@]}"
  while [ "$i" -lt "$len" ]; do
      eval "cur=\${"$n"[$i]}"
      if [ "$cur" = "$v" ]; then echo "$i"; return; fi
      i=$((i+1))
  done
  echo -1
}
function Array_Contains() {
  local idx
  idx=$(Array_IndexOf "$1" "$2")
  if [ "$idx" -ge 0 ]; then echo 1; else echo 0; fi
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
values=(10 20 30)
length=${#values[@]}
index=`Array_IndexOf $values 20`
hasTwenty=`Array_Contains $values 20`
copy=(0 0 0)
for h_array_index in ${!values[@]}; do
  copy[$h_array_index]=${values[$h_array_index]}
done
copyLen=${#copy[@]}
echo "length=${length}"
echo "index=${index}"
echo "hasTwenty=${hasTwenty}"
echo "copyLen=${copyLen}"
Assert_Equals 3 $length
Assert_Equals 3 $copyLen
echo "EXAMPLE_OK:Array"
