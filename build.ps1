rmdir -Recurse -Force .\HaPlugin\bin\
dotnet build --configuration Release
mv .\HaPlugin\bin\metadata\LoupedeckPackage.yaml .\HaPlugin\bin\