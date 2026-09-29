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
function DotEnv_Get() {
grep -E "^$2=" "$1" | tail -n1 | cut -d= -f2-
}
#-------------------------------------------------------------------------------
root=`Environment_GetVariable "SHELLSCRIPT_EXAMPLE_ROOT"`
envFile=`Path_Combine "$root" ".env"`
LastFunctionCall=`File_WriteAllText "$envFile" "API_TOKEN=abc123"`
token=`DotEnv_Get "$envFile" "API_TOKEN"`
echo "$token"
echo "EXAMPLE_OK:DotEnv"
