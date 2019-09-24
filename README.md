# SiiDTSCodeManagementPCH
Sii Test Project

## Build without Visual studio

1. Download standalone nuget manager nuget.exe from https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
2. Update PATH env variable with a folder name where nuget.exe has been saved
3. Check if MSBuild executable file path in each of the build cmd files is valid and fix them when necessary vith at least v14
4. Run one of the build scripts

## Basic test after build

1. From `bin\debug` or `bin\release` folder open new cmd window
2. Execute cmd `.\SiiDTSCodeManagementPCH.exe ..\..\test\input.txt`

