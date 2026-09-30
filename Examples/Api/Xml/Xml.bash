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
  function Xml_IsValid() {
    python3 -c 'import xml.etree.ElementTree as ET,sys; ET.fromstring(sys.argv[1])' "$1" >/dev/null
  }
else
  function Xml_IsValid() {
    python3 -c 'import xml.etree.ElementTree as ET,sys; ET.fromstring(sys.argv[1])' "$1" >/dev/null && echo 1 || echo 0
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
  function Xml_GetPath() {
    python3 -c 'import sys,xml.etree.ElementTree as ET; r=ET.fromstring(sys.argv[1]); print(r.findtext(sys.argv[2]))' "$1" "$2"
  }
else
  function Xml_GetPath() {
    python3 -c 'import sys,xml.etree.ElementTree as ET; print(ET.fromstring(sys.argv[1]).tag)' "$1"
  }
fi

#-------------------------------------------------------------------------------
xml="<root><item>shell</item></root>"
valid=`Xml_IsValid "$xml"`
item=`Xml_GetPath "$xml" "item"`
echo "valid=${valid}"
echo "$item"
Assert_Equals "shell" "$item"
echo "EXAMPLE_OK:Xml"
