#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
#-------------------------------------------------------------------------------
#! Void printEntry
#\param $1 Integer - entry
function printEntry() {
  echo "$1" > /dev/tty
}
#! Void printFibonacci
#\param $1 Integer - max
function printFibonacci() {
  if [ $1 -lt 0 ]
  then
    echo "Maximum should be positive non-zero value." >&2
    return 1
  fi
  local x=1
  local y=1
  local sum=2
  echo "1" > /dev/tty
  echo "1" > /dev/tty
  while
  [ $sum -le $1 ]
  do
    echo "2"
    x=$y
    y=$sum
    sum=$(($x + $y))
  done
}
