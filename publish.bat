@echo off

rmdir /S /Q docs
dotnet publish -c Release
move best-first-search\bin\Release\netstandard2.0\publish\best-first-search\dist docs
