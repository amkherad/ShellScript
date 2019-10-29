# ShellScript API Documentation

## Core

### Array

#### `void` Array.Copy(`any`[] destination, `any`[] source)
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

#### `int` Array.GetLength(`any`[] array)
Returns the length of an array.
```csharp
    int[] array = new int[] { 10, 20 };
    
    echo Array.GetLength(array); //2
```


#### `void` Array.Initialize(`any`[] array)
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

