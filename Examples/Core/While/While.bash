#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
#-------------------------------------------------------------------------------
remaining=3
while
[ $remaining -gt 0 ]
do
echo "remaining = ${remaining}"
remaining=$(($remaining - 1))
done
found=0
probe=0
while
[ $found -eq 0 ] && [ $probe -lt 10 ]
do
if [ $probe -eq 7 ]
then
found=1
echo "found at 7"
fi
probe=$(($probe + 1))
done
