#!/usr/bin/env bash
#ShellScript v0.1.2.2018 - [https://github.com/amkherad/ShellScript]
#-------------------------------------------------------------------------------
#-------------------------------------------------------------------------------
coffeePrice=3.5
coffeeCount=4
pizzaPrice=39.99
pizzaCount=3
total=`awk "BEGIN {print (($coffeePrice * $coffeeCount) + ($pizzaPrice * $pizzaCount))}"`
taxPercentage=7.2
totalCaption="Total"
h_arithmetic=`awk "BEGIN {print ($total - (($total * $taxPercentage) / 100))}"`
totalStr1="${totalCaption}: ${h_arithmetic}"
h_arithmetic1=`awk "BEGIN {print ($total - (($total * $taxPercentage) / 100))}"`
totalStr2="Total: ${h_arithmetic1}"
echo "$totalStr1"
echo "$totalStr2"
