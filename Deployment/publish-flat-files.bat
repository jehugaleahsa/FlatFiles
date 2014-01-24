msbuild ../FlatFiles.sln /p:Configuration=Release
nuget pack ../FlatFiles/FlatFiles.csproj -Prop Configuration=Release
nuget push *.nupkg
del *.nupkg