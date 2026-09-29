#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Console_ReadText() {
IFS= read -r -p "$1" line; printf '%s' "$line"
}
function Console_WriteError() {
printf '%s\n' "$1" >&2
}
LastFunctionCall=0
#-------------------------------------------------------------------------------
#Resolve third-party utility backends once (keeps hot paths branch-free).
command -v awk > /dev/null
if [ $? -eq 0 ]
then
h_awk_existence=1
else
h_awk_existence=0
fi
command -v bc > /dev/null
if [ $? -eq 0 ]
then
h_bc_existence=1
else
h_bc_existence=0
fi
command -v python3 > /dev/null
if [ $? -eq 0 ]
then
h_python_existence=1
else
h_python_existence=0
fi
if [ $h_awk_existence -ne 0 ]
then
function Convert_ToInteger() {
awk -v a="$1" 'BEGIN { printf "%.0f\n", a+0 }'
}
elif [ $h_bc_existence -ne 0 ]
then
function Convert_ToInteger() {
echo "scale=10; ($1)/1" | bc -l
}
elif [ $h_python_existence -ne 0 ]
then
function Convert_ToInteger() {
python3 -c 'import sys,math; a=float(sys.argv[1]); print(int(float(a)))' "$1"
}
else
function Convert_ToInteger() {
echo $(( ${1%.*} ))
}
fi

#-------------------------------------------------------------------------------
CReset=$'\033[0m'
CBold=$'\033[1m'
CRed=$'\033[31m'
CGreen=$'\033[32m'
CYellow=$'\033[33m'
CBlue=$'\033[34m'
CCyan=$'\033[36m'
choice=0
while
[ $choice -ne 3 ]
do
echo "----------------------------------------"
echo "${CBold}${CCyan}ShellScript Menu${CReset}"
echo "----------------------------------------"
echo ""
echo "  ${CBlue}1${CReset} - Greet"
echo "  ${CBlue}2${CReset} - Show environment"
echo "  ${CBlue}3${CReset} - Exit"
echo ""
choicePrompt="${CYellow}>${CReset} Choice: "
line=`Console_ReadText "$choicePrompt"`
choice=`Convert_ToInteger "$line"`
if [ $choice -eq 1 ]
then
echo "${CGreen}[OK] Hello from ShellScript!${CReset}"
echo "  Tip: use echo for ANSI colors in transpiled bash."
fi
if [ $choice -eq 2 ]
then
echo "${CYellow}[!!] Paths come from the transpiled shell.${CReset}"
echo "  See Examples/Api/Environment for HOME and env vars."
echo "  shell: bash"
fi
if [ $choice -eq 3 ]
then
echo "${CBold}Goodbye.${CReset}"
fi
if [ $choice -ne 1 ] && [ $choice -ne 2 ] && [ $choice -ne 3 ]
then
LastFunctionCall=`Console_WriteError ""${CRed}[ERR] Invalid choice. Enter 1, 2, or 3.${CReset}""`
choice=0
fi
echo ""
done
echo "${CGreen}[OK] Session ended.${CReset}"
