&dotnet pack "..\src\FlatFiles\project.json" --configuration Release --output .

Remove-Item FlatFiles.*.symbols.nupkg

.\NuGet.exe push FlatFiles.*.nupkg -Source https://www.nuget.org/api/v2/package

Remove-Item FlatFiles.*.nupkg