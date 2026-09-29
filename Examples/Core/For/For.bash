#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
#-------------------------------------------------------------------------------
i=0
while
[ $i -lt 5 ]
do
echo "for i = ${i}"
((i++))
done
n=0
n=10
while
[ $n -gt 0 ]
do
echo "countdown ${n}"
((n--))
done
