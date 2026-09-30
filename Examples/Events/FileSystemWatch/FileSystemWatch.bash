#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Path_GetDirectoryName() {
  if [[ $1 == / ]]; then
        printf '/'
  else
        path=${1%/}
        if [[ $path == */* ]]; then
              directory=${path%/*}
              printf '%s' "${directory:-/}"
        else
              printf '.'
        fi
  fi
}
#-------------------------------------------------------------------------------
file="/tmp/list.txt"
dir=`Path_GetDirectoryName "$file"`
if [ -z "$dir" ]; then
  h_String_IsNullOrEmpty_Result=1
else
  h_String_IsNullOrEmpty_Result=0
fi
dirSet=$((! $h_String_IsNullOrEmpty_Result))
if [ $dirSet -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
while
[ ! -e "$file" ]
do
  $((`inotifywait -qqt 2 -e create -e moved_to "${dir}"`))
done
echo "file appeared"
