#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Log_Info() {
  printf '[INFO] %s\n' "$1" >&2
}
#-------------------------------------------------------------------------------
if [ 1 -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
Log_Info "EXAMPLE_OK:Log"
