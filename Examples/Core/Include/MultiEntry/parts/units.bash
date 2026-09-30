#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
#-------------------------------------------------------------------------------
#! Integer FeetToMeters
#\param $1 Integer - feet
function FeetToMeters() {
  echo $(($1 * 3))
}
