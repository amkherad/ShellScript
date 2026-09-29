#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
#-------------------------------------------------------------------------------
score=85
grade=""
if [ $score -ge 90 ]
then
grade="A"
elif [ $score -ge 80 ]
then
grade="B"
elif [ $score -ge 70 ]
then
grade="C"
else
grade="F"
fi
echo "Score ${score} => grade ${grade}"
if [ $score -gt 0 ] && [ $score -le 100 ]
then
echo "Score is in valid range."
fi
