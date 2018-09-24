# ShellScript

[Work In Progress]  
Cross-platform intermediate shell scripting language.  
A transpiler to generate native OS shell commands from a shared code base with a powerfull class library and rich language features.

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

### Download the Binary

[ShellScript Binaries](http://google.com/) 

### Build from the source

First you need to install dotnet core SDK and runtime along side with host:

Arch Linux:
```
sudo pacman -S dotnet-runtime
sudo pacman -S dotnet-host
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

## Getting Started

ShellScript is a C# like language with less features from C# and with some additional features to allow coding for shell scripting environments.  
As of today ShellScript supports transpiling to Unix-Bash and Windows-Batch shell languages.

### Data Types
ShellScript is a strong-typed/static-typed language, all data types are listed below:  

| DataType | Alias(es)     | Description
|----------|---------------|-------------
| Decimal  | decimal, int  | Any decimal number. (e.g. 956)
| Float    | double, float | Any float number. (e.g. 56.48)
| Number   | number        | Any number, either decimal or float.
| String   | string        | Representing a string of characters.
| Void     | void          | Void data type. (The only usage is to define a void method)
| Object   | object        | Representing an instance of a class. (different from objects in C#) `[NOT IMPLEMENTED YET]`
| Array    | {DATATYPE}[]  | Represents an array of items of the given data type. (e.g. int[]) `[NOT IMPLEMENTED YET]`
| Delegate | delegate      | Holds a reference to a callable object (function). `[NOT IMPLEMENTED YET]`

Types have no boundaries or limitation in the language itself but they're limited to target platform specifications.

Only decimal types can cast to float types implicitly, for other types an explicit cast is required.

Example of data types in code:
```csharp
int i = 956;
decimal d = 956;
float f = 56.48;
double d = +8.56e-23;
number n = 956;
number m = +8.56e-23;
string s = "Hello World";
```

### Variable Definition
Variable definitions are similar to C#, except there is no var keyword for auto typing.  
There are four places for variable definitions:  
* Defining a variable inside a block of code:

  ```csharp
  int myVariable = 43;
  ```


* Defining a variable inside for/foreach loop:

  ```csharp
  for(int myVariable = 0; myVariable <10; myVariable++) { }
  foreach(int myVariable in GetIntegers()) { }
  ```

* Defining a parameter in a function definition:

  ```csharp
  void myFunction(int myParam1) { }
  ```

Just like C#, variable definition is not an embedded statement, example:
```csharp
for (int i = 0; i < 100; i++)
    int j = i; //Compiler error.

for (int i = 0; i < 100; i++) {
    int j = i; //OK
}
```

### Assignement
There are four places for assignements:  
* Assigning a variable inside a block of code:

  ```csharp
  myVariable = 43;
  ```


* Assigning a variable inside for/foreach loop:

  ```csharp
  for(myVariable = 0; myVariable <10; myVariable++) { }
  [^footnote1] foreach(myVariable in GetIntegers()) { }
  ```

* Assigning a parameter to a default value:

  ```csharp
  void myFunction(int myParam1 = 10) { }
  ```

* Assigning a variable inside an evaluation expression: `[NOT IMPLEMENTED YET]`

  ```csharp
  myVariable = x = 2;
  while ((that = that.Parent) != null) { }
  ```

[^footnote1]: foreach variable is immutable inside foreach block (and inaccessible outside of the block, if it's defined in the foreach statement).

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

### Conditional Blocks

**If** block:  
If block is implemented exactly as C#.

```csharp
if (condition) {
    echo ("condition is true");
} else if (condition2 == true) {
    echo ("condition2 is true");
} else {
    echo ("condition and condition2 are false");
}
```

**Switch Case** block:  
Switch case block is implemented exactly as C#.

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

### Loops
There are four loops in ShellScript.

* **for** to iterate using a condition and an optional counter:
  ```csharp
  for (int i = 0; i < 10; i++) {
      echo ("Line: " + i);
  }
  ```

* **foreach** to iterate over an array:
  ```csharp
  foreach (int age in GetAges()) {
      echo ("Age is: " + age);
  }
  ```

* **while** to iterate using a condition:
  ```csharp
  while (_continue) {
      echo ($"Continue is {_continue}");
  }
  ```

* **do while** to iterate at least once using a condition:
  ```csharp
  do {
      echo ("Going to check the condition.");
  } while (_continue);
  ```

### Evaluation Expressions And Operators

There's no limitations to expressions, but it's highly suggested to use parenthesis to clarify expressions -and it will compile faster :)-.

| Category         | Operator                          | Associativity
|------------------|-----------------------------------|-----------------
| Postfix          | () [] ++ -- .                     | Left to right
| Unary	           | + - ! ~ ++ - - (type)             | Right to left
| Multiplicative   | * / %                             | Left to right
| Additive         | + -                               | Left to right
| Shift            | << >>                             | Left to right
| Relational       | < <= > >=                         | Left to right
| Equality         | == !=                             | Left to right
| Bitwise AND	   | &                                 | Left to right
| Bitwise XOR	   | ^                                 | Left to right
| Bitwise OR	   | \|                                | Left to right
| Logical AND	   | &&                                | Left to right
| Logical OR	   | \|\|                              | Left to right
| Conditional	   | ?:                                | Right to left
| Assignment	   | = += -= *= /= %=>>= <<= &= ^= \|= | Right to left
| Comma	           | ,                                 | Left to right

`Some operators are not implemented yet.`

```csharp
return (1024 ^ 1023) + 1024 * 2; //1 + 2048 = 2049
```

Non-void functions are considered as an evaluation expression:
```csharp
return 2 * factorial(20);
```

Transpiler will do it's best for reliability and high performance, but sometimes the performance and correctness rely on external utilities, like using floating-point arithmetic in bash, it will use **awk** or other utilities available to do the math but they can reject to calculate the expression. (e.g. compiling awk with no math enabled)

## Echo
Echo is one of the special syntaxes in ShellScript. it's classified as a **Void** function call. (i.e. cannot be used inside evaluation expressions)  
The echo syntax doesn't require parenthesis unlike general function calls.

```csharp
echo ("Hello World");
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
The first echo inside the method writes directly to `/dev/tty` in unix. because shell environments use standard-output to return a value. (i.e. by redirecting the output of a command/function to a variable or another command.)

## API and Class Library
ShellScript provide some API methods to minify the need to write platform-specific code for each platform.  
These methods try to use the target shell's dedicated way to get the results but in cases they will generate meta functions inside the output script file. (meta codes are at the beginning of the file)


[Enter the Class Library documentation here](https://github.com/amkherad/ShellScript/blob/master/docs/ClassLibrary.md)


## Contributing

Please read [Contributing.md](https://github.com/amkherad/ShellScript/blob/master/docs/Contributing.md) for details on our code of conduct, and the process for submitting pull requests to us.

## Authors

* **Ali Mousavi Kherad** - *Initial work*

See also the list of [contributors](https://github.com/amkherad/ShellScript/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [https://opensource.org/licenses/MIT](https://opensource.org/licenses/MIT) for details