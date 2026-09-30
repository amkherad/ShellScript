#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
#-------------------------------------------------------------------------------
#! Integer Double
#\param $1 Integer - n
function Double() {
  echo $(($1 + $1))
}
#! Integer Triple
#\param $1 Integer - n
function Triple() {
  echo $((($1 + $1) + $1))
}
