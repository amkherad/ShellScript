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
Assert_Equals "ShellScript API" "$trimmed"
Assert_Equals "abc" "$lower"
Assert_Equals "Script" "$sub"
Assert_Equals "a" "$part"
if [ $same -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
Assert_Equals "one_two" "$replaced"
Assert_Equals 1 $at
Assert_Equals "a-b-c" "$joined"
h_str_arg="${parts[1]}"
Assert_Equals "y" "$h_str_arg"
Assert_Equals "***" "$stars"
Assert_Equals 3 $last
echo "EXAMPLE_OK:String"
