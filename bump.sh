#!/bin/bash

if [ "$#" -ne 1 ]; then
  echo "usage: bump.sh <version>"
  exit 1
fi

v=$1
if [[ $v == *"-" ]]; then
  echo "Version must not end with -"
  exit 1
fi

sed -i 's/^version: .*/version: '$v'-{build}/' .appveyor.yml
sed -i 's/<VersionPrefix>[^<]*<\/VersionPrefix>/<VersionPrefix>'$v'<\/VersionPrefix>/' src/Npgsql/Npgsql.csproj
sed -i 's/\(<Identity .*Version=\)"[^"]*"/\1"'$v'"/' src/VSIX/source.extension.vsixmanifest
sed -i 's/.*ProvideBindingRedirection.*/[assembly: ProvideBindingRedirection(AssemblyName = "Npgsql", NewVersion = "'$v'.0", OldVersionLowerBound = "0.0.0.0", OldVersionUpperBound = "'$v'.0")]/' src/VSIX/Properties/AssemblyInfo.cs

git add .appveyor.yml
git add src/Npgsql/Npgsql.csproj
git add src/VSIX/source.extension.vsixmanifest
git add src/VSIX/Properties/AssemblyInfo.cs

git commit -m "Bump version to $v"
