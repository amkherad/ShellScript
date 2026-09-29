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
function Directory_Create() {
mkdir -p -- "$1"
}
LastFunctionCall=0
#-------------------------------------------------------------------------------
root=`Environment_GetVariable "SHELLSCRIPT_EXAMPLE_ROOT"`
sub=`Path_Combine "$root" "subdir"`
LastFunctionCall=`Directory_Create "$sub"`
if [ -d "$sub" ]; then
h_Directory_Exists_Result=1
else
h_Directory_Exists_Result=0
fi
exists=$h_Directory_Exists_Result
echo "exists=${exists}"
echo "EXAMPLE_OK:Directory"
