#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function String_Trim() {
local s="$1"
s="${s#"${s%%[![:space:]]*}"}"
s="${s%"${s##*[![:space:]]}"}"
printf '%s' "$s"
}
function String_ToLower() {
printf '%s' "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"
}
function String_Substring() {
if [ -z "$3" ] || [ "$3" -lt 0 ]; then printf '%s' "${1:$2}"; else printf '%s' "${1:$2:$3}"; fi
}
function String_GetBefore() {
case "$1" in *"$2"*) printf '%s' "${1%%"$2"*}" ;; *) printf '%s' "$1" ;; esac
}
function String_Replace() {
printf '%s' "${1//$2/$3}"
}
function String_IndexOf() {
local hay="$1" needle="$2" prefix
prefix=${hay%%"$needle"*}
if [ "$prefix" = "$hay" ]; then echo -1; else echo ${#prefix}; fi
}
function String_Join() {
local n="$1" sep="$2" first=1 out=""
eval 'for part in "${'"$n"'[@]}"; do if [ $first -eq 1 ]; then out="$part"; first=0; else out="${out}${sep}${part}"; fi; done'
printf '%s' "$out"
}
LastFunctionCall=0
function String_Split() {
IFS="$2" read -r -a LastFunctionCall <<< "$1"
}
function String_Repeat() {
local i=0 out=""
while [ $i -lt $2 ]; do out="${out}$1"; i=$((i + 1)); done
printf '%s' "$out"
}
function String_LastIndexOf() {
local hay="$1" needle="$2" suffix
suffix=${hay##*"$needle"}
if [ "$suffix" = "$hay" ]; then echo -1; else echo $((${#hay} - ${#suffix} - ${#needle})); fi
}
#-------------------------------------------------------------------------------
sample="  ShellScript API  "
trimmed=`String_Trim "$sample"`
lower=`String_ToLower "AbC"`
sub=`String_Substring "ShellScript" 5`
part=`String_GetBefore "a=b=c" "="`
if [[ "x" == "x" ]]; then
h_String_Equals_Result=1
else
h_String_Equals_Result=0
fi
same=$h_String_Equals_Result
replaced=`String_Replace "one-two" "-" "_"`
at=`String_IndexOf "abc" "b"`
h_array_helper[0]="a"
h_array_helper[1]="b"
h_array_helper[2]="c"
joined=`String_Join "h_array_helper" "-"`
String_Split "x:y:z" ":"
for h_array_index in ${!LastFunctionCall[@]}; do
parts[$h_array_index]="${LastFunctionCall[$h_array_index]}"
done
stars=`String_Repeat "*" 3`
last=`String_LastIndexOf "ababa" "ba"`
echo "$trimmed"
echo "$lower"
echo "$sub"
echo "$part"
echo "same=${same}"
echo "$replaced"
echo "at=${at}"
echo "$joined"
echo "${parts[1]}"
echo "$stars"
echo "last=${last}"
echo "EXAMPLE_OK:String"
