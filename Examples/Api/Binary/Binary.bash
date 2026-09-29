#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Binary_ToBase64() {
printf '%s' "$1" | base64 -w 0
}
function Binary_FromBase64() {
printf '%s' "$1" | base64 -d
}
#-------------------------------------------------------------------------------
encoded=`Binary_ToBase64 "hi"`
decoded=`Binary_FromBase64 "$encoded"`
echo "$decoded"
echo "EXAMPLE_OK:Binary"
