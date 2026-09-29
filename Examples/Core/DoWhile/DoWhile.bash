#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
#-------------------------------------------------------------------------------
attempt=0
success=0
while :
do
attempt=$(($attempt + 1))
echo "attempt ${attempt}"
if [ $attempt -ge 3 ]
then
success=1
fi
if ! [ $success -eq 0 ]; then
break
fi
done
echo "done"
