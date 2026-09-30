#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Thread_Sleep() {
  sleep $(awk -v ms="$1" 'BEGIN{printf "%.3f", ms/1000}')
}
#-------------------------------------------------------------------------------
Thread_Sleep 1
if [ 1 -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
echo "EXAMPLE_OK:Thread"
