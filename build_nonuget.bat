dotnet publish src/GGDBF.BuildAll/GGDBF.BuildAll.csproj -c debug

if not exist "build\debug" mkdir build\debug
xcopy src\GGDBF.BuildAll\bin\Debug\netstandard2.0\publish build\Debug /Y /q
PAUSE