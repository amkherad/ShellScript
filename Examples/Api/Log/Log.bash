#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Log_Info() {
printf '[INFO] %s\n' "$1" >&2
}
LastFunctionCall=0
#-------------------------------------------------------------------------------
LastFunctionCall=`Log_Info "EXAMPLE_OK:Log"`
