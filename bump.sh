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

echo "echo ##teamcity[buildNumber '$v-%1']" > teamcity_set_version.cmd

if [[ $v == *"-"* ]]; then
  # Prerelease version

  without_prerelease=`echo $v | cut -d- -f1`

  sed -i 's/^\(\s*\)"version": "[^"]*"/\1"version": "'$v'-*"/' src/Npgsql/project.json

  sed -i 's/AssemblyVersion("[^"]*")/AssemblyVersion("'$without_prerelease'")/' src/CommonAssemblyInfo.cs
  sed -i 's/AssemblyFileVersion("[^"]*")/AssemblyFileVersion("'$without_prerelease'")/' src/CommonAssemblyInfo.cs
  sed -i 's/AssemblyInformationalVersion("[^"]*")/AssemblyInformationalVersion("'$v'")/' src/CommonAssemblyInfo.cs

  sed -i 's/\(<Identity .*Version=\)"[^"]*"/\1"'$without_prerelease'"/' src/VSIX/source.extension.vsixmanifest

  sed -i 's/.*ProvideBindingRedirection.*/[assembly: ProvideBindingRedirection(AssemblyName = "Npgsql", NewVersion = "'$without_prerelease'.0", OldVersionLowerBound = "0.0.0.0", OldVersionUpperBound = "'$without_prerelease'.0")]/' src/VSIX/Properties/AssemblyInfo.cs
else
  # Release version

  sed -i 's/^\(\s*\)"version": "[^"]*"/\1"version": "'$v'"/' src/Npgsql/project.json

  sed -i 's/AssemblyVersion("[^"]*")/AssemblyVersion("'$v'")/' src/CommonAssemblyInfo.cs
  sed -i 's/AssemblyFileVersion("[^"]*")/AssemblyFileVersion("'$v'")/' src/CommonAssemblyInfo.cs
  sed -i 's/AssemblyInformationalVersion("[^"]*")/AssemblyInformationalVersion("'$v'")/' src/CommonAssemblyInfo.cs

  sed -i 's/\(<Identity .*Version=\)"[^"]*"/\1"'$v'"/' src/VSIX/source.extension.vsixmanifest
  sed -i 's/.*ProvideBindingRedirection.*/[assembly: ProvideBindingRedirection(AssemblyName = "Npgsql", NewVersion = "'$v'.0", OldVersionLowerBound = "0.0.0.0", OldVersionUpperBound = "'$v'.0")]/' src/VSIX/Properties/AssemblyInfo.cs
fi

git add teamcity_set_version.cmd
git add src/Npgsql/project.json
git add src/CommonAssemblyInfo.cs
git add src/VSIX/source.extension.vsixmanifest
git add src/VSIX/Properties/AssemblyInfo.cs

git commit -m "Bump version to $v"
