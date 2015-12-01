C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild ../FlatFiles.sln /p:Configuration=Release
nuget pack ../FlatFiles/FlatFiles.csproj -Prop Configuration=Release
nuget pack ../FlatFiles.Mvc/FlatFiles.Mvc.csproj -Prop Configuration=Release
nuget push *.nupkg
del *.nupkg