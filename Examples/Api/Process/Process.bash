#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Process_RunAndCapture() {
eval "$1"
}
#-------------------------------------------------------------------------------
pid=$$
if kill -0 "$1" 2>/dev/null; then
h_Process_Exists_Result=1
else
h_Process_Exists_Result=0
fi
running=$h_Process_Exists_Result
captured=`Process_RunAndCapture "printf capture-ok"`
echo "running=${running}"
echo "$captured"
echo "EXAMPLE_OK:Process"
