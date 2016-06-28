&dotnet pack "..\src\FlatFiles\project.json" --configuration Release --output .

Remove-Item FlatFiles.*.symbols.nupkg

.\NuGet.exe push FlatFiles.*.nupkg -Source nuget.org

Remove-Item FlatFiles.*.nupkg