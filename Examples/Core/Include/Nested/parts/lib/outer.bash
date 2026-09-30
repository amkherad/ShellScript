#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
#-------------------------------------------------------------------------------
#! String Prefix
function Prefix() {
  echo "Hello"
}
#! Integer Add
#\param $1 Integer - a
#\param $2 Integer - b
function Add() {
  echo $(($1 + $2))
}
#! String Greeting
function Greeting() {
  echo "Hello from nested include"
}
