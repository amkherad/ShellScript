#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
#-------------------------------------------------------------------------------
colors=("red" "green" "blue")
for color in "${colors[@]}"
do
echo "$color"
done
values=(2 4 6 8)
sum=0
for value in "${values[@]}"
do
sum=$(($sum + $value))
done
echo "sum = ${sum}"
