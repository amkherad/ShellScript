#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
#-------------------------------------------------------------------------------
command="list"
case "$command" in
  help)
  echo "Show help."
  ;;
  list)
  echo "List items."
  ;;
  quit)
  echo "Goodbye."
  ;;
  *)
  echo "Unknown command: ${command}"
  ;;
esac
Assert_Equals "list" "$command"
