#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
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
function Path_GetFileName() {
path=${1%/}
printf '%s' "${path##*/}"
}
#-------------------------------------------------------------------------------
temp=${TMPDIR:-/tmp}
combined=`Path_Combine "$temp" "child"`
name=`Path_GetFileName "$combined"`
if [[ "$temp" == /* ]]; then
h_Path_IsPathRooted_Result=1
else
h_Path_IsPathRooted_Result=0
fi
rooted=$h_Path_IsPathRooted_Result
echo "temp=${temp}"
echo "name=${name}"
echo "rooted=${rooted}"
echo "EXAMPLE_OK:Path"
