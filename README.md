# SiiDTSCodeManagementPCH
Sii Test Project

## Build without Visual studio

1. Download standalone nuget manager nuget.exe from https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
2. Update PATH environment variable with a folder name where nuget.exe has been saved or place nuget.exe in project folder
3. Check if MSBuild executable file path in each of the build cmd files is valid and fix them when necessary with path to MSBuild in version at least v14
4. From cmd window in project folder path run one of the build scripts

## Basic test after build

1. From `bin\debug` or `bin\release` folder open new cmd window
2. Execute cmd `.\SiiDTSCodeManagementPCH.exe ..\..\test\input.txt`
3. Program ends 10 seconds after processing is finished

