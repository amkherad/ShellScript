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
nameSet=$((! $h_String_IsNullOrEmpty_Result))
echo "nameSet=${nameSet}"
echo "elevated=${elevated}"
if [ $nameSet -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
echo "EXAMPLE_OK:User"
