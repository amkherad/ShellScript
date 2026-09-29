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
function File_WriteAllText() {
printf '%s' "$2" > "$1"
}
LastFunctionCall=0
function File_ReadAllText() {
cat -- "$1"
}
function File_GetLength() {
stat --format=%s -- "$1" 2>/dev/null || stat -f%z -- "$1"
}
#-------------------------------------------------------------------------------
root=`Environment_GetVariable "SHELLSCRIPT_EXAMPLE_ROOT"`
file=`Path_Combine "$root" "sample.txt"`
LastFunctionCall=`File_WriteAllText "$file" "hello"`
text=`File_ReadAllText "$file"`
len=`File_GetLength "$file"`
if [ -e "$file" ]; then
h_File_Exists_Result=1
else
h_File_Exists_Result=0
fi
exists=$h_File_Exists_Result
echo "$text"
echo "len=${len}"
echo "exists=${exists}"
echo "EXAMPLE_OK:File"
