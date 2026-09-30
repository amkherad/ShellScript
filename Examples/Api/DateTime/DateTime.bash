#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function DateTime_ToUnixTime() {
  date +%s
}
#-------------------------------------------------------------------------------
unix=`DateTime_ToUnixTime`
unixPositive=$(($unix > 0))
echo "unixPositive=${unixPositive}"
if [ $unixPositive -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
echo "EXAMPLE_OK:DateTime"
