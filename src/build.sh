#!/bin/bash
dotnet restore
for path in */project.json; do
    dirname="$(dirname "${path}")"
    dotnet build ${dirname} -c Release
done

for path in Sprache.Tests/project.json; do
    dirname="$(dirname "${path}")"
    dotnet build ${dirname} -f netcoreapp1.0 -c Release
    dotnet test ${dirname} -f netcoreapp1.0  -c Release
done

for path in TinyTemplates.Tests/project.json; do
    dirname="$(dirname "${path}")"
    dotnet build ${dirname} -f netcoreapp1.0 -c Release 
done
