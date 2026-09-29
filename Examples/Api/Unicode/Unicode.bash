#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Unicode_GetLength() {
python3 -c 'import sys; print(len(sys.argv[1]))' "$1"
}
#-------------------------------------------------------------------------------
len=`Unicode_GetLength "hi"`
echo "len=${len}"
echo "EXAMPLE_OK:Unicode"
