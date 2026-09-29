#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Environment_GetVariable() {
printf '%s' "${!1-}"
}
function Path_Combine() {
if [[ -z $1 ]]; then
    printf '%s' "$2"
elif [[ -z $2 ]]; then
    printf '%s' "$1"
elif [[ $1 == */ ]]; then
    printf '%s%s' "$1" "${2#/}"
else
    printf '%s/%s' "$1" "${2#/}"
fi
}
function Ini_SetValue() {
python3 -c 'import sys,re,pathlib; p,sec,key,val=sys.argv[1:5]; t=pathlib.Path(p); lines=t.read_text().splitlines() if t.exists() else []; out=[]; in_s=False; found=False; sec_h="["+sec+"]";
for line in lines:
  if line.strip().startswith("[") and line.strip().endswith("]"):
    if in_s and not found: out.append(key+"="+val); found=True
    in_s=line.strip()==sec_h; out.append(line); continue
  if in_s and line.split("=",1)[0].strip()==key: out.append(key+"="+val); found=True; continue
  out.append(line)
if not any(l.strip()==sec_h for l in out): out.append(sec_h)
if not found: out.append(key+"="+val)
t.write_text("\n".join(out)+"\n")' "$1" "$2" "$3" "$4"
}
LastFunctionCall=0
function Ini_GetValue() {
awk -F= -v section="[$2]" -v key="$3" '
  $0 ~ /^[[:space:]]*#/ { next }
  $0 ~ /^[[:space:]]*\[/ { in_section = ($0 == section); next }
  in_section && $1 == key { sub(/^[^=]*=/, ""); print; exit }
' "$1"
}
#-------------------------------------------------------------------------------
root=`Environment_GetVariable "SHELLSCRIPT_EXAMPLE_ROOT"`
ini=`Path_Combine "$root" "app.ini"`
LastFunctionCall=`Ini_SetValue "$ini" "app" "key" "value"`
value=`Ini_GetValue "$ini" "app" "key"`
echo "$value"
echo "EXAMPLE_OK:Ini"
