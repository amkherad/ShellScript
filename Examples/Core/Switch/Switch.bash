#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
#-------------------------------------------------------------------------------
command="list"
case "$command" in
help)
echo "Show help."
;;
list)
echo "List items."
;;
quit)
echo "Goodbye."
;;
*)
echo "Unknown command: ${command}"
;;
esac
