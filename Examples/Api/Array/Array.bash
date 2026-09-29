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
#-------------------------------------------------------------------------------
values=(10 20 30)
length=${#values[@]}
index=`Array_IndexOf $values 20`
hasTwenty=`Array_Contains $values 20`
copy=(0 0 0)
for h_array_index in ${!values[@]}; do
copy[$h_array_index]=${values[$h_array_index]}
done
echo "length=${length}"
echo "index=${index}"
echo "hasTwenty=${hasTwenty}"
echo "copyLen=${#copy[@]}"
echo "EXAMPLE_OK:Array"
