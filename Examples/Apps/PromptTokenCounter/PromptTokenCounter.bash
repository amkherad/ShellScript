#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Platform_GetScriptDirectory() {
  local src="${BASH_SOURCE[1]}"
  if [ -z "$src" ]; then src="${BASH_SOURCE[0]}"; fi
  (cd "$(dirname "$src")" && pwd)
}
function Path_Combine() {
  if [[ -z $1 ]]; then
        printf '%s' "$2"
  elif [[ -z $2 ]]; then
        printf '%s' "$1"
  elif [[ $1 == */ ]]; then
        printf '%s%s' "$1" "${2#/}"
  else
        printf '%s/%s' "$1" "${2#/}"
  fi
}
function Path_Combine() {
  if [[ -z $1 ]]; then
        printf '%s' "$2"
  elif [[ -z $2 ]]; then
        printf '%s' "$1"
  elif [[ $1 == */ ]]; then
        printf '%s%s' "$1" "${2#/}"
  else
        printf '%s/%s' "$1" "${2#/}"
  fi
}
_SS_SCRIPT_ARGS=("$@")
function Cli_HasFlag() {
  local f="$1" a
  for a in "${_SS_SCRIPT_ARGS[@]}"; do [ "$a" = "$f" ] && echo 1 && return; done
  echo 0
}
function Cli_HasFlag() {
  local f="$1" a
  for a in "${_SS_SCRIPT_ARGS[@]}"; do [ "$a" = "$f" ] && echo 1 && return; done
  echo 0
}
function Cli_HasFlag() {
  local f="$1" a
  for a in "${_SS_SCRIPT_ARGS[@]}"; do [ "$a" = "$f" ] && echo 1 && return; done
  echo 0
}
function Cli_HasFlag() {
  local f="$1" a
  for a in "${_SS_SCRIPT_ARGS[@]}"; do [ "$a" = "$f" ] && echo 1 && return; done
  echo 0
}
function Cli_HasFlag() {
  local f="$1" a
  for a in "${_SS_SCRIPT_ARGS[@]}"; do [ "$a" = "$f" ] && echo 1 && return; done
  echo 0
}
function Cli_GetFlagValue() {
  local f="$1" a i
  for i in "${!_SS_SCRIPT_ARGS[@]}"; do
      a="${_SS_SCRIPT_ARGS[$i]}"
      case "$a" in "$f"=*) printf '%s' "${a#*=}"; return ;; "$f") printf '%s' "${_SS_SCRIPT_ARGS[$((i+1))]:-}"; return ;; esac
  done
  printf ''
}
function Cli_GetFlagValue() {
  local f="$1" a i
  for i in "${!_SS_SCRIPT_ARGS[@]}"; do
      a="${_SS_SCRIPT_ARGS[$i]}"
      case "$a" in "$f"=*) printf '%s' "${a#*=}"; return ;; "$f") printf '%s' "${_SS_SCRIPT_ARGS[$((i+1))]:-}"; return ;; esac
  done
  printf ''
}
function Cli_GetFlagValue() {
  local f="$1" a i
  for i in "${!_SS_SCRIPT_ARGS[@]}"; do
      a="${_SS_SCRIPT_ARGS[$i]}"
      case "$a" in "$f"=*) printf '%s' "${a#*=}"; return ;; "$f") printf '%s' "${_SS_SCRIPT_ARGS[$((i+1))]:-}"; return ;; esac
  done
  printf ''
}
function Cli_GetFlagValue() {
  local f="$1" a i
  for i in "${!_SS_SCRIPT_ARGS[@]}"; do
      a="${_SS_SCRIPT_ARGS[$i]}"
      case "$a" in "$f"=*) printf '%s' "${a#*=}"; return ;; "$f") printf '%s' "${_SS_SCRIPT_ARGS[$((i+1))]:-}"; return ;; esac
  done
  printf ''
}
function Cli_GetFlagValue() {
  local f="$1" a i
  for i in "${!_SS_SCRIPT_ARGS[@]}"; do
      a="${_SS_SCRIPT_ARGS[$i]}"
      case "$a" in "$f"=*) printf '%s' "${a#*=}"; return ;; "$f") printf '%s' "${_SS_SCRIPT_ARGS[$((i+1))]:-}"; return ;; esac
  done
  printf ''
}
function Path_Combine() {
  if [[ -z $1 ]]; then
        printf '%s' "$2"
  elif [[ -z $2 ]]; then
        printf '%s' "$1"
  elif [[ $1 == */ ]]; then
        printf '%s%s' "$1" "${2#/}"
  else
        printf '%s/%s' "$1" "${2#/}"
  fi
}
function File_WriteAllText() {
  printf '%s' "$2" > "$1"
}
function Path_Combine() {
  if [[ -z $1 ]]; then
        printf '%s' "$2"
  elif [[ -z $2 ]]; then
        printf '%s' "$1"
  elif [[ $1 == */ ]]; then
        printf '%s%s' "$1" "${2#/}"
  else
        printf '%s/%s' "$1" "${2#/}"
  fi
}
function Console_WriteError() {
  printf '%s\n' "$1" >&2
  if [ -n "${__SS_CONSOLE_ERROR_LOG:-}" ]; then printf '%s\n' "$1" >> "${__SS_CONSOLE_ERROR_LOG}"; fi
}
LastFunctionCall=0
function Text_Split() {
  IFS="$2" read -r -a LastFunctionCall <<< "$1"
}
function Process_RunAndCapture() {
  eval "$1"
}
function Path_Combine() {
  if [[ -z $1 ]]; then
        printf '%s' "$2"
  elif [[ -z $2 ]]; then
        printf '%s' "$1"
  elif [[ $1 == */ ]]; then
        printf '%s%s' "$1" "${2#/}"
  else
        printf '%s/%s' "$1" "${2#/}"
  fi
}
function File_WriteAllText() {
  printf '%s' "$2" > "$1"
}
function Path_Combine() {
  if [[ -z $1 ]]; then
        printf '%s' "$2"
  elif [[ -z $2 ]]; then
        printf '%s' "$1"
  elif [[ $1 == */ ]]; then
        printf '%s%s' "$1" "${2#/}"
  else
        printf '%s/%s' "$1" "${2#/}"
  fi
}
function Console_WriteError() {
  printf '%s\n' "$1" >&2
  if [ -n "${__SS_CONSOLE_ERROR_LOG:-}" ]; then printf '%s\n' "$1" >> "${__SS_CONSOLE_ERROR_LOG}"; fi
}
function Process_RunAndCapture() {
  eval "$1"
}
#-------------------------------------------------------------------------------
scriptDir=`Platform_GetScriptDirectory`
counterPy=`Path_Combine "$scriptDir" "count_tokens.py"`
defaultPrompt=`Path_Combine "$scriptDir" "sample_prompt.txt"`
platform="openai"
promptFile=""
inlineText=""
jsonOut=0
listPlatforms=0
showHelp=0
helpFlag=`Cli_HasFlag "--help"`
helpShort=`Cli_HasFlag "-h"`
listFlag=`Cli_HasFlag "--list-platforms"`
jsonFlag=`Cli_HasFlag "--json"`
compareFlag=`Cli_HasFlag "--compare"`
if [ $helpFlag -ne 0 ] || [ $helpShort -ne 0 ]
then
  showHelp=1
fi
if [ $listFlag -ne 0 ]
then
  listPlatforms=1
fi
if [ $jsonFlag -ne 0 ]
then
  jsonOut=1
fi
platformFlag=`Cli_GetFlagValue "--platform"`
if [ -n "$platformFlag" ]
then
  platform="$platformFlag"
fi
fileFlag=`Cli_GetFlagValue "--file"`
if [ -n "$fileFlag" ]
then
  promptFile="$fileFlag"
fi
textFlag=`Cli_GetFlagValue "--text"`
if [ -n "$textFlag" ]
then
  inlineText="$textFlag"
fi
if [ $showHelp -ne 0 ]
then
  echo "PromptTokenCounter — estimate prompt tokens per AI platform."
  echo ""
  echo "  --platform=NAME   openai (default), openai-o200k, anthropic, google,"
  echo "                    meta, mistral, xai, cohere, amazon"
  echo "  --file=PATH       UTF-8 prompt file (default: sample_prompt.txt beside script)"
  echo "  --text=TEXT       Prompt inline (written to a temp file for the counter)"
  echo "  --json            JSON output { platform, tokens, method, characters }"
  echo "  --list-platforms  Print supported platform ids"
  echo "  --compare         Count the same prompt on every supported platform"
  echo "  --help            This message"
  echo ""
  echo "Install optional 'tiktoken' (pip) for accurate OpenAI/xAI counts."
fi
if [ $listPlatforms -ne 0 ]
then
  echo "openai"
  echo "openai-o200k"
  echo "anthropic"
  echo "google"
  echo "meta"
  echo "mistral"
  echo "xai"
  echo "cohere"
  echo "amazon"
fi
if [ $showHelp -eq 0 ] && [ $listPlatforms -eq 0 ] && [ $compareFlag -ne 0 ]
then
  inputPath="$defaultPrompt"
  fileFlagCmp=`Cli_GetFlagValue "--file"`
  textFlagCmp=`Cli_GetFlagValue "--text"`
  if [ -n "$textFlagCmp" ]
  then
    inputPath=`Path_Combine "$scriptDir" ".prompt_token_counter_input.txt"`
    File_WriteAllText "$inputPath" "$textFlagCmp"
  else
    if [ -n "$fileFlagCmp" ]
    then
      inputPath="$fileFlagCmp"
      if [[ ! "$inputPath" == /* ]]
      then
        inputPath=`Path_Combine "$scriptDir" "$fileFlagCmp"`
      fi
    fi
  fi
  if [ ! -e "$inputPath" ]
  then
    h_str_arg="Prompt file not found: ${inputPath}"
    Console_WriteError "$h_str_arg"
  else
    echo "compare prompt=${inputPath}"
    Text_Split "openai openai-o200k anthropic google meta mistral xai cohere amazon" " "
    for h_array_index in ${!LastFunctionCall[@]}; do
      platforms[$h_array_index]="${LastFunctionCall[$h_array_index]}"
    done
    pCount=${#platforms[@]}
    pi=0
    while
    [ $pi -lt $pCount ]
    do
      p="${platforms[$pi]}"
      cmd="python3 "${counterPy}" --platform ${p} --file "${inputPath}""
      block=`Process_RunAndCapture "$cmd"`
      echo "--- ${p}"
      echo "$block"
      pi=$(($pi + 1))
    done
  fi
fi
if [ $showHelp -eq 0 ] && [ $listPlatforms -eq 0 ] && [ $compareFlag -eq 0 ]
then
  inputPath=""
  if [ -n "$inlineText" ]
  then
    inputPath=`Path_Combine "$scriptDir" ".prompt_token_counter_input.txt"`
    File_WriteAllText "$inputPath" "$inlineText"
  else
    if [ -n "$promptFile" ]
    then
      inputPath="$promptFile"
      if [[ ! "$inputPath" == /* ]]
      then
        inputPath=`Path_Combine "$scriptDir" "$promptFile"`
      fi
    else
      inputPath="$defaultPrompt"
    fi
  fi
  if [ ! -e "$inputPath" ]
  then
    h_str_arg="Prompt file not found: ${inputPath}"
    Console_WriteError "$h_str_arg"
  else
    pyArgs="python3 "${counterPy}" --platform ${platform} --file "${inputPath}""
    if [ $jsonOut -ne 0 ]
    then
      pyArgs="${pyArgs} --json"
    fi
    raw=`Process_RunAndCapture "$pyArgs"`
    if [ $jsonOut -ne 0 ]
    then
      echo "$raw"
    else
      echo "platform=${platform}"
      echo "$raw"
    fi
  fi
fi
