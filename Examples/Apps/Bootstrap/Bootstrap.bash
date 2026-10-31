#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Console_WriteLine() {
printf '%s\n' "$1"
}
function Console_WriteLine() {
printf '%s\n' "$1"
}
function Console_WriteLine() {
printf '%s\n' "$1"
}
function Console_WriteLine() {
printf '%s\n' "$1"
}
function Console_WriteLine() {
printf '%s\n' "$1"
}
function Console_WriteLine() {
printf '%s\n' "$1"
}
function Console_WriteLine() {
printf '%s\n' "$1"
}
function Console_WriteLine() {
printf '%s\n' "$1"
}
function Console_WriteLine() {
printf '%s\n' "$1"
}
function Console_WriteLine() {
printf '%s\n' "$1"
}
LastFunctionCall=0
function Text_Split() {
IFS="$2" read -r -a LastFunctionCall <<< "$1"
}
function Console_WriteLine() {
printf '%s\n' "$1"
}
function Platform_GetScriptDirectory() {
local src="${BASH_SOURCE[1]}"
if [ -z "$src" ]; then src="${BASH_SOURCE[0]}"; fi
(cd "$(dirname "$src")" && pwd)
}
function File_WriteAllText() {
printf '%s' "$2" > "$1"
}
function Process_RunAndCapture() {
eval "$1"
}
function File_WriteAllText() {
printf '%s' "$2" > "$1"
}
function File_ReadAllText() {
cat -- "$1"
}
function Text_Split() {
IFS="$2" read -r -a LastFunctionCall <<< "$1"
}
function Platform_GetScriptDirectory() {
local src="${BASH_SOURCE[1]}"
if [ -z "$src" ]; then src="${BASH_SOURCE[0]}"; fi
(cd "$(dirname "$src")" && pwd)
}
function Process_RunAndCapture() {
eval "$1"
}
function File_WriteAllText() {
printf '%s' "$2" > "$1"
}
function File_ReadAllText() {
cat -- "$1"
}
function Text_Split() {
IFS="$2" read -r -a LastFunctionCall <<< "$1"
}
function Platform_GetScriptDirectory() {
local src="${BASH_SOURCE[1]}"
if [ -z "$src" ]; then src="${BASH_SOURCE[0]}"; fi
(cd "$(dirname "$src")" && pwd)
}
function File_WriteAllText() {
printf '%s' "$2" > "$1"
}
function Process_RunAndCapture() {
eval "$1"
}
function Platform_GetScriptDirectory() {
local src="${BASH_SOURCE[1]}"
if [ -z "$src" ]; then src="${BASH_SOURCE[0]}"; fi
(cd "$(dirname "$src")" && pwd)
}
function File_ReadAllText() {
cat -- "$1"
}
function File_ReadAllText() {
cat -- "$1"
}
function File_WriteAllText() {
printf '%s' "$2" > "$1"
}
function File_ReadAllText() {
cat -- "$1"
}
#-------------------------------------------------------------------------------
#! String SpecKeywordList
function SpecKeywordList() {
echo "const void int long bool double float number string object any delegate if else for foreach while do loop return new include echo true false null nil switch case default class throw async await"
}
#! Void SpecPrintRoadmap
function SpecPrintRoadmap() {
Console_WriteLine "ShellScript Bootstrap — self-hosted compiler roadmap (see docs/language/specification.md)"
Console_WriteLine ""
Console_WriteLine "Phase 1 Lexer     : whitespace, // comments, identifiers, keywords, numbers, strings, punctuation"
Console_WriteLine "Phase 2 Parser    : echo statements; grow toward full grammar"
Console_WriteLine "Phase 3 Emit Bash : Unix-Bash backend; Windows targets later"
Console_WriteLine "Phase 4 Includes  : include resolution (spec section 8.4)"
Console_WriteLine "Phase 5 Full spec : types, functions, control flow, API, preprocessor"
Console_WriteLine ""
Console_WriteLine "Goal: replace the C# transpiler with ShellScript sources that emit platform shells."
Console_WriteLine "Interim: bootstrap_*.py reference phases; port each module into parts/*.shellscript."
}
#! Void SpecPrintKeywordCount
function SpecPrintKeywordCount() {
Text_Split "const void int long bool double float number string object any delegate if else for foreach while do loop return new include echo true false null nil switch case default class throw async await" " "
for h_array_index in ${!LastFunctionCall[@]}; do
kws[$h_array_index]="${LastFunctionCall[$h_array_index]}"
done
local n=${#kws[@]}
h_str_arg="spec-keywords=${n}"
Console_WriteLine "$h_str_arg"
}
#! String BsTokenPath
#\param $1 String - scriptDir
function BsTokenPath() {
echo "${1}/.bootstrap_tokens.txt"
}
#! Void BsWriteTokens
#\param $1 String - source
#\param $2 String - tokenOutPath
function BsWriteTokens() {
local scriptDir=`Platform_GetScriptDirectory`
local tmp="${scriptDir}/.bootstrap_lexer_input.txt"
File_WriteAllText "$tmp" "$1"
local py="${scriptDir}/bootstrap_tokenize.py"
local cmd="python3 '${py}' '${tmp}'"
local tokens=`Process_RunAndCapture "$cmd"`
File_WriteAllText "$2" "$tokens"
}
#! Integer BsCountTokensInFile
#\param $1 String - tokenPath
function BsCountTokensInFile() {
local text=`File_ReadAllText "$1"`
Text_Split "$text" "\n"
for h_array_index in ${!LastFunctionCall[@]}; do
lines[$h_array_index]="${LastFunctionCall[$h_array_index]}"
done
echo ${#lines[@]}
}
#! String BsIrPath
#\param $1 String - scriptDir
function BsIrPath() {
echo "${1}/.bootstrap_ir.txt"
}
#! Void BsWriteIr
#\param $1 String - tokenPath
#\param $2 String - irOutPath
function BsWriteIr() {
local scriptDir=`Platform_GetScriptDirectory`
local py="${scriptDir}/bootstrap_parse.py"
local cmd="python3 '${py}' '${1}'"
local ir=`Process_RunAndCapture "$cmd"`
File_WriteAllText "$2" "$ir"
}
#! Integer BsCountStatementsInFile
#\param $1 String - irPath
function BsCountStatementsInFile() {
local text=`File_ReadAllText "$1"`
Text_Split "$text" "\n"
for h_array_index in ${!LastFunctionCall[@]}; do
lines[$h_array_index]="${LastFunctionCall[$h_array_index]}"
done
echo ${#lines[@]}
}
#! String BsEmitBashHeader
function BsEmitBashHeader() {
echo "#!/usr/bin/env bash\n# Generated by ShellScript Bootstrap\n"
}
#! String BsEmitBashFromIr
#\param $1 String - ir
function BsEmitBashFromIr() {
local scriptDir=`Platform_GetScriptDirectory`
local tmp="${scriptDir}/.bootstrap_ir_emit.txt"
File_WriteAllText "$tmp" "$1"
local py="${scriptDir}/bootstrap_emit.py"
local cmd="python3 '${py}' '${tmp}'"
echo "#!/usr/bin/env bash\n# Generated by ShellScript Bootstrap\n`Process_RunAndCapture "$cmd"`"
}
scriptDir=`Platform_GetScriptDirectory`
fixture="${scriptDir}/fixtures/minimal.shellscript"
tokenPath="${scriptDir}/.bootstrap_tokens.txt"
irPath="${scriptDir}/.bootstrap_ir.txt"
bashPath="${scriptDir}/.bootstrap_selftest.bash"
source=`File_ReadAllText "$fixture"`
BsWriteTokens "$source" "$tokenPath"
tokenCount=`BsCountTokensInFile "$tokenPath"`
if [ $tokenCount -gt 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
BsWriteIr "$tokenPath" "$irPath"
stmtCount=`BsCountStatementsInFile "$irPath"`
if [ $stmtCount -gt 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
irText=`File_ReadAllText "$irPath"`
bash=`BsEmitBashFromIr "$irText"`
File_WriteAllText "$bashPath" "$bash"
if [[ `File_ReadAllText "$bashPath"` == *"bootstrap-ok"* ]]; then
h_String_Contains_Result=1
else
h_String_Contains_Result=0
fi
if [ $h_String_Contains_Result ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
echo "tokens="${tokenCount}""
echo "statements="${stmtCount}""
echo "phase=lexer,parser,emit-bash"
echo "EXAMPLE_OK:Bootstrap"
