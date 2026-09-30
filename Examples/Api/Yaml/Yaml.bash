#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
function Assert_Equals() {
  if [[ "$1" == "$2" ]]; then return 0; fi
  if [ -n "$3" ]; then printf '%s\n' "$3" >&2; else printf 'Assert.Equals failed: expected "%s" but was "%s"\n' "$1" "$2" >&2; fi
  return 1 2>/dev/null || exit 1
}
#-------------------------------------------------------------------------------
#Resolve third-party utility backends once (keeps hot paths branch-free).
command -v python3 > /dev/null
if [ $? -eq 0 ]
then
  h_python_existence=1
else
  h_python_existence=0
fi
if [ $h_python_existence -ne 0 ]
then
  function Yaml_IsValid() {
    python3 -c 'import yaml,sys; yaml.safe_load(sys.argv[1])' "$1" >/dev/null
  }
else
  function Yaml_IsValid() {
    python3 -c 'import yaml,sys; yaml.safe_load(sys.argv[1])' "$1" >/dev/null && echo 1 || echo 0
  }
fi

command -v python3 > /dev/null
if [ $? -eq 0 ]
then
  h_python_existence1=1
else
  h_python_existence1=0
fi
if [ $h_python_existence1 -ne 0 ]
then
  function Yaml_GetPath() {
    python3 -c 'import sys,yaml; d=yaml.safe_load(sys.argv[1]); p=sys.argv[2].split("."); c=d
    for k in p:
      c=c[k]
    print(c)' "$1" "$2"
  }
else
  function Yaml_GetPath() {
    python3 -c 'import sys,yaml; print(yaml.safe_load(sys.argv[1]))' "$1"
  }
fi

#-------------------------------------------------------------------------------
yaml="name: shell"
valid=`Yaml_IsValid "$yaml"`
name=`Yaml_GetPath "$yaml" "name"`
echo "valid=${valid}"
echo "$name"
Assert_Equals "shell" "$name"
echo "EXAMPLE_OK:Yaml"
