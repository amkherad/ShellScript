#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Net_ResolveHost() {
  getent hosts "$1" | awk '{print $1; exit}'
}
#-------------------------------------------------------------------------------
loopback=`Net_ResolveHost "localhost"`
if ping -c 1 -W 1 -- "127.0.0.1" >/dev/null 2>&1; then
  h_Net_Ping_Result=1
else
  h_Net_Ping_Result=0
fi
alive=$h_Net_Ping_Result
echo "loopback=${loopback}"
echo "alive=${alive}"
if [ -z "$loopback" ]; then
  h_String_IsNullOrEmpty_Result=1
else
  h_String_IsNullOrEmpty_Result=0
fi
loopbackSet=$((! $h_String_IsNullOrEmpty_Result))
if [ $loopbackSet -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
echo "EXAMPLE_OK:Net"
