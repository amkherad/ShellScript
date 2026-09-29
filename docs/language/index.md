---
title: Language
nav_order: 2
has_children: true
---

# Language reference

ShellScript source files typically use the `.shellscript` extension. The compiler targets platform backends selected at compile or run time (`Unix-Bash`, `Windows-PowerShell`, `Windows-Batch`).

## Contents

- [Language specification](specification) — lexical grammar, statements, expressions, and what is implemented today
- [Core examples](https://github.com/amkherad/ShellScript/tree/master/Examples/Core) — variables, loops, functions, and OOP samples in the repo

## Minimal program

```shellscript
#!/usr/bin/env shellscript

echo "Hello, ShellScript!";
Console.WriteLine("via API");
```

Compile and run (Bash backend):

```bash
dotnet run --project ShellScript/ShellScript.csproj -- \
  compile script.shellscript script.bash Unix-Bash
bash script.bash
```
