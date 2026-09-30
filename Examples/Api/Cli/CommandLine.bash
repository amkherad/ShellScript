#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
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
function Console_WriteLine() {
  printf '%s\n' "$1"
}
function Cli_GetFlagValue() {
  local f="$1" a i
  for i in "${!_SS_SCRIPT_ARGS[@]}"; do
      a="${_SS_SCRIPT_ARGS[$i]}"
      case "$a" in "$f"=*) printf '%s' "${a#*=}"; return ;; "$f") printf '%s' "${_SS_SCRIPT_ARGS[$((i+1))]:-}"; return ;; esac
  done
  printf ''
}
function Console_WriteLine() {
  printf '%s\n' "$1"
}
function Console_WriteLine() {
  printf '%s\n' "$1"
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

command -v awk > /dev/null
if [ $? -eq 0 ]
then
  h_awk_existence1=1
else
  h_awk_existence1=0
fi
command -v bc > /dev/null
if [ $? -eq 0 ]
then
  h_bc_existence1=1
else
  h_bc_existence1=0
fi
command -v python3 > /dev/null
if [ $? -eq 0 ]
then
  h_python_existence1=1
else
  h_python_existence1=0
fi
if [ $h_awk_existence1 -ne 0 ]
then
  function Convert_ToInteger() {
    awk -v a="$1" 'BEGIN { printf "%.0f\n", a+0 }'
  }
elif [ $h_bc_existence1 -ne 0 ]
then
  function Convert_ToInteger() {
    echo "scale=10; ($1)/1" | bc -l
  }
elif [ $h_python_existence1 -ne 0 ]
then
  function Convert_ToInteger() {
    python3 -c 'import sys,math; a=float(sys.argv[1]); print(int(float(a)))' "$1"
  }
else
  function Convert_ToInteger() {
    echo $(( ${1%.*} ))
  }
fi

command -v awk > /dev/null
if [ $? -eq 0 ]
then
  h_awk_existence12=1
else
  h_awk_existence12=0
fi
command -v bc > /dev/null
if [ $? -eq 0 ]
then
  h_bc_existence12=1
else
  h_bc_existence12=0
fi
command -v python3 > /dev/null
if [ $? -eq 0 ]
then
  h_python_existence12=1
else
  h_python_existence12=0
fi
if [ $h_awk_existence12 -ne 0 ]
then
  function Convert_ToInteger() {
    awk -v a="$1" 'BEGIN { printf "%.0f\n", a+0 }'
  }
elif [ $h_bc_existence12 -ne 0 ]
then
  function Convert_ToInteger() {
    echo "scale=10; ($1)/1" | bc -l
  }
elif [ $h_python_existence12 -ne 0 ]
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
#Resolve third-party utility backends once (keeps hot paths branch-free).
command -v awk > /dev/null
if [ $? -eq 0 ]
then
  h_awk_existence123=1
else
  h_awk_existence123=0
fi
command -v bc > /dev/null
if [ $? -eq 0 ]
then
  h_bc_existence123=1
else
  h_bc_existence123=0
fi
command -v python3 > /dev/null
if [ $? -eq 0 ]
then
  h_python_existence123=1
else
  h_python_existence123=0
fi
if [ $h_awk_existence123 -ne 0 ]
then
  function Convert_ToInteger() {
    awk -v a="$1" 'BEGIN { printf "%.0f\n", a+0 }'
  }
elif [ $h_bc_existence123 -ne 0 ]
then
  function Convert_ToInteger() {
    echo "scale=10; ($1)/1" | bc -l
  }
elif [ $h_python_existence123 -ne 0 ]
then
  function Convert_ToInteger() {
    python3 -c 'import sys,math; a=float(sys.argv[1]); print(int(float(a)))' "$1"
  }
else
  function Convert_ToInteger() {
    echo $(( ${1%.*} ))
  }
fi

command -v awk > /dev/null
if [ $? -eq 0 ]
then
  h_awk_existence1234=1
else
  h_awk_existence1234=0
fi
command -v bc > /dev/null
if [ $? -eq 0 ]
then
  h_bc_existence1234=1
else
  h_bc_existence1234=0
fi
command -v python3 > /dev/null
if [ $? -eq 0 ]
then
  h_python_existence1234=1
else
  h_python_existence1234=0
fi
if [ $h_awk_existence1234 -ne 0 ]
then
  function Convert_ToInteger() {
    awk -v a="$1" 'BEGIN { printf "%.0f\n", a+0 }'
  }
elif [ $h_bc_existence1234 -ne 0 ]
then
  function Convert_ToInteger() {
    echo "scale=10; ($1)/1" | bc -l
  }
elif [ $h_python_existence1234 -ne 0 ]
then
  function Convert_ToInteger() {
    python3 -c 'import sys,math; a=float(sys.argv[1]); print(int(float(a)))' "$1"
  }
else
  function Convert_ToInteger() {
    echo $(( ${1%.*} ))
  }
fi

command -v awk > /dev/null
if [ $? -eq 0 ]
then
  h_awk_existence12345=1
else
  h_awk_existence12345=0
fi
command -v bc > /dev/null
if [ $? -eq 0 ]
then
  h_bc_existence12345=1
else
  h_bc_existence12345=0
fi
command -v python3 > /dev/null
if [ $? -eq 0 ]
then
  h_python_existence12345=1
else
  h_python_existence12345=0
fi
if [ $h_awk_existence12345 -ne 0 ]
then
  function Convert_ToInteger() {
    awk -v a="$1" 'BEGIN { printf "%.0f\n", a+0 }'
  }
elif [ $h_bc_existence12345 -ne 0 ]
then
  function Convert_ToInteger() {
    echo "scale=10; ($1)/1" | bc -l
  }
elif [ $h_python_existence12345 -ne 0 ]
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
#! Void printUsage
function printUsage() {
  echo "CommandLine — Cli parsing demo" > /dev/tty
  echo "" > /dev/tty
  echo "Usage:" > /dev/tty
  echo "  CommandLine <command> [options] [args...]" > /dev/tty
  echo "" > /dev/tty
  echo "Commands:" > /dev/tty
  echo "  greet              Print a greeting (--name / -n, --count, --verbose / -v)" > /dev/tty
  echo "  sum <numbers...>   Sum numeric arguments (flags ignored)" > /dev/tty
  echo "  config show        Show --file= path and optional positional key" > /dev/tty
  echo "" > /dev/tty
  echo "Global:" > /dev/tty
  echo "  --help, -h         Show this help" > /dev/tty
}
#! Void greet
function greet() {
  local verboseFlag=`Cli_HasFlag "--verbose"`
  local verboseShort=`Cli_HasFlag "-v"`
  local verbose=$(($verboseFlag || $verboseShort))
  local name=`Cli_GetFlagValue "--name"`
  if [ -z "$name" ]
  then
    name=`Cli_GetFlagValue "-n"`
  fi
  if [ -z "$name" ]
  then
    name="world"
  fi
  local countText=`Cli_GetFlagValue "--count"`
  local times=1
  if [ -n "$countText" ]
  then
    times=`Convert_ToInteger "$countText"`
  fi
  local t=0
  while
  [ $t -lt $times ]
  do
    if [ $verbose -ne 0 ]
    then
      echo "[verbose] greet #$(($t + 1))"
    fi
    echo "Hello, ${name}!"
    ((t++))
  done
}
#! Void sumNumbers
function sumNumbers() {
  local argc=${#_SS_SCRIPT_ARGS[@]}
  local total=0
  local i=1
  while
  [ $i -lt $argc ]
  do
    local token=${_SS_SCRIPT_ARGS[$i]}
    if [[ "$token" == "--verbose" ]]; then
      local h_String_Equals_Result=1
    else
      local h_String_Equals_Result=0
    fi
    local isVerbose=$h_String_Equals_Result
    if [[ "$token" == "-v" ]]; then
      local h_String_Equals_Result1=1
    else
      local h_String_Equals_Result1=0
    fi
    local isVerboseShort=$h_String_Equals_Result1
    local skip=$(($isVerbose || $isVerboseShort))
    if [[ "$token" == "--"* ]]; then
      local h_String_StartsWith_Result=1
    else
      local h_String_StartsWith_Result=0
    fi
    local isLongFlag=$h_String_StartsWith_Result
    if [ $skip -eq 0 ] && [ $isLongFlag -ne 0 ]
    then
      skip=1
    fi
    if [[ "$token" == "-"* ]]; then
      local h_String_StartsWith_Result1=1
    else
      local h_String_StartsWith_Result1=0
    fi
    local isShortFlag=$(($h_String_StartsWith_Result1 && (! $isLongFlag)))
    if [ $skip -eq 0 ] && [ $isShortFlag -ne 0 ]
    then
      skip=1
    fi
    if [ $skip -eq 0 ]
    then
      total=$(($total + `Convert_ToInteger "$token"`))
    fi
    i=$(($i + 1))
  done
  h_str_arg="sum=${total}"
  Console_WriteLine "$h_str_arg"
  if [ $total -gt 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
}
#! Void configShow
function configShow() {
  local file=`Cli_GetFlagValue "--file"`
  if [ -z "$file" ]
  then
    file="(none)"
  fi
  h_str_arg="config-file=${file}"
  Console_WriteLine "$h_str_arg"
  local argc=${#_SS_SCRIPT_ARGS[@]}
  local i=2
  while
  [ $i -lt $argc ]
  do
    local token=${_SS_SCRIPT_ARGS[$i]}
    if [[ "$token" == "--"* ]]; then
      local h_String_StartsWith_Result=1
    else
      local h_String_StartsWith_Result=0
    fi
    local isFlag=$h_String_StartsWith_Result
    if [ $isFlag -eq 0 ]
    then
      h_str_arg1="config-key=${token}"
      Console_WriteLine "$h_str_arg1"
      i=$argc
    fi
    i=$(($i + 1))
  done
}
#! Void dispatch
function dispatch() {
  local command=${_SS_SCRIPT_ARGS[0]}
  case "$command" in
    greet)
    greet
    ;;
    sum)
    sumNumbers
    ;;
    config)
    local configArgc=${#_SS_SCRIPT_ARGS[@]}
    if [ $configArgc -lt 2 ]
    then
      echo "config: expected subcommand 'show'"
    else
      local sub=${_SS_SCRIPT_ARGS[1]}
      case "$sub" in
        show)
        configShow
        ;;
        *)
        echo "config: unknown subcommand '${sub}'"
        ;;
      esac
    fi
    ;;
    *)
    echo "Unknown command: ${command}"
    ;;
  esac
}
argc=${#_SS_SCRIPT_ARGS[@]}
helpFlag=`Cli_HasFlag "--help"`
helpShort=`Cli_HasFlag "-h"`
if [ $argc -eq 0 ] || [ $helpFlag -ne 0 ] || [ $helpShort -ne 0 ]
then
  printUsage
  if [ 1 -ne 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
  echo "EXAMPLE_OK:CommandLine"
else
  dispatch
  if [ $argc -gt 0 ]; then :; else printf '%s\n' "Assert.True failed." >&2; return 1 2>/dev/null || exit 1; fi
  echo "EXAMPLE_OK:CommandLine"
fi
