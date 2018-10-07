# ShellScript

| Linux Build (Travis-CI) |
|-------------------------|
| [![Build Status](https://travis-ci.com/amkherad/ShellScript.svg?branch=master)](https://travis-ci.com/amkherad/ShellScript) |

[Work In Progress]  
Cross-platform intermediate shell scripting language.  
A transpiler to generate native OS shell commands from a shared code base with a powerfull class library and rich language features.

Learning all of the shell scripting languages with all those details could be so fraustating and it can be forgotten so easily due to tons of details and special cases, the idea of ShellScript is to create an intermediate language that can be transpiled (source to source compilation) into any shell language with use of paltform-dependent utilities.

## Installing

### Arch Linux

```
sudo pacman -S shellscript
```
You can install dotnet package using (the above command will install dotnet automatically):
```
sudo pacman -S dotnet-runtime
sudo pacman -S dotnet-host
```

### Download the Binaries

[ShellScript Releases](https://github.com/amkherad/ShellScript/releases) 

### Build from the source

First you need to install dotnet core host and runtime along side with SDK:

Arch Linux:
```
sudo pacman -S dotnet-host
sudo pacman -S dotnet-runtime
sudo pacman -S dotnet-sdk
```

For other operating systems please refer to [Microsoft .NET downloads](https://www.microsoft.com/net/download) and install .NET Core 2.1

Clone the repository and build the solution:
```
git clone git@github.com:amkherad/ShellScript.git
cd ShellScript
dotnet build ShellScript.sln
```

* You can find the binaries in ShellScript/ShellScript/bin/[Release-Debug]/[Framework]

---

## Getting Started

ShellScript is a C#-like language with less features from C# and some new features to allow coding for shell scripting environments.  
As of today ShellScript supports transpiling to Unix-Bash and Windows-Batch shell languages.

Here are some rules of the ShellScript:

* Keywords in ShellScript are case-sensitive.
* White-spaces are totally ignored unless they're inside single or double quote.
* Blocks are free to use (i.e. you can have {} wherever you want just like C#)
* Variables are accessible to their scope only.
* Using arrays will generate hacks to implement the functions, so avoid using arrays as possible.
* API functions and objects will be written to the output only when they're used inside the code.
* ShellScript API methods will use platform's dedicated way to get the results wherever possible instead of writing it's own function.
  ```csharp
  int x = -23;
  echo Math.Abs(x); //will generate 'echo ${x#-}' in bash.
  ```
* There's no label/goto syntax.

### Data Types
ShellScript is a strong-typed/static-typed language, all data types are listed below:  

| Data Type | Alias(es)     | Description
|-----------|---------------|-------------
| Decimal   | decimal, int  | Any decimal number. (e.g. 956)
| Float     | double, float | Any float number. (e.g. 56.48)
| Number    | number        | Any number, either decimal or float.
| String    | string        | Representing a string of characters.
| Boolean   | bool          | Representing a boolean value. (i.e. true or false)
| Void      | void          | Void data type. (The only usage is to define a void method)
| Object    | object        | Representing an instance of a class. (different from object in C#) `[NOT IMPLEMENTED YET]`
| Array     | DATATYPE[]    | Represents an array of items of the given data type. (e.g. int[]) `[NOT IMPLEMENTED YET]`
| Delegate  | delegate      | Holds a reference to a callable object (function). `[NOT IMPLEMENTED YET]`

Example of data types in code:
```csharp
int i = 956;
decimal d = 956;
float f = 56.48;
float f = +8.56e-23;
double d = +8.56e-23;
number n = 956;
number m = +8.56e-23;
string s = "Hello World";
boolean b = true || false;
```

##### Notices

* Types have no boundaries or limitation in the language itself but they're limited to target platform specifications.

* Only decimal types can cast to float types implicitly, for other types an explicit cast is required.

### Variable Definition
Variable definitions are similar to C#, except there is no var keyword for auto typing.  
There are four places for variable definitions:  
* Defining a variable inside a block of code:

  ```csharp
  int myVariable = 43;
  ```


* Defining a variable inside for/foreach loop:

  ```csharp
  for (int myVariable = 0; myVariable < 10; myVariable++) { }
  foreach (int myVariable in GetIntegers()) { }
  ```
  - foreach variable is immutable inside foreach block (and inaccessible outside of the block, if it's defined in the foreach statement).

* Defining a parameter in a function definition:

  ```csharp
  void myFunction (int myParam1) { }
  ```

##### Notices

* Just like C#, variable definition is not an embedded statement, example:
  ```csharp
  for (int i = 0; i < 100; i++)
      int j = i; //Compiler error.
  
  for (int i = 0; i < 100; i++) {
      int j = i; //OK
  }
  ```

### Assignment
There are four places for assignments:  
* Assigning a variable inside a block of code:

  ```csharp
  myVariable = 43;
  ```


* Assigning a variable inside for/foreach loop:

  ```csharp
  for (myVariable = 0; myVariable < 10; myVariable++) { }
  foreach (myVariable in GetIntegers()) { }
  ```

* Assigning a parameter to a default value:

  ```csharp
  void myFunction (int myParam1 = 10) { }
  ```

* Assigning a variable inside an evaluation expression: `[NOT IMPLEMENTED YET]`

  ```csharp
  myVariable = x = 2;
  while ((that = that.Parent) != null) { }
  ```

### Function Definition
ShellScript use same syntax for function definition as C#.  

```csharp
int myFunction (int parameter1) {
    return 0;
}
```

Same as the C#, all code paths must return a value, so this is a compiler error:

```csharp
double myDouble (decimal parameter1) {
    if (parameter1 < 10) {
        echo "ERROR";

        //No return from if body. :CompilerError
    } else {
        return 0;
    }
}
```

### Conditional Blocks (Branches)

#### If
`if` block is implemented exactly as C#.

```csharp
if (condition) {
    echo ("condition is true");
} else if (condition2 == true) {
    echo ("condition2 is true");
} else {
    echo ("condition and condition2 are false");
}
```

#### Switch Case
`switch case` block is implemented exactly as C#.

```csharp
switch (value1) {
    case "Item1": {
        echo ("Item1");
        break;
    }
    default: {
        echo ("Default");
    }
}
```

##### Notices

* Conditional blocks with constant conditions will be compiled if their condition is true.
  ```csharp
  if (true) {
      echo ("true");
  } else if (false) {
      echo ("false");
  } else {
      echo ("else");
  }
  //The entire if statement will be removed and only one echo ("true") will be generated.
  ```
* `if` may compile to a `switch case` syntax if required or vice-versa.
* `if` and `switch case` considered as branch statements, they're required to return a value in every branch inside a non-void method, and may be converted to arithmetic/logical expression to remove branch.

### Loops
There are four loops in ShellScript.

#### for
`for` syntax used to iterate using a condition and an optional counter:
```csharp
for (int i = 0; i < 10; i++) {
    echo ("Line: " + i);
}
```

#### foreach
`foreach` syntax used to iterate over an array:
```csharp
foreach (int age in GetAges()) {
    echo ("Age is: " + age);
}
```

#### while
`while` syntax used to iterate using a condition:
```csharp
while (_continue) {
    echo ($"Continue is {_continue}");
}
```

#### do while
`do while` syntax used to iterate at least once using a condition:
```csharp
do {
    echo ("Going to check the condition.");
} while (_continue);
```

##### Notices

* Empty loops will be ignored.
* Preferred loop for infinite iterations is **`for`**.
  ```csharp
  for (;;) {
      //Infinite loop
  }
  ```

### Evaluation Expressions And Operators

There's no limitations on expressions, but it's highly suggested to use parenthesis to clarify expressions.

Here are all the operators with their order (first row has the most priority):

| Category         | Operator                            | Associativity
|------------------|-------------------------------------|-----------------
| Primary/Postfix  | () `[]` ++ -- .                     | Left to right
| Unary	           | + - ! ~ ++ - - `(type)`             | Right to left
| Multiplicative   | * / %                               | Left to right
| Additive         | + -                                 | Left to right
| Shift            | `<<` `>>`                           | Left to right
| Relational       | < <= > >=                           | Left to right
| Equality         | == !=                               | Left to right
| Bitwise AND	   | &                                   | Left to right
| Bitwise XOR	   | ^                                   | Left to right
| Bitwise OR	   | \|                                  | Left to right
| Logical AND	   | &&                                  | Left to right
| Logical OR	   | \|\|                                | Left to right
| Conditional	   | `?:`                                | Right to left
| Assignment	   | = `+= -= *= /= %=>>= <<= &= ^= \|=` | Right to left
| Comma	           | ,                                   | Left to right

`Some operators are not implemented yet.`

```csharp
return (1024 ^ 1023) + 1024 * 2; //7 + 2048 = 2055
```

Non-void functions are considered as evaluation expression:
```csharp
return 2 * factorial(20);
```

##### Notices

* Expressions are parsed from left to right.
* Expressions may be truncated into multiple helper variables.
* Constant expressions will be calculated at compile-time. (i.e. 1024 * 2 will generate 2048 constant value)
* Both void methods or non-void methods will be inlined if there's only one statement inside.
* By default ShellScript only check errors/exceptions for the methods/commands having throw syntax inside or marked by a throws syntax.
* Writing raw platform-specific code is considered unsafe and should be avoided unless there's no other way.
* You can enable value tracking to treat variables as constant if their value is not changed.
  ```csharp
  int x = 10;
  return x * 10 / 3;
  //if value-tracking enabled it will generate "return 33" constant value instead of arithmetic evaluation.
  //that's because value of x is not changed before reading it.
  ```
* Evaluation of second operand in logical operators is UB (Undefined Behavior) and it depends on the target shell.
* It's better to avoid micro-optimizations or outsmarting the compiler, because ShellScript will optimize the well-known statements to platform's dedicated way to implement the functionality, even ignore statements or reorder for better results, and doing so will prevent ShellScript from recognizing the function.
  ```csharp
  //keep files of a directory in an array to optimize performance.
  string[] files = Directory.GetFiles("Path-To-Directory");
  foreach (string fileName in files) {
      echo (fileName);
  }
  //somewhere else in the code, we need to iterate the files again.
  foreach (string fileName in files) {
      echo (fileName);
  }

  //the code above will prevent to query the file system twice, but as said before using arrays will generate hacks,
  //so the generated code might not be as good as expected.

  //now iterating directory's files by querying file system separately.
  foreach (string fileName in Directory.GetFiles("Path-To-Directory")) {
      echo (fileName);
  }
  //somewhere else in the code, we need to iterate the files again.
  foreach (string fileName in Directory.GetFiles("Path-To-Directory")) {
      echo (fileName);
  }

  //this code will generate "for filename in Path-To-Directory/*; do" in bash and
  //it's easier to understand (if required to read the output) and it's not using any hacks.
  //but it may be slower (you can benchmark your code to choose which is better for your requirements)
  ```


#### String Concatenation
You can concatenate strings using addition operator (+) or using string interpolation ($"").
```csharp
return "Hello" + " " + "World " + dateValue;
```
```csharp
return $"Hello World {dateValue}";
```

##### Notices

* You can use both single quote or double quote. (both are strings, ShellScript has no character data type)
* String escaping is exactly like C#.
* Concatenating constant strings will generate a concatenated constant string.
* You can use multiplication operator on one decimal and one string to repeat the string.
  ```csharp
  echo (80 * "-"); //will repeat (-) 80 times.
  ```

Transpiler will do it's best for reliability and high performance, but sometimes the performance and correctness relies on external utilities, like using floating-point arithmetic in bash, it will use `awk` or other utilities available to do the math but they can reject to calculate the expression. (e.g. compiling `awk` with no math enabled)

## Echo
Echo is one of the special syntaxes in ShellScript. it's classified as a **void** function call. (i.e. cannot be used inside evaluation expressions)  
The echo syntax doesn't require parenthesis unlike general function calls.

```csharp
echo "Hello World";
```

Most of the times echo is a syntax-to-syntax transpilation, but it can generate different syntaxes on different target platforms, look at this code:
```csharp
int myFunction (int param1) {
    echo ("Hello World");
    return 10;
}
```
This code will transpile to the following script in unix bash:
```bash
function myFunction () {
    echo "Hello World" > /dev/tty
    echo 10;
    return 0; //this might be omitted.
}
```
The first echo inside the method writes directly to `/dev/tty` in unix. because shell environments use standard-output to return a value. (i.e. by redirecting the output of a command/function to a variable or another command)

## Pre-Processors (Macros)

Like every C-like language ShellScript has pre-processors.  
Pre-processors are checked before the compilation begins, so you can make statements and tokens conditional, or changing some compilation behaviors.

#### If pre-processor
`#if` used to make statements and tokens conditional:
```csharp
#if (Bash)
echo ("Welcome bash users");
#elseif (Batch)
echo ("Welcome batch users");
#else
echo ("Welcome everyone");
#endif
```

#### Option pre-processor
`#option` used to set or override compiler options.
```csharp
#option ("Author", "Ali Mousavi Kherad") //this will set the author property.
#option ("Awk", "Disable") //this will disable usage of awk
```

###### Notices
* If pre-processor MUST have parenthesis unlike C#.

---

## API and Class Library
ShellScript provide some API methods to minify the need to write platform-specific code for each platform.  
These methods try to use the target shell's dedicated way to get the results but in cases they will generate meta functions inside the output script file. (meta codes are at the beginning of the file)

* It's possible to override API functions by knowing the generated function name and writing your own function with that name before the first usage of the API.

[Enter the API documentation here](https://github.com/amkherad/ShellScript/blob/master/docs/ClassLibrary.md)

---

## Command-Line
Command-line format is ShellScript command [parameters] [-switch-name[=switch-value]]

#### help, -h, --help
To show the help.

#### --platforms
To print the installed target platforms.

#### -v, --version
To print the version of the compiler.

#### compile, -c, --compile
To compile a file/project.
```
ShellScript compile Source Output Platform [-switch-name[=switch-value]]
```
Example:
```
ShellScript compile /home/github/projects/test.shellscript /home/github/projects/build/test.bash Unix-Bash --verbose
```

---  

## Contributing

Please read [Contributing.md](https://github.com/amkherad/ShellScript/blob/master/docs/Contributing.md) for details on our code of conduct, and the process for submitting pull requests to us.

## Authors

* **Ali Mousavi Kherad** - *Owner*

See also the list of [contributors](https://github.com/amkherad/ShellScript/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [https://opensource.org/licenses/MIT](https://opensource.org/licenses/MIT) for details
