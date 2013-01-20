nuget pack ../FlatFileReaders/FlatFileReaders.csproj -Prop Configuration=Release -Build
nuget push *.nupkg
del *.nupkg