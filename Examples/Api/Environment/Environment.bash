#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Environment_SetVariable() {
export "$1=$2"
}
LastFunctionCall=0
function Environment_GetVariable() {
printf '%s' "${!1-}"
}
#-------------------------------------------------------------------------------
cwd=${PWD}
home=${HOME}
LastFunctionCall=`Environment_SetVariable "SS_EXAMPLE_MARKER" "set"`
marker=`Environment_GetVariable "SS_EXAMPLE_MARKER"`
if [ -z "$cwd" ]; then
h_String_IsNullOrEmpty_Result=1
else
h_String_IsNullOrEmpty_Result=0
fi
cwdEmpty=$h_String_IsNullOrEmpty_Result
echo "marker=${marker}"
echo "cwdEmpty=${cwdEmpty}"
if [ -z "$home" ]; then
h_String_IsNullOrEmpty_Result1=1
else
h_String_IsNullOrEmpty_Result1=0
fi
echo "homeSet=$((! $h_String_IsNullOrEmpty_Result1))"
echo "EXAMPLE_OK:Environment"
