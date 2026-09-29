#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
_SS_SCRIPT_ARGS=("$@")
function Cli_HasFlag() {
local f="$1" a
for a in "${_SS_SCRIPT_ARGS[@]}"; do [ "$a" = "$f" ] && echo 1 && return; done
echo 0
}
#-------------------------------------------------------------------------------
count=${#_SS_SCRIPT_ARGS[@]}
first=${_SS_SCRIPT_ARGS[0]}
verbose=`Cli_HasFlag "--verbose"`
echo "count=${count}"
echo "$first"
echo "verbose=${verbose}"
echo "EXAMPLE_OK:Cli"
