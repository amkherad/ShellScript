#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
#-------------------------------------------------------------------------------
#! Void greet
#\param $1 String - name
function greet() {
echo "Hello, ${1}!" > /dev/tty
}
#! Integer add
#\param $1 Integer - a
#\param $2 Integer - b
function add() {
echo $(($1 + $2))
}
#! Integer maxOf
#\param $1 Integer - a
#\param $2 Integer - b
function maxOf() {
if [ $1 -ge $2 ]
then
echo $1
fi
echo $2
}
echo "Hello, ShellScript!"
echo $((2 + 3))
echo `awk "BEGIN {print (10 + 0)}"`
echo `maxOf 4 9`
