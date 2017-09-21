&dotnet pack "..\src\FlatFiles\FlatFiles.csproj" --configuration Release --output $PWD

.\NuGet.exe push FlatFiles.*.nupkg -Source https://www.nuget.org/api/v2/package

Remove-Item FlatFiles.*.nupkg