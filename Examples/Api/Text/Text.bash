#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Text_NormalizeWhitespace() {
printf '%s' "$1" | tr -s '[:space:]' ' ' | sed 's/^ //;s/ $//'
}
function Regex_IsMatch() {
if printf '%s' "$1" | grep -Eq -- "$2"; then echo 1; else echo 0; fi
}
function Regex_Replace() {
printf '%s' "$1" | sed -E "s|$2|$3|g"
}
#-------------------------------------------------------------------------------
normalized=`Text_NormalizeWhitespace "  a   b  "`
match=`Regex_IsMatch "abc123" "[0-9]+"`
replaced=`Regex_Replace "a-b-c" "-" "_"`
echo "$normalized"
echo "match=${match}"
echo "$replaced"
echo "EXAMPLE_OK:Text"
