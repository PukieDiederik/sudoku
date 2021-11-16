# Sudoku solver in C#
this is a simple sudoku solver made in C# that allows you to input a sudoku and it will give you the solved sudoku back.

## Installation
1. clone the git repository

```
git clone https://github.com/PukieDiederik/sudoku.git
```
2. install the dotnet sdk
3. install nuget (dotnet package manager)
4. install CommandLineParser
```
NuGet Install CommandLineParser
```
5. run the program
```
dotnet run Program.cs -- [options]
```

## how to use
The sudoku has to be flattened as a 81 character long number (where you go from top to bottom left to right) and empty cells have to be 0. for example
```
497 200 000
100 400 005
000 016 098
  
620 300 040
300 900 000
001 072 600
  
002 005 870
000 600 004
530 097 061
```
A sudoku like this would become:
```
497200000100400005000016098620300040300900000001072600002005870000600004530097061
```

You will either need to supply a file using `-f [path to file]` or a single sudoku using `-s [sudoku]`. Each sudoku in the file will need to be on a new line. If you are using the file option you can add a max amount of sudokus it needs to process by using `-m 100`. It is capped by default at 100.

**examples**
```
dotnet run Program.cs -- -s 497200000100400005000016098620300040300900000001072600002005870000600004530097061
```
```
dotnet run Program.cs -- -f My/Path/To/Sudoku -m 100
```