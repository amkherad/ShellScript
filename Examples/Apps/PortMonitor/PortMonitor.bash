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
function Console_WriteLine() {
printf '%s\n' "$1"
}
function Console_Clear() {
printf '\033[2J\033[H'
}
function Console_EnterInteractiveInputMode() {
if [ -t 0 ] && [ -z "${__SS_CONSOLE_STTY_SAVED:-}" ]; then
  __SS_CONSOLE_STTY_SAVED=$(stty -g 2>/dev/null) || return 0
  stty -echo -icanon min 0 time 0 2>/dev/null || stty -echo -icanon 2>/dev/null
  trap 'Console_ExitInteractiveInputMode' EXIT HUP
  trap 'Console_ExitInteractiveInputMode; __SS_CONSOLE_INTERRUPT=1' INT TERM
fi
}
function Console_ConsumeInterruptRequest() {
if [ "${__SS_CONSOLE_INTERRUPT:-0}" = "1" ]; then
  __SS_CONSOLE_INTERRUPT=0
  echo 1
else
  echo 0
fi
}
function Console_ReadTerminalInputTimeout() {
local ms="$1"
local t
t=$(awk -v ms="$ms" 'BEGIN{printf "%.3f", ms/1000}')
local key=""
if [ "$2" = "1" ]; then
  if ! IFS= read -r -n 1 -t "$t" key; then printf ''; return 0; fi
else
  if ! IFS= read -r -s -n 1 -t "$t" key; then printf ''; return 0; fi
fi
if [ "$key" != $'\033' ]; then
  printf '%s' "$key"
  return 0
fi
local seq=""
local c=""
local end=""
local i=0
while [ "$i" -lt 48 ]; do
  if ! IFS= read -r -s -n 1 -t 0.05 c; then
    if [ -z "$seq" ]; then printf 'ESC'; return 0; fi
    break
  fi
  seq="${seq}${c}"
  case "$c" in M|m) end="$c"; break ;; '~') break ;; esac
  i=$((i + 1))
done
if [ "${seq:0:2}" = "[<" ]; then
  local inner="${seq:2}"
  end="${inner: -1}"
  inner="${inner%M}"
  inner="${inner%m}"
  local btn="" col="" row=""
  IFS=';' read -r btn col row <<< "$inner"
  if [ "$btn" -ge 64 ] 2>/dev/null; then
    printf 'MOUSE:%s:%s:%s' "$row" "$col" "$btn"
    return 0
  fi
  if [ "$end" = "M" ] && [ "$btn" -eq 0 ]; then
    printf 'MOUSE:%s:%s:%s' "$row" "$col" "$btn"
    return 0
  fi
fi
case "$seq" in
  "[A"|"OA") printf 'UP'; return 0 ;;
  "[B"|"OB") printf 'DOWN'; return 0 ;;
  "[D"|"OD") printf 'LEFT'; return 0 ;;
  "[C"|"OC") printf 'RIGHT'; return 0 ;;
esac
if [[ "$seq" =~ ^\[5.*~$ ]]; then printf 'PAGE_UP'; return 0; fi
if [[ "$seq" =~ ^\[6.*~$ ]]; then printf 'PAGE_DOWN'; return 0; fi
if [ -n "$seq" ]; then printf 'ESC'; return 0; fi
printf 'ESC'
}
function Console_ReadKeyTimeout() {
local ms="$1"
local t
t=$(awk -v ms="$ms" 'BEGIN{printf "%.3f", ms/1000}')
local key=""
if [ "$2" = "1" ]; then
  if ! IFS= read -r -n 1 -t "$t" key; then printf ''; return 0; fi
else
  if ! IFS= read -r -s -n 1 -t "$t" key; then printf ''; return 0; fi
fi
if [ "$key" != $'\033' ]; then
  printf '%s' "$key"
  return 0
fi
local seq=""
local c=""
local i=0
while [ "$i" -lt 12 ]; do
  if ! IFS= read -r -s -n 1 -t 0.05 c; then
    if [ -z "$seq" ]; then printf 'ESC'; return 0; fi
    break
  fi
  seq="${seq}${c}"
  case "$seq" in "[A"|"[B"|"[C"|"[D"|"OA"|"OB"|"OC"|"OD") break ;; esac
  case "$c" in '~') break ;; esac
  i=$((i + 1))
done
case "$seq" in
  "[A"|"OA") printf 'UP'; return 0 ;;
  "[B"|"OB") printf 'DOWN'; return 0 ;;
  "[D"|"OD") printf 'LEFT'; return 0 ;;
  "[C"|"OC") printf 'RIGHT'; return 0 ;;
esac
if [[ "$seq" =~ ^\[5.*~$ ]]; then printf 'PAGE_UP'; return 0; fi
if [[ "$seq" =~ ^\[6.*~$ ]]; then printf 'PAGE_DOWN'; return 0; fi
if [ -n "$seq" ]; then printf 'ESC'; return 0; fi
printf 'ESC'
}
LastFunctionCall=0
function String_Split() {
IFS="$2" read -r -a LastFunctionCall <<< "$1"
}
function Net_GetOpenSocketsOnInterface() {
if [ -z "$1" ]; then
  mapfile -t LastFunctionCall < <(command -v ss >/dev/null && ss -H -antup 2>/dev/null || netstat -antup 2>/dev/null | tail -n +3)
elif command -v ss >/dev/null; then
  mapfile -t LastFunctionCall < <(ss -H -antup dev "$1" 2>/dev/null)
else
  mapfile -t LastFunctionCall < <(netstat -antup -i 2>/dev/null | tail -n +3 | awk -v d="$1" '$0 ~ d {print}')
fi
if [ ${#LastFunctionCall[@]} -gt 0 ]; then
  mapfile -t LastFunctionCall < <(printf '%s\n' "${LastFunctionCall[@]}" | awk '{
    pid=0; if (match($0, /pid=[0-9]+/)) { pid=substr($0,RSTART+4,RLENGTH-4)+0 } else { n=split($0,f," "); last=f[n]; if (match(last,/^[0-9]+\//)) { sub(/\/.*/,"",last); pid=last+0 } }
    printf "%010d%s\n", pid, $0 }' | sort -n | cut -c11-)
fi
}
function String_ToLower() {
printf '%s' "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"
}
function String_ToLower() {
printf '%s' "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"
}
function Net_GetSocketProcessId() {
line="$1"
if [[ "$line" == *pid=* ]]; then
  rest=${line#*pid=}
  echo "${rest%%,*}"
else
  last=""
  for f in $line; do last=$f; done
  if [[ "$last" == */* ]]; then echo "${last%%/*}"; else echo 0; fi
fi
}
function File_AppendAllText() {
printf '%s' "$2" >> "$1"
}
function Process_GetName() {
ps -p "$1" -o comm= 2>/dev/null | tr -d ' '
}
function Process_RunAndCapture() {
eval "$1"
}
function String_Trim() {
local s="$1"
s="${s#"${s%%[![:space:]]*}"}"
s="${s%"${s##*[![:space:]]}"}"
printf '%s' "$s"
}
function Process_Kill() {
kill "$1"
}
function Net_GetNetworkInterfaces() {
if command -v ip >/dev/null; then
  mapfile -t LastFunctionCall < <(ip -o link show 2>/dev/null | awk -F': ' '{n=$2; sub(/@.*/,"",n); print n}')
else
  mapfile -t LastFunctionCall < <(ls /sys/class/net 2>/dev/null)
fi
}
function Console_EnterInteractiveInputMode() {
if [ -t 0 ] && [ -z "${__SS_CONSOLE_STTY_SAVED:-}" ]; then
  __SS_CONSOLE_STTY_SAVED=$(stty -g 2>/dev/null) || return 0
  stty -echo -icanon min 0 time 0 2>/dev/null || stty -echo -icanon 2>/dev/null
  trap 'Console_ExitInteractiveInputMode' EXIT HUP
  trap 'Console_ExitInteractiveInputMode; __SS_CONSOLE_INTERRUPT=1' INT TERM
fi
}
function String_Substring() {
if [ -z "$3" ] || [ "$3" -lt 0 ]; then printf '%s' "${1:$2}"; else printf '%s' "${1:$2:$3}"; fi
}
function Console_EnterInteractiveInputMode() {
if [ -t 0 ] && [ -z "${__SS_CONSOLE_STTY_SAVED:-}" ]; then
  __SS_CONSOLE_STTY_SAVED=$(stty -g 2>/dev/null) || return 0
  stty -echo -icanon min 0 time 0 2>/dev/null || stty -echo -icanon 2>/dev/null
  trap 'Console_ExitInteractiveInputMode' EXIT HUP
  trap 'Console_ExitInteractiveInputMode; __SS_CONSOLE_INTERRUPT=1' INT TERM
fi
}
function Console_GetWindowHeight() {
tput lines 2>/dev/null || echo 24
}
function Net_GetOpenSocketsOnInterface() {
if [ -z "$1" ]; then
  mapfile -t LastFunctionCall < <(command -v ss >/dev/null && ss -H -antup 2>/dev/null || netstat -antup 2>/dev/null | tail -n +3)
elif command -v ss >/dev/null; then
  mapfile -t LastFunctionCall < <(ss -H -antup dev "$1" 2>/dev/null)
else
  mapfile -t LastFunctionCall < <(netstat -antup -i 2>/dev/null | tail -n +3 | awk -v d="$1" '$0 ~ d {print}')
fi
if [ ${#LastFunctionCall[@]} -gt 0 ]; then
  mapfile -t LastFunctionCall < <(printf '%s\n' "${LastFunctionCall[@]}" | awk '{
    pid=0; if (match($0, /pid=[0-9]+/)) { pid=substr($0,RSTART+4,RLENGTH-4)+0 } else { n=split($0,f," "); last=f[n]; if (match(last,/^[0-9]+\//)) { sub(/\/.*/,"",last); pid=last+0 } }
    printf "%010d%s\n", pid, $0 }' | sort -n | cut -c11-)
fi
}
function String_ToLower() {
printf '%s' "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"
}
function String_ToLower() {
printf '%s' "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"
}
function Net_GetSocketProcessId() {
line="$1"
if [[ "$line" == *pid=* ]]; then
  rest=${line#*pid=}
  echo "${rest%%,*}"
else
  last=""
  for f in $line; do last=$f; done
  if [[ "$last" == */* ]]; then echo "${last%%/*}"; else echo 0; fi
fi
}
function Net_GetOpenSocketsOnInterface() {
if [ -z "$1" ]; then
  mapfile -t LastFunctionCall < <(command -v ss >/dev/null && ss -H -antup 2>/dev/null || netstat -antup 2>/dev/null | tail -n +3)
elif command -v ss >/dev/null; then
  mapfile -t LastFunctionCall < <(ss -H -antup dev "$1" 2>/dev/null)
else
  mapfile -t LastFunctionCall < <(netstat -antup -i 2>/dev/null | tail -n +3 | awk -v d="$1" '$0 ~ d {print}')
fi
if [ ${#LastFunctionCall[@]} -gt 0 ]; then
  mapfile -t LastFunctionCall < <(printf '%s\n' "${LastFunctionCall[@]}" | awk '{
    pid=0; if (match($0, /pid=[0-9]+/)) { pid=substr($0,RSTART+4,RLENGTH-4)+0 } else { n=split($0,f," "); last=f[n]; if (match(last,/^[0-9]+\//)) { sub(/\/.*/,"",last); pid=last+0 } }
    printf "%010d%s\n", pid, $0 }' | sort -n | cut -c11-)
fi
}
function String_ToLower() {
printf '%s' "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"
}
function String_ToLower() {
printf '%s' "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"
}
function Net_GetSocketProcessId() {
line="$1"
if [[ "$line" == *pid=* ]]; then
  rest=${line#*pid=}
  echo "${rest%%,*}"
else
  last=""
  for f in $line; do last=$f; done
  if [[ "$last" == */* ]]; then echo "${last%%/*}"; else echo 0; fi
fi
}
function Net_GetOpenSocketsOnInterface() {
if [ -z "$1" ]; then
  mapfile -t LastFunctionCall < <(command -v ss >/dev/null && ss -H -antup 2>/dev/null || netstat -antup 2>/dev/null | tail -n +3)
elif command -v ss >/dev/null; then
  mapfile -t LastFunctionCall < <(ss -H -antup dev "$1" 2>/dev/null)
else
  mapfile -t LastFunctionCall < <(netstat -antup -i 2>/dev/null | tail -n +3 | awk -v d="$1" '$0 ~ d {print}')
fi
if [ ${#LastFunctionCall[@]} -gt 0 ]; then
  mapfile -t LastFunctionCall < <(printf '%s\n' "${LastFunctionCall[@]}" | awk '{
    pid=0; if (match($0, /pid=[0-9]+/)) { pid=substr($0,RSTART+4,RLENGTH-4)+0 } else { n=split($0,f," "); last=f[n]; if (match(last,/^[0-9]+\//)) { sub(/\/.*/,"",last); pid=last+0 } }
    printf "%010d%s\n", pid, $0 }' | sort -n | cut -c11-)
fi
}
function String_ToLower() {
printf '%s' "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"
}
function String_ToLower() {
printf '%s' "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"
}
function Net_GetSocketProcessId() {
line="$1"
if [[ "$line" == *pid=* ]]; then
  rest=${line#*pid=}
  echo "${rest%%,*}"
else
  last=""
  for f in $line; do last=$f; done
  if [[ "$last" == */* ]]; then echo "${last%%/*}"; else echo 0; fi
fi
}
function Console_ExitInteractiveInputMode() {
if [ -n "${__SS_CONSOLE_STTY_SAVED:-}" ]; then
  stty "$__SS_CONSOLE_STTY_SAVED" 2>/dev/null || true
  unset __SS_CONSOLE_STTY_SAVED
fi
}
function Console_DisableMouseReporting() {
printf '\033[?1006l\033[?1000l'
}
function Console_ExitInteractiveInputMode() {
if [ -n "${__SS_CONSOLE_STTY_SAVED:-}" ]; then
  stty "$__SS_CONSOLE_STTY_SAVED" 2>/dev/null || true
  unset __SS_CONSOLE_STTY_SAVED
fi
}
function Console_DisableMouseReporting() {
printf '\033[?1006l\033[?1000l'
}
function Console_ExitInteractiveInputMode() {
if [ -n "${__SS_CONSOLE_STTY_SAVED:-}" ]; then
  stty "$__SS_CONSOLE_STTY_SAVED" 2>/dev/null || true
  unset __SS_CONSOLE_STTY_SAVED
fi
}
function Console_DisableMouseReporting() {
printf '\033[?1006l\033[?1000l'
}
function Net_GetNetworkInterfaces() {
if command -v ip >/dev/null; then
  mapfile -t LastFunctionCall < <(ip -o link show 2>/dev/null | awk -F': ' '{n=$2; sub(/@.*/,"",n); print n}')
else
  mapfile -t LastFunctionCall < <(ls /sys/class/net 2>/dev/null)
fi
}
function Net_GetNetworkInterfaces() {
if command -v ip >/dev/null; then
  mapfile -t LastFunctionCall < <(ip -o link show 2>/dev/null | awk -F': ' '{n=$2; sub(/@.*/,"",n); print n}')
else
  mapfile -t LastFunctionCall < <(ls /sys/class/net 2>/dev/null)
fi
}
function Console_DisableMouseReporting() {
printf '\033[?1006l\033[?1000l'
}
function Console_EnableMouseReporting() {
printf '\033[?1000h\033[?1006h'
}
function Net_GetOpenSocketsOnInterface() {
if [ -z "$1" ]; then
  mapfile -t LastFunctionCall < <(command -v ss >/dev/null && ss -H -antup 2>/dev/null || netstat -antup 2>/dev/null | tail -n +3)
elif command -v ss >/dev/null; then
  mapfile -t LastFunctionCall < <(ss -H -antup dev "$1" 2>/dev/null)
else
  mapfile -t LastFunctionCall < <(netstat -antup -i 2>/dev/null | tail -n +3 | awk -v d="$1" '$0 ~ d {print}')
fi
if [ ${#LastFunctionCall[@]} -gt 0 ]; then
  mapfile -t LastFunctionCall < <(printf '%s\n' "${LastFunctionCall[@]}" | awk '{
    pid=0; if (match($0, /pid=[0-9]+/)) { pid=substr($0,RSTART+4,RLENGTH-4)+0 } else { n=split($0,f," "); last=f[n]; if (match(last,/^[0-9]+\//)) { sub(/\/.*/,"",last); pid=last+0 } }
    printf "%010d%s\n", pid, $0 }' | sort -n | cut -c11-)
fi
}
function String_ToLower() {
printf '%s' "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"
}
function String_ToLower() {
printf '%s' "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"
}
function Net_GetSocketProcessId() {
line="$1"
if [[ "$line" == *pid=* ]]; then
  rest=${line#*pid=}
  echo "${rest%%,*}"
else
  last=""
  for f in $line; do last=$f; done
  if [[ "$last" == */* ]]; then echo "${last%%/*}"; else echo 0; fi
fi
}
function String_ToLower() {
printf '%s' "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"
}
function String_ToLower() {
printf '%s' "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"
}
function Console_GetWindowHeight() {
tput lines 2>/dev/null || echo 24
}
function Console_GetWindowWidth() {
tput cols 2>/dev/null || echo 80
}
function String_ToLower() {
printf '%s' "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"
}
function String_ToLower() {
printf '%s' "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"
}
function String_ToLower() {
printf '%s' "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"
}
function String_ToLower() {
printf '%s' "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"
}
function Net_GetSocketProcessId() {
line="$1"
if [[ "$line" == *pid=* ]]; then
  rest=${line#*pid=}
  echo "${rest%%,*}"
else
  last=""
  for f in $line; do last=$f; done
  if [[ "$last" == */* ]]; then echo "${last%%/*}"; else echo 0; fi
fi
}
function Console_BeginBatchWrite() {
if [ -z "${__SS_CONSOLE_BATCH_FILE:-}" ]; then __SS_CONSOLE_BATCH_FILE=$(mktemp); exec 9>&1; exec 1>>"$__SS_CONSOLE_BATCH_FILE"; fi
}
function Console_MoveCursorHome() {
printf '\033[H'
}
function OS_GetHostName() {
hostname
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
function String_ToLower() {
printf '%s' "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"
}
function String_ToLower() {
printf '%s' "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')"
}
function Net_GetSocketProcessId() {
line="$1"
if [[ "$line" == *pid=* ]]; then
  rest=${line#*pid=}
  echo "${rest%%,*}"
else
  last=""
  for f in $line; do last=$f; done
  if [[ "$last" == */* ]]; then echo "${last%%/*}"; else echo 0; fi
fi
}
function Process_GetName() {
ps -p "$1" -o comm= 2>/dev/null | tr -d ' '
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
function Net_GetNetworkInterfaces() {
if command -v ip >/dev/null; then
  mapfile -t LastFunctionCall < <(ip -o link show 2>/dev/null | awk -F': ' '{n=$2; sub(/@.*/,"",n); print n}')
else
  mapfile -t LastFunctionCall < <(ls /sys/class/net 2>/dev/null)
fi
}
function String_Repeat() {
local i=0 out=""
while [ $i -lt $2 ]; do out="${out}$1"; i=$((i + 1)); done
printf '%s' "$out"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function String_PadRight() {
local s="$1" w=$2 pad="$3"
while [ ${#s} -lt $w ]; do s="${s}${pad}"; done
printf '%s' "$s"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function String_PadRight() {
local s="$1" w=$2 pad="$3"
while [ ${#s} -lt $w ]; do s="${s}${pad}"; done
printf '%s' "$s"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function String_PadRight() {
local s="$1" w=$2 pad="$3"
while [ ${#s} -lt $w ]; do s="${s}${pad}"; done
printf '%s' "$s"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function String_PadRight() {
local s="$1" w=$2 pad="$3"
while [ ${#s} -lt $w ]; do s="${s}${pad}"; done
printf '%s' "$s"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function String_PadRight() {
local s="$1" w=$2 pad="$3"
while [ ${#s} -lt $w ]; do s="${s}${pad}"; done
printf '%s' "$s"
}
function String_PadRight() {
local s="$1" w=$2 pad="$3"
while [ ${#s} -lt $w ]; do s="${s}${pad}"; done
printf '%s' "$s"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function String_PadRight() {
local s="$1" w=$2 pad="$3"
while [ ${#s} -lt $w ]; do s="${s}${pad}"; done
printf '%s' "$s"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function String_PadRight() {
local s="$1" w=$2 pad="$3"
while [ ${#s} -lt $w ]; do s="${s}${pad}"; done
printf '%s' "$s"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function String_Repeat() {
local i=0 out=""
while [ $i -lt $2 ]; do out="${out}$1"; i=$((i + 1)); done
printf '%s' "$out"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function String_PadRight() {
local s="$1" w=$2 pad="$3"
while [ ${#s} -lt $w ]; do s="${s}${pad}"; done
printf '%s' "$s"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function String_PadRight() {
local s="$1" w=$2 pad="$3"
while [ ${#s} -lt $w ]; do s="${s}${pad}"; done
printf '%s' "$s"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function String_PadRight() {
local s="$1" w=$2 pad="$3"
while [ ${#s} -lt $w ]; do s="${s}${pad}"; done
printf '%s' "$s"
}
function String_PadRight() {
local s="$1" w=$2 pad="$3"
while [ ${#s} -lt $w ]; do s="${s}${pad}"; done
printf '%s' "$s"
}
function String_PadRight() {
local s="$1" w=$2 pad="$3"
while [ ${#s} -lt $w ]; do s="${s}${pad}"; done
printf '%s' "$s"
}
function String_PadRight() {
local s="$1" w=$2 pad="$3"
while [ ${#s} -lt $w ]; do s="${s}${pad}"; done
printf '%s' "$s"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function String_PadRight() {
local s="$1" w=$2 pad="$3"
while [ ${#s} -lt $w ]; do s="${s}${pad}"; done
printf '%s' "$s"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function Console_SetCursorPosition() {
printf '\033[%d;%dH' "$1" "$2"
}
function Console_Write() {
printf '%s' "$1"
}
function Console_WriteLine() {
printf '%s\n' "$1"
}
function Console_EndBatchWrite() {
if [ -n "${__SS_CONSOLE_BATCH_FILE:-}" ]; then exec 1>&9; exec 9>&-; cat "$__SS_CONSOLE_BATCH_FILE"; rm -f "$__SS_CONSOLE_BATCH_FILE"; unset __SS_CONSOLE_BATCH_FILE; fi
}
function Console_DisableMouseReporting() {
printf '\033[?1006l\033[?1000l'
}
function Console_ExitInteractiveInputMode() {
if [ -n "${__SS_CONSOLE_STTY_SAVED:-}" ]; then
  stty "$__SS_CONSOLE_STTY_SAVED" 2>/dev/null || true
  unset __SS_CONSOLE_STTY_SAVED
fi
}
function Console_DisableErrorLog() {
unset __SS_CONSOLE_ERROR_LOG
}
function Console_WriteLine() {
printf '%s\n' "$1"
}
function Console_Clear() {
printf '\033[2J\033[H'
}
function Console_WriteLine() {
printf '%s\n' "$1"
}
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
CRev=$'\033[7m'
CDim=$'\033[2m'
CRed=$'\033[31m'
CGreen=$'\033[32m'
CYellow=$'\033[33m'
CBlue=$'\033[34m'
CCyan=$'\033[36m'
CAltScreenOn=$'\033[?1049h'
CAltScreenOff=$'\033[?1049l'
CHideCursor=$'\033[?25l'
CShowCursor=$'\033[?25h'
CEraseBelow=$'\033[J'
running=1
filter=""
ifaceFilter=""
selected=0
scroll=0
anchorLine=""
firstPaint=1
idlePolls=0
dataRefreshPolls=8
filterDialogOpen=0
filterTab=0
draftText=""
draftIface=""
ifacePick=0
ifaceListScroll=0
restoreIfacePick=0
mouseEnabled=0
stdinTui=0
actionMenuOpen=0
actionPick=0
actionMenuCount=4
actionSocketLine=""
actionSocketPid=0
statusMessage=""
listFirstSocketRow=3
listPageStep=1
scriptDir=`Platform_GetScriptDirectory`
errorLogPath=`Path_Combine "$scriptDir" "PortMonitor.log"`
Console_EnableErrorLog "$errorLogPath" 0
if [ ! -t 1 ]
then
ttyError="${CRed}PortMonitor requires an interactive terminal (stdout is not a TTY).${CReset}"
Console_WriteError "$ttyError"
running=0
fi
if [ $running -ne 0 ]
then
h_str_arg="${CAltScreenOn}${CHideCursor}"
Console_WriteLine "$h_str_arg"
Console_Clear
if [ -t 0 ]
then
Console_EnterInteractiveInputMode
stdinTui=1
fi
fi
while
[ $running -ne 0 ]
do
if [ `Console_ConsumeInterruptRequest` -ne 0 ]
then
running=0
fi
repaint=0
if [ $firstPaint -ne 0 ]
then
repaint=1
firstPaint=0
fi
key=""
if [ $stdinTui -ne 0 ] || [ $filterDialogOpen -ne 0 ] || [ $actionMenuOpen -ne 0 ] || [ $mouseEnabled -ne 0 ]
then
key=`Console_ReadTerminalInputTimeout 250`
else
key=`Console_ReadKeyTimeout 250`
fi
if [ -n "$key" ]
then
idlePolls=0
repaint=1
if [ $mouseEnabled -ne 0 ] && [[ "$key" == "MOUSE:"* ]]
then
String_Split "$key" ":"
for h_array_index in ${!LastFunctionCall[@]}; do
mouseParts[$h_array_index]="${LastFunctionCall[$h_array_index]}"
done
if [ ${#mouseParts[@]} -ge 4 ]
then
h_str_arg="${mouseParts[1]}"
mouseRow=`Convert_ToInteger "$h_str_arg"`
mouseBtn="${mouseParts[3]}"
if [ $filterDialogOpen -eq 0 ] && [ $actionMenuOpen -eq 0 ]
then
if [[ "$mouseBtn" == "64" ]]
then
selected=$(($selected - 1))
anchorLine=""
else
if [[ "$mouseBtn" == "65" ]]
then
selected=$(($selected + 1))
anchorLine=""
else
if [[ "$mouseBtn" == "0" ]]
then
Net_GetOpenSocketsOnInterface "$ifaceFilter"
for h_array_index1 in ${!LastFunctionCall[@]}; do
sockMouse[$h_array_index1]="${LastFunctionCall[$h_array_index1]}"
done
hitIndex=-1
screenRow=$listFirstSocketRow
rowIndex=0
prevPid=-1
for row in "${sockMouse[@]}"
do
textOkMouse=0
if [ -z "$filter" ]
then
textOkMouse=1
else
if [[ `String_ToLower "$row"` == *`String_ToLower "$filter"`* ]]
then
textOkMouse=1
fi
fi
if [ $textOkMouse -ne 0 ]
then
rowPid=`Net_GetSocketProcessId "$row"`
if [ $rowIndex -ge $scroll ]
then
if [ $rowIndex -eq $scroll ] || [ $rowPid -ne $prevPid ]
then
if [ $mouseRow -eq $screenRow ]
then
hitIndex=$rowIndex
fi
screenRow=$(($screenRow + 1))
fi
if [ $mouseRow -eq $screenRow ]
then
hitIndex=$rowIndex
fi
screenRow=$(($screenRow + 1))
fi
prevPid=$rowPid
rowIndex=$(($rowIndex + 1))
fi
done
if [ $hitIndex -ge 0 ]
then
selected=$hitIndex
anchorLine=""
fi
fi
fi
fi
fi
fi
key=""
fi
if [ $actionMenuOpen -ne 0 ]
then
runSocketAction=0
if [[ "$key" == "q" ]]
then
actionMenuOpen=0
else
if [[ "$key" == $'
' ]]
then
runSocketAction=1
else
if [[ "$key" == $'
' ]]
then
runSocketAction=1
else
if [[ "$key" == "j" ]]
then
actionPick=$(($actionPick + 1))
fi
if [[ "$key" == "k" ]]
then
actionPick=$(($actionPick - 1))
fi
if [[ "$key" == "DOWN" ]]
then
actionPick=$(($actionPick + 1))
fi
if [[ "$key" == "UP" ]]
then
actionPick=$(($actionPick - 1))
fi
fi
fi
fi
if [ $actionPick -lt 0 ]
then
actionPick=0
fi
if [ $actionPick -ge $actionMenuCount ]
then
actionPick=$(($actionMenuCount - 1))
fi
if [ $runSocketAction -ne 0 ]
then
if [ $actionPick -eq 0 ]
then
h_str_arg="${actionSocketLine}
"
File_AppendAllText "$errorLogPath" "$h_str_arg"
statusMessage="Appended socket line to PortMonitor.log"
else
if [ $actionPick -eq 1 ]
then
procFilter=`Process_GetName $actionSocketPid`
if [ -z "$procFilter" ]
then
statusMessage="No process name for this socket"
else
filter="$procFilter"
anchorLine=""
selected=0
scroll=0
statusMessage="Text filter set to: ${procFilter}"
fi
else
if [ $actionPick -eq 2 ]
then
if [ $actionSocketPid -gt 0 ]
then
psCmd="ps -p "${actionSocketPid}" -o pid,ppid,user,cmd --no-headers 2>/dev/null"
psLine=`Process_RunAndCapture "$psCmd"`
psLine=`String_Trim "$psLine"`
if [ -z "$psLine" ]
then
statusMessage="No ps output for PID "${actionSocketPid}""
else
statusMessage="$psLine"
fi
else
statusMessage="No PID on this socket line"
fi
else
if [ $actionPick -eq 3 ]
then
selfPid=$$
if [ $actionSocketPid -le 0 ]
then
statusMessage="No PID to signal"
else
if [ $actionSocketPid -eq $selfPid ]
then
statusMessage="Refusing to signal PortMonitor itself"
else
if [ ! kill -0 "$1" 2>/dev/null ]
then
statusMessage="Process "${actionSocketPid}" not found"
else
Process_Kill $actionSocketPid
statusMessage="Sent SIGTERM to PID "${actionSocketPid}""
anchorLine=""
fi
fi
fi
fi
fi
fi
fi
actionMenuOpen=0
fi
else
if [ $filterDialogOpen -ne 0 ]
then
Net_GetNetworkInterfaces
for h_array_index in ${!LastFunctionCall[@]}; do
ifcList[$h_array_index]="${LastFunctionCall[$h_array_index]}"
done
maxPick=${#ifcList[@]}
if [ $ifacePick -lt 0 ]
then
ifacePick=0
fi
if [ $ifacePick -gt $maxPick ]
then
ifacePick=$maxPick
fi
if [ $ifacePick -eq 0 ]
then
draftIface=""
else
draftIface="${ifcList[$ifacePick - 1]}"
fi
applyFilterDialog=0
if [[ "$key" == "q" ]] || [[ "$key" == "ESC" ]]
then
filterDialogOpen=0
if [ $stdinTui -ne 0 ]
then
Console_EnterInteractiveInputMode
fi
else
if [[ "$key" == $'
' ]]
then
applyFilterDialog=1
else
if [[ "$key" == $'
' ]]
then
applyFilterDialog=1
else
if [[ "$key" == "LEFT" ]]
then
if [ $filterTab -gt 0 ]
then
filterTab=$(($filterTab - 1))
fi
else
if [[ "$key" == "RIGHT" ]]
then
if [ $filterTab -lt 1 ]
then
filterTab=$(($filterTab + 1))
fi
else
if [ $filterTab -eq 1 ]
then
if [[ "$key" == " " ]]
then
applyFilterDialog=1
else
if [[ "$key" == "j" ]]
then
ifacePick=$(($ifacePick + 1))
fi
if [[ "$key" == "k" ]]
then
ifacePick=$(($ifacePick - 1))
fi
if [[ "$key" == "DOWN" ]]
then
ifacePick=$(($ifacePick + 1))
fi
if [[ "$key" == "UP" ]]
then
ifacePick=$(($ifacePick - 1))
fi
fi
else
if [[ "$key" == $'\t' ]]
then
filterTab=$((1 - $filterTab))
else
if [[ "$key" == "1" ]]
then
filterTab=0
else
if [[ "$key" == "2" ]]
then
filterTab=1
else
if [[ "$key" == "h" ]]
then
filterTab=0
else
if [[ "$key" == "l" ]]
then
filterTab=1
else
if [[ "$key" == $'\b' ]]
then
dl=${#draftText}
if [ $dl -gt 0 ]
then
draftText=`String_Substring "$draftText" 0 $(($(($dl - 1))))`
fi
else
if [[ ! "$key" == "UP" ]]
then
if [[ ! "$key" == "DOWN" ]]
then
if [[ ! "$key" == "ESC" ]]
then
draftText="${draftText}${key}"
fi
fi
fi
fi
fi
fi
fi
fi
fi
fi
fi
fi
fi
fi
fi
if [ $applyFilterDialog -ne 0 ]
then
filter="$draftText"
ifaceFilter="$draftIface"
filterDialogOpen=0
if [ $stdinTui -ne 0 ]
then
Console_EnterInteractiveInputMode
fi
anchorLine=""
selected=0
scroll=0
else
if [ $filterDialogOpen -ne 0 ]
then
if [ $ifacePick -lt 0 ]
then
ifacePick=0
fi
if [ $ifacePick -gt $maxPick ]
then
ifacePick=$maxPick
fi
if [ $ifacePick -eq 0 ]
then
draftIface=""
else
draftIface="${ifcList[$ifacePick - 1]}"
fi
fi
fi
if [ $filterTab -eq 1 ]
then
dlgH=$((`Console_GetWindowHeight` - 6))
if [ $dlgH -gt 28 ]
then
dlgH=28
fi
if [ $dlgH -lt 14 ]
then
dlgH=14
fi
listSlots=$(($dlgH - 9))
if [ $listSlots -lt 3 ]
then
listSlots=3
fi
ifcCount=${#ifcList[@]}
if [ $ifacePick -gt 0 ]
then
pickInList=$(($ifacePick - 1))
if [ $pickInList -lt $ifaceListScroll ]
then
ifaceListScroll=$pickInList
fi
if [ $pickInList -ge $(($ifaceListScroll + $listSlots)) ]
then
ifaceListScroll=$((($pickInList - $listSlots) + 1))
fi
fi
if [ $ifaceListScroll -lt 0 ]
then
ifaceListScroll=0
fi
if [ $(($ifaceListScroll + $listSlots)) -gt $ifcCount ] && [ $ifcCount -gt $listSlots ]
then
ifaceListScroll=$(($ifcCount - $listSlots))
fi
fi
else
if [ -n "$statusMessage" ]
then
if [[ ! "$key" == "a" ]]
then
if [[ ! "$key" == $'
' ]]
then
if [[ ! "$key" == $'
' ]]
then
statusMessage=""
fi
fi
fi
fi
if [[ "$key" == "q" ]]
then
running=0
fi
if [[ "$key" == "a" ]]
then
Net_GetOpenSocketsOnInterface "$ifaceFilter"
for h_array_index in ${!LastFunctionCall[@]}; do
socketsForAction[$h_array_index]="${LastFunctionCall[$h_array_index]}"
done
actionSel=0
pickedLine=""
for row in "${socketsForAction[@]}"
do
textOkAction=0
if [ -z "$filter" ]
then
textOkAction=1
else
if [[ `String_ToLower "$row"` == *`String_ToLower "$filter"`* ]]
then
textOkAction=1
fi
fi
if [ $textOkAction -ne 0 ]
then
if [ $actionSel -eq $selected ]
then
pickedLine="$row"
fi
actionSel=$(($actionSel + 1))
fi
done
if [ -n "$pickedLine" ]
then
actionSocketLine="$pickedLine"
actionSocketPid=`Net_GetSocketProcessId "$actionSocketLine"`
actionPick=0
actionMenuOpen=1
fi
fi
if [[ "$key" == $'
' ]]
then
Net_GetOpenSocketsOnInterface "$ifaceFilter"
for h_array_index in ${!LastFunctionCall[@]}; do
socketsEnter[$h_array_index]="${LastFunctionCall[$h_array_index]}"
done
enterSel=0
enterLine=""
for row in "${socketsEnter[@]}"
do
textOkEnter=0
if [ -z "$filter" ]
then
textOkEnter=1
else
if [[ `String_ToLower "$row"` == *`String_ToLower "$filter"`* ]]
then
textOkEnter=1
fi
fi
if [ $textOkEnter -ne 0 ]
then
if [ $enterSel -eq $selected ]
then
enterLine="$row"
fi
enterSel=$(($enterSel + 1))
fi
done
if [ -n "$enterLine" ]
then
actionSocketLine="$enterLine"
actionSocketPid=`Net_GetSocketProcessId "$actionSocketLine"`
actionPick=0
actionMenuOpen=1
fi
fi
if [[ "$key" == $'
' ]]
then
Net_GetOpenSocketsOnInterface "$ifaceFilter"
for h_array_index in ${!LastFunctionCall[@]}; do
socketsEnterCr[$h_array_index]="${LastFunctionCall[$h_array_index]}"
done
enterSelCr=0
enterLineCr=""
for row in "${socketsEnterCr[@]}"
do
textOkEnterCr=0
if [ -z "$filter" ]
then
textOkEnterCr=1
else
if [[ `String_ToLower "$row"` == *`String_ToLower "$filter"`* ]]
then
textOkEnterCr=1
fi
fi
if [ $textOkEnterCr -ne 0 ]
then
if [ $enterSelCr -eq $selected ]
then
enterLineCr="$row"
fi
enterSelCr=$(($enterSelCr + 1))
fi
done
if [ -n "$enterLineCr" ]
then
actionSocketLine="$enterLineCr"
actionSocketPid=`Net_GetSocketProcessId "$actionSocketLine"`
actionPick=0
actionMenuOpen=1
fi
fi
if [[ "$key" == "j" ]]
then
selected=$(($selected + 1))
anchorLine=""
fi
if [[ "$key" == "k" ]]
then
selected=$(($selected - 1))
anchorLine=""
fi
if [[ "$key" == "DOWN" ]]
then
selected=$(($selected + 1))
anchorLine=""
fi
if [[ "$key" == "UP" ]]
then
selected=$(($selected - 1))
anchorLine=""
fi
if [[ "$key" == "PAGE_DOWN" ]]
then
selected=$(($selected + $listPageStep))
anchorLine=""
fi
if [[ "$key" == "PAGE_UP" ]]
then
selected=$(($selected - $listPageStep))
anchorLine=""
fi
if [[ "$key" == "g" ]]
then
selected=0
scroll=0
anchorLine=""
fi
if [[ "$key" == "G" ]]
then
selected=999999
anchorLine=""
fi
if [[ "$key" == "f" ]]
then
actionMenuOpen=0
if [ $stdinTui -ne 0 ]
then
Console_ExitInteractiveInputMode
fi
if [ $mouseEnabled -ne 0 ]
then
mouseEnabled=0
Console_DisableMouseReporting
fi
filterDialogOpen=1
filterTab=0
draftText="$filter"
draftIface="$ifaceFilter"
ifacePick=0
ifaceListScroll=0
if [ -z "$ifaceFilter" ]; then
h_String_IsNullOrEmpty_Result=1
else
h_String_IsNullOrEmpty_Result=0
fi
restoreIfacePick=$((! $h_String_IsNullOrEmpty_Result))
fi
if [[ "$key" == "/" ]]
then
actionMenuOpen=0
if [ $stdinTui -ne 0 ]
then
Console_ExitInteractiveInputMode
fi
if [ $mouseEnabled -ne 0 ]
then
mouseEnabled=0
Console_DisableMouseReporting
fi
filterDialogOpen=1
filterTab=0
draftText="$filter"
draftIface="$ifaceFilter"
ifacePick=0
ifaceListScroll=0
if [ -z "$ifaceFilter" ]; then
h_String_IsNullOrEmpty_Result=1
else
h_String_IsNullOrEmpty_Result=0
fi
restoreIfacePick=$((! $h_String_IsNullOrEmpty_Result))
fi
if [[ "$key" == "i" ]]
then
actionMenuOpen=0
if [ $stdinTui -ne 0 ]
then
Console_ExitInteractiveInputMode
fi
if [ $mouseEnabled -ne 0 ]
then
mouseEnabled=0
Console_DisableMouseReporting
fi
filterDialogOpen=1
filterTab=1
draftText="$filter"
draftIface="$ifaceFilter"
ifacePick=0
ifaceListScroll=0
restoreIfacePick=1
fi
if [[ "$key" == "]" ]]
then
Net_GetNetworkInterfaces
for h_array_index in ${!LastFunctionCall[@]}; do
ifcCycle[$h_array_index]="${LastFunctionCall[$h_array_index]}"
done
ifcCount=${#ifcCycle[@]}
currentPick=0
if [ -n "$ifaceFilter" ]
then
scan=1
for name in "${ifcCycle[@]}"
do
if [[ "$name" == "$ifaceFilter" ]]
then
currentPick=$scan
fi
scan=$(($scan + 1))
done
fi
nextPick=$(($currentPick + 1))
if [ $nextPick -gt $ifcCount ]
then
nextPick=0
fi
if [ $nextPick -eq 0 ]
then
ifaceFilter=""
else
ifaceFilter="${ifcCycle[$nextPick - 1]}"
fi
anchorLine=""
selected=0
scroll=0
fi
if [[ "$key" == "[" ]]
then
Net_GetNetworkInterfaces
for h_array_index in ${!LastFunctionCall[@]}; do
ifcCycle[$h_array_index]="${LastFunctionCall[$h_array_index]}"
done
ifcCount=${#ifcCycle[@]}
currentPick=0
if [ -n "$ifaceFilter" ]
then
scan=1
for name in "${ifcCycle[@]}"
do
if [[ "$name" == "$ifaceFilter" ]]
then
currentPick=$scan
fi
scan=$(($scan + 1))
done
fi
nextPick=$(($currentPick - 1))
if [ $nextPick -lt 0 ]
then
nextPick=$ifcCount
fi
if [ $nextPick -eq 0 ]
then
ifaceFilter=""
else
ifaceFilter="${ifcCycle[$nextPick - 1]}"
fi
anchorLine=""
selected=0
scroll=0
fi
if [[ "$key" == "c" ]]
then
filter=""
ifaceFilter=""
anchorLine=""
selected=0
scroll=0
fi
if [[ "$key" == "r" ]]
then
repaint=1
fi
if [[ "$key" == "m" ]]
then
if [ $mouseEnabled -ne 0 ]
then
mouseEnabled=0
Console_DisableMouseReporting
else
mouseEnabled=1
Console_EnableMouseReporting
fi
fi
fi
fi
else
if [ $filterDialogOpen -eq 0 ] && [ $actionMenuOpen -eq 0 ]
then
idlePolls=$(($idlePolls + 1))
if [ $idlePolls -ge $dataRefreshPolls ]
then
idlePolls=0
repaint=1
fi
fi
fi
if [ $repaint -ne 0 ]
then
Net_GetOpenSocketsOnInterface "$ifaceFilter"
for h_array_index in ${!LastFunctionCall[@]}; do
sockets[$h_array_index]="${LastFunctionCall[$h_array_index]}"
done
total=0
processGroups=0
lastGroupPid=-1
for row in "${sockets[@]}"
do
textOk=0
if [ -z "$filter" ]
then
textOk=1
else
if [[ `String_ToLower "$row"` == *`String_ToLower "$filter"`* ]]
then
textOk=1
fi
fi
if [ $textOk -ne 0 ]
then
total=$(($total + 1))
rowPid=`Net_GetSocketProcessId "$row"`
if [ $rowPid -ne $lastGroupPid ]
then
processGroups=$(($processGroups + 1))
lastGroupPid=$rowPid
fi
fi
done
if [ $total -eq 0 ]
then
selected=0
scroll=0
anchorLine=""
else
if [ $selected -ge $total ]
then
selected=$(($total - 1))
fi
if [ $selected -lt 0 ]
then
selected=0
fi
if [ -n "$anchorLine" ]
then
foundIndex=-1
scan=0
for row in "${sockets[@]}"
do
textOk=0
if [ -z "$filter" ]
then
textOk=1
else
if [[ `String_ToLower "$row"` == *`String_ToLower "$filter"`* ]]
then
textOk=1
fi
fi
if [ $textOk -ne 0 ]
then
if [[ "$row" == "$anchorLine" ]]
then
foundIndex=$scan
fi
scan=$(($scan + 1))
fi
done
if [ $foundIndex -ge 0 ]
then
selected=$foundIndex
fi
fi
fi
height=`Console_GetWindowHeight`
width=`Console_GetWindowWidth`
listChromeRows=5
if [ -n "$filter" ]
then
listChromeRows=$(($listChromeRows + 1))
fi
if [ -n "$ifaceFilter" ]
then
listChromeRows=$(($listChromeRows + 1))
fi
bodyRows=$(($height - $listChromeRows))
if [ $bodyRows -lt 4 ]
then
bodyRows=4
fi
if [ $total -gt 0 ]
then
anchorScan=0
for row in "${sockets[@]}"
do
textOkAnchor=0
if [ -z "$filter" ]
then
textOkAnchor=1
else
if [[ `String_ToLower "$row"` == *`String_ToLower "$filter"`* ]]
then
textOkAnchor=1
fi
fi
if [ $textOkAnchor -ne 0 ]
then
if [ $anchorScan -eq $selected ]
then
anchorLine="$row"
fi
anchorScan=$(($anchorScan + 1))
fi
done
adjustGuard=0
while
[ $adjustGuard -lt $(($total + 8)) ]
do
adjustGuard=$(($adjustGuard + 1))
firstVisible=-1
lastVisible=-1
simDrawn=0
simRow=0
simPrevPid=-1
for row in "${sockets[@]}"
do
textOkSim=0
if [ -z "$filter" ]
then
textOkSim=1
else
if [[ `String_ToLower "$row"` == *`String_ToLower "$filter"`* ]]
then
textOkSim=1
fi
fi
if [ $textOkSim -ne 0 ]
then
simPid=`Net_GetSocketProcessId "$row"`
if [ $simRow -ge $scroll ] && [ $simDrawn -lt $bodyRows ]
then
if [ $simRow -eq $scroll ] || [ $simPid -ne $simPrevPid ]
then
simDrawn=$(($simDrawn + 1))
fi
if [ $simDrawn -lt $bodyRows ]
then
if [ $firstVisible -lt 0 ]
then
firstVisible=$simRow
fi
lastVisible=$simRow
simDrawn=$(($simDrawn + 1))
fi
fi
simPrevPid=$simPid
simRow=$(($simRow + 1))
fi
done
if [ $firstVisible -lt 0 ]
then
scroll=0
adjustGuard=$(($total + 8))
else
if [ $selected -ge $firstVisible ] && [ $selected -le $lastVisible ]
then
listPageStep=$((($lastVisible - $firstVisible) + 1))
if [ $listPageStep -lt 1 ]
then
listPageStep=1
fi
adjustGuard=$(($total + 8))
else
if [ $selected -gt $lastVisible ]
then
scroll=$(($scroll + 1))
else
if [ $selected -lt $firstVisible ]
then
scroll=$(($scroll - 1))
fi
fi
if [ $scroll -lt 0 ]
then
scroll=0
fi
fi
fi
done
fi
listFirstSocketRow=3
if [ -n "$filter" ]
then
listFirstSocketRow=$(($listFirstSocketRow + 1))
fi
if [ -n "$ifaceFilter" ]
then
listFirstSocketRow=$(($listFirstSocketRow + 1))
fi
Console_BeginBatchWrite
Console_MoveCursorHome
hostName=`OS_GetHostName`
h_str_arg="${CBold}${CCyan} PortMonitor ${CReset}${CDim}| ${hostName} | ${total} sockets, ${processGroups} processes${CReset}"
Console_WriteLine "$h_str_arg"
if [ -n "$filter" ]
then
h_str_arg1="${CYellow} text: ${filter}${CReset}"
Console_WriteLine "$h_str_arg1"
fi
if [ -n "$ifaceFilter" ]
then
h_str_arg1="${CYellow} iface: ${ifaceFilter}${CReset}"
Console_WriteLine "$h_str_arg1"
fi
h_str_arg1="${CDim}Proto  State      Local              Peer               Process${CReset}"
Console_WriteLine "$h_str_arg1"
rowIndex=0
drawn=0
detail=""
prevSocketPid=-1
for row in "${sockets[@]}"
do
textOk=0
if [ -z "$filter" ]
then
textOk=1
else
if [[ `String_ToLower "$row"` == *`String_ToLower "$filter"`* ]]
then
textOk=1
fi
fi
if [ $textOk -ne 0 ]
then
rowPid=`Net_GetSocketProcessId "$row"`
if [ $rowIndex -ge $scroll ] && [ $drawn -lt $bodyRows ]
then
if [ $rowIndex -eq $scroll ] || [ $rowPid -ne $prevSocketPid ]
then
procName=`Process_GetName $rowPid`
if [ -z "$procName" ]
then
procName="?"
fi
header="${CBold}${CYellow}-- PID "${rowPid}" ${procName} --${CReset}"
Console_WriteLine "$header"
drawn=$(($drawn + 1))
fi
if [ $drawn -lt $bodyRows ]
then
if [ $rowIndex -eq $selected ]
then
h_str_arg12="${CRev}    ${row}${CReset}"
Console_WriteLine "$h_str_arg12"
detail="$row"
else
h_str_arg12="${CDim}    ${row}${CReset}"
Console_WriteLine "$h_str_arg12"
fi
drawn=$(($drawn + 1))
fi
fi
prevSocketPid=$rowPid
rowIndex=$(($rowIndex + 1))
fi
done
if [ -z "$detail" ] && [ -n "$anchorLine" ]
then
detail="$anchorLine"
fi
Console_WriteLine ""
if [ -n "$statusMessage" ]
then
h_str_arg12="${CGreen}${statusMessage}${CReset}"
Console_WriteLine "$h_str_arg12"
else
h_str_arg12="${CDim}${detail}${CReset}"
Console_WriteLine "$h_str_arg12"
fi
helpLine="j/k/PgUp/PgDn  Enter/a actions  f filter  i iface  c clear  q quit  m mouse"
if [ $mouseEnabled -ne 0 ]
then
helpLine="mouse on (no text select)  ${helpLine}"
fi
h_str_arg12="${CDim}${helpLine}${CReset}"
Console_WriteLine "$h_str_arg12"
while
[ $drawn -lt $bodyRows ]
do
Console_WriteLine ""
drawn=$(($drawn + 1))
done
if [ $filterDialogOpen -ne 0 ]
then
Net_GetNetworkInterfaces
for h_array_index1 in ${!LastFunctionCall[@]}; do
ifc[$h_array_index1]="${LastFunctionCall[$h_array_index1]}"
done
if [ $restoreIfacePick -ne 0 ]
then
restoreIfacePick=0
if [ -z "$ifaceFilter" ]
then
ifacePick=0
else
idx=1
for name in "${ifc[@]}"
do
if [[ "$name" == "$ifaceFilter" ]]
then
ifacePick=$idx
fi
idx=$(($idx + 1))
done
fi
fi
boxW=56
boxH=$(($height - 6))
if [ $boxH -gt 28 ]
then
boxH=28
fi
if [ $boxH -lt 14 ]
then
boxH=14
fi
top=$((($height - $boxH) / 2))
if [ $top -lt 2 ]
then
top=2
fi
left=$((($width - $boxW) / 2))
if [ $left -lt 2 ]
then
left=2
fi
border="+`String_Repeat "-" $(($(($boxW - 2))))`+"
Console_SetCursorPosition $top $left
h_str_arg123="${CRev}${border}${CReset}"
Console_Write "$h_str_arg123"
titleLine="`String_PadRight "| Filter (Enter apply, Esc/q cancel)" $(($(($boxW - 1)))) " "`|"
Console_SetCursorPosition $(($(($top + 1)))) $left
h_str_arg1234="${CRev}${titleLine}${CReset}"
Console_Write "$h_str_arg1234"
tab0=" Text "
tab1=" Interface "
tabBar=""
if [ $filterTab -eq 0 ]
then
tabBar="| [${tab0}] ${tab1}"
else
tabBar="|  ${tab0} [${tab1}]"
fi
tabBar="`String_PadRight "$tabBar" $(($(($boxW - 1)))) " "`|"
Console_SetCursorPosition $(($(($top + 2)))) $left
h_str_arg12345="${CRev}${tabBar}${CReset}"
Console_Write "$h_str_arg12345"
Console_SetCursorPosition $(($(($top + 3)))) $left
h_str_arg123456="${CRev}${border}${CReset}"
Console_Write "$h_str_arg123456"
if [ $filterTab -eq 0 ]
then
row="| Contains: ${draftText}"
row="`String_PadRight "$row" $(($(($boxW - 1)))) " "`|"
Console_SetCursorPosition $(($(($top + 5)))) $left
h_str_arg1234567="${CRev}${row}${CReset}"
Console_Write "$h_str_arg1234567"
Console_SetCursorPosition $(($(($top + 7)))) $left
h_str_arg12345678="${CRev}`String_PadRight "| Left/Right arrows: switch tabs" $(($(($boxW - 1)))) " "`|${CReset}"
Console_Write "$h_str_arg12345678"
else
ifcTotal=${#ifc[@]}
listSlots=$(($boxH - 9))
if [ $listSlots -lt 3 ]
then
listSlots=3
fi
if [ $ifacePick -gt 0 ]
then
pickInList=$(($ifacePick - 1))
if [ $pickInList -lt $ifaceListScroll ]
then
ifaceListScroll=$pickInList
fi
if [ $pickInList -ge $(($ifaceListScroll + $listSlots)) ]
then
ifaceListScroll=$((($pickInList - $listSlots) + 1))
fi
fi
if [ $ifaceListScroll -lt 0 ]
then
ifaceListScroll=0
fi
if [ $ifcTotal -gt $listSlots ] && [ $(($ifaceListScroll + $listSlots)) -gt $ifcTotal ]
then
ifaceListScroll=$(($ifcTotal - $listSlots))
fi
hint="| j/k pick  Enter apply  ("${ifcTotal}" interfaces)"
Console_SetCursorPosition $(($(($top + 4)))) $left
h_str_arg1234567="${CRev}`String_PadRight "$hint" $(($(($boxW - 1)))) " "`|${CReset}"
Console_Write "$h_str_arg1234567"
allLine="|   (all interfaces)"
if [ $ifacePick -eq 0 ]
then
allLine="| > (all interfaces)"
fi
allLine="`String_PadRight "$allLine" $(($(($boxW - 1)))) " "`|"
Console_SetCursorPosition $(($(($top + 5)))) $left
h_str_arg12345678="${CRev}${allLine}${CReset}"
Console_Write "$h_str_arg12345678"
displaySlot=0
ifcIndex=0
for name in "${ifc[@]}"
do
if [ $ifcIndex -ge $ifaceListScroll ] && [ $displaySlot -lt $listSlots ]
then
displaySlot=$(($displaySlot + 1))
mark="  "
if [ $ifacePick -eq $(($ifcIndex + 1)) ]
then
mark="> "
fi
line="| ${mark}${name}"
line="`String_PadRight "$line" $(($(($boxW - 1)))) " "`|"
Console_SetCursorPosition $(($((($top + 5) + $displaySlot)))) $left
h_str_arg123456789="${CRev}${line}${CReset}"
Console_Write "$h_str_arg123456789"
fi
ifcIndex=$(($ifcIndex + 1))
done
if [ $ifcTotal -gt $listSlots ]
then
scrollEnd=$(($ifaceListScroll + $listSlots))
if [ $scrollEnd -gt $ifcTotal ]
then
scrollEnd=$ifcTotal
fi
scrollHint="| ("$(($ifaceListScroll + 1))"-"${scrollEnd}" of "${ifcTotal}")"
Console_SetCursorPosition $(($((($top + $boxH) - 2)))) $left
h_str_arg123456789="${CRev}`String_PadRight "$scrollHint" $(($(($boxW - 1)))) " "`|${CReset}"
Console_Write "$h_str_arg123456789"
fi
fi
Console_SetCursorPosition $(($((($top + $boxH) - 1)))) $left
h_str_arg1234567="${CRev}${border}${CReset}"
Console_Write "$h_str_arg1234567"
fi
if [ $actionMenuOpen -ne 0 ]
then
actW=52
actH=10
actTop=$((($height - $actH) / 2))
if [ $actTop -lt 2 ]
then
actTop=2
fi
actLeft=$((($width - $actW) / 2))
if [ $actLeft -lt 2 ]
then
actLeft=2
fi
actBorder="+`String_Repeat "-" $(($(($actW - 2))))`+"
Console_SetCursorPosition $actTop $actLeft
h_str_arg123="${CRev}${actBorder}${CReset}"
Console_Write "$h_str_arg123"
actTitle="| Actions for selected socket (j/k, Enter)"
actTitle="`String_PadRight "$actTitle" $(($(($actW - 1)))) " "`|"
Console_SetCursorPosition $(($(($actTop + 1)))) $actLeft
h_str_arg1234="${CRev}${actTitle}${CReset}"
Console_Write "$h_str_arg1234"
actSub="| PID "${actionSocketPid}""
actSub="`String_PadRight "$actSub" $(($(($actW - 1)))) " "`|"
Console_SetCursorPosition $(($(($actTop + 2)))) $actLeft
h_str_arg12345="${CRev}${actSub}${CReset}"
Console_Write "$h_str_arg12345"
Console_SetCursorPosition $(($(($actTop + 3)))) $actLeft
h_str_arg123456="${CRev}${actBorder}${CReset}"
Console_Write "$h_str_arg123456"
act0="|  Append line to PortMonitor.log"
act1="|  Filter list by process name"
act2="|  Show process (ps)"
act3="|  Send SIGTERM to PID"
if [ $actionPick -eq 0 ]
then
act0="|> Append line to PortMonitor.log"
fi
if [ $actionPick -eq 1 ]
then
act1="|> Filter list by process name"
fi
if [ $actionPick -eq 2 ]
then
act2="|> Show process (ps)"
fi
if [ $actionPick -eq 3 ]
then
act3="|> Send SIGTERM to PID"
fi
act0="`String_PadRight "$act0" $(($(($actW - 1)))) " "`|"
act1="`String_PadRight "$act1" $(($(($actW - 1)))) " "`|"
act2="`String_PadRight "$act2" $(($(($actW - 1)))) " "`|"
act3="`String_PadRight "$act3" $(($(($actW - 1)))) " "`|"
Console_SetCursorPosition $(($(($actTop + 4)))) $actLeft
h_str_arg1234567="${CRev}${act0}${CReset}"
Console_Write "$h_str_arg1234567"
Console_SetCursorPosition $(($(($actTop + 5)))) $actLeft
h_str_arg12345678="${CRev}${act1}${CReset}"
Console_Write "$h_str_arg12345678"
Console_SetCursorPosition $(($(($actTop + 6)))) $actLeft
h_str_arg123456789="${CRev}${act2}${CReset}"
Console_Write "$h_str_arg123456789"
Console_SetCursorPosition $(($(($actTop + 7)))) $actLeft
h_str_arg12345678910="${CRev}${act3}${CReset}"
Console_Write "$h_str_arg12345678910"
actFoot="| q cancel"
actFoot="`String_PadRight "$actFoot" $(($(($actW - 1)))) " "`|"
Console_SetCursorPosition $(($(($actTop + 8)))) $actLeft
h_str_arg1234567891011="${CRev}${actFoot}${CReset}"
Console_Write "$h_str_arg1234567891011"
Console_SetCursorPosition $(($(($actTop + 9)))) $actLeft
h_str_arg123456789101112="${CRev}${actBorder}${CReset}"
Console_Write "$h_str_arg123456789101112"
fi
Console_WriteLine "$CEraseBelow"
Console_EndBatchWrite
fi
done
if [ $mouseEnabled -ne 0 ]
then
Console_DisableMouseReporting
fi
if [ -t 0 ]
then
Console_ExitInteractiveInputMode
fi
Console_DisableErrorLog
h_str_arg="${CShowCursor}${CAltScreenOff}${CReset}"
Console_WriteLine "$h_str_arg"
Console_Clear
h_str_arg1="${CGreen}[OK] PortMonitor closed.${CReset}"
Console_WriteLine "$h_str_arg1"
