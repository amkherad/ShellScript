#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Regex_IsMatch() {
if printf '%s' "$1" | grep -Eq -- "$2"; then echo 1; else echo 0; fi
}
function Regex_Replace() {
printf '%s' "$1" | sed -E "s|$2|$3|g"
}
#-------------------------------------------------------------------------------
digits=`Regex_IsMatch "item42" "[0-9]+"`
cleaned=`Regex_Replace "a1b2c3" "[0-9]" ""`
echo "digits=${digits}"
echo "$cleaned"
echo "EXAMPLE_OK:Regex"
