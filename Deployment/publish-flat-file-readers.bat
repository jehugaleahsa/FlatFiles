msbuild ../FlatFileReaders.sln /p:Configuration=Release
nuget pack ../FlatFileReaders/FlatFileReaders.csproj -Prop Configuration=Release
nuget push *.nupkg
del *.nupkg