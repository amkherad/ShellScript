#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function OS_GetHostName() {
  hostname
}
function OS_GetKernelName() {
  uname -s
}
function OS_GetArchitecture() {
  uname -m
}
#-------------------------------------------------------------------------------
host=`OS_GetHostName`
kernel=`OS_GetKernelName`
arch=`OS_GetArchitecture`
if [ -z "$host" ]; then
  h_String_IsNullOrEmpty_Result=1
else
  h_String_IsNullOrEmpty_Result=0
fi
hostSet=$((! $h_String_IsNullOrEmpty_Result))
if [ -z "$kernel" ]; then
  h_String_IsNullOrEmpty_Result1=1
else
  h_String_IsNullOrEmpty_Result1=0
fi
kernelSet=$((! $h_String_IsNullOrEmpty_Result1))
if [ -z "$arch" ]; then
  h_String_IsNullOrEmpty_Result12=1
else
  h_String_IsNullOrEmpty_Result12=0
fi
archSet=$((! $h_String_IsNullOrEmpty_Result12))
echo "host=${host}"
echo "kernel=${kernel}"
echo "arch=${arch}"
if [ $hostSet -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
if [ $kernelSet -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
if [ $archSet -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
echo "EXAMPLE_OK:OS"
