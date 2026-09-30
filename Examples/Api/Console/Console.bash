#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Console_WriteLine() {
  printf '%s\n' "$1"
}
function Console_EnableErrorLog() {
  if [ "$2" = "1" ]; then
      : >> "$1"
  else
      : > "$1"
  fi
  __SS_CONSOLE_ERROR_LOG="$1"
}
function Console_WriteError() {
  printf '%s\n' "$1" >&2
  if [ -n "${__SS_CONSOLE_ERROR_LOG:-}" ]; then printf '%s\n' "$1" >> "${__SS_CONSOLE_ERROR_LOG}"; fi
}
function Console_DisableErrorLog() {
  unset __SS_CONSOLE_ERROR_LOG
}
function File_ReadAllText() {
  cat -- "$1"
}
function Console_WriteLine() {
  printf '%s\n' "$1"
}
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
#-------------------------------------------------------------------------------
Console_WriteLine "console-line"
Console_EnableErrorLog "error.log" 0
Console_WriteError "mirror-me"
Console_DisableErrorLog
logged=`File_ReadAllText "error.log"`
Console_WriteLine "$logged"
Assert_Equals "mirror-me" "$logged"
echo "EXAMPLE_OK:Console"
