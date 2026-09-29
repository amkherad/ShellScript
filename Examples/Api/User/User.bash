#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
#-------------------------------------------------------------------------------
name=${USER:-$(id -un)}
if [ $(id -u) -eq 0 ]; then
h_User_IsSuperUser_Result=1
else
h_User_IsSuperUser_Result=0
fi
elevated=$h_User_IsSuperUser_Result
if [ -z "$name" ]; then
h_String_IsNullOrEmpty_Result=1
else
h_String_IsNullOrEmpty_Result=0
fi
echo "nameSet=$((! $h_String_IsNullOrEmpty_Result))"
echo "elevated=${elevated}"
echo "EXAMPLE_OK:User"
