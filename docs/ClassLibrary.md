# ShellScript API Documentation

## Core

### Array

#### `void` Array.Copy(`any[]` destination, `any[]` source)
Copies a source array into destination array.
```csharp
    int[] destination;
    int[] source = new int[] { 10, 20, 30, 40 };
    
    Array.Copy(destination, source);
    
    echo destination[0]; //10
    echo destination[1]; //20
    echo destination[2]; //30
    echo destination[3]; //40
```

#### `int` Array.GetLength(`any[]` array)
Returns the length of an array.
```csharp
    int[] array = new int[] { 10, 20 };
    
    echo Array.GetLength(array); //2
```


#### `void` Array.Initialize(`any[]` array)
Initializes an array with zero length.
```csharp
    int[] array;
    Array.Initialize(array);
```


### Convert


### Locale


### Math

#### `number` Math.Abs(`number` num)
Returns the absolute value of a number
```charp
    echo Math.Abs(-10); //10
```


### Platform


### String


### User


## IO

### File


## Network

### Net

#### `void` Net.Ping()
Forwards the call to native OS's ping command.
```charp 
    Net.Ping();
```


## Linux Bash Additions

The Unix-Bash platform exposes native implementations for the following APIs:

- `Environment.GetVariable`, `Environment.GetCurrentDirectory`, and `Environment.GetHomeDirectory`.
- `Path.Combine`, `Path.GetFileName`, `Path.GetDirectoryName`, and `Path.GetExtension`.
- `String.Contains`, `String.StartsWith`, and `String.EndsWith`.
- `File.IsDirectory` and `File.IsFile`, alongside the existing file permission and existence checks.
- `User.GetUserName`, `User.IsSuperUser`, `Locale.GetCurrentLocale`, and `Net.Ping`.

Bash array operations use native expansions and loops. `Array.GetLength` emits `${#array[@]}`, while sized array initialization emits a collision-safe arithmetic loop and uses the element type's default value.
