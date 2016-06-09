"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" ../FlatFiles.sln /p:Configuration=Release /p:VisualStudioVersion=14
nuget pack ../FlatFiles/FlatFiles.csproj -Prop Configuration=Release
nuget push *.nupkg -Source nuget.org
del *.nupkg