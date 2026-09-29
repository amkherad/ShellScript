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
echo "host=${host}"
echo "kernel=${kernel}"
echo "arch=${arch}"
echo "EXAMPLE_OK:OS"
