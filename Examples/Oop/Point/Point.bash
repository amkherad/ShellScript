#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
#-------------------------------------------------------------------------------
#! Void Point__new
#\param $2 Integer - ax
#\param $3 Integer - ay
function Point__new() {
local -n self=$1
self[x]=$2
self[y]=$3
}
declare -A p
Point__new p 3 4
echo 7
