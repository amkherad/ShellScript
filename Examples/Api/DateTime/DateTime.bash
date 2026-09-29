#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function DateTime_ToUnixTime() {
date +%s
}
#-------------------------------------------------------------------------------
unix=`DateTime_ToUnixTime`
unixPositive=$(($unix > 0))
echo "unixPositive=${unixPositive}"
echo "EXAMPLE_OK:DateTime"
