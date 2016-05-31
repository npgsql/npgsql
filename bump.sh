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

  sed -i 's/^\(\s*\)<version>[^<]*<\/version>/\1<version>$version$<\/version>/' src/Npgsql/Npgsql.nuspec

  sed -i 's/AssemblyVersion("[^"]*")/AssemblyVersion("'$without_prerelease'")/' src/CommonAssemblyInfo.cs
  sed -i 's/AssemblyFileVersion("[^"]*")/AssemblyFileVersion("'$without_prerelease'")/' src/CommonAssemblyInfo.cs
  sed -i 's/AssemblyInformationalVersion("[^"]*")/AssemblyInformationalVersion("'$v'")/' src/CommonAssemblyInfo.cs
else
  # Release version

  sed -i 's/^\(\s*\)<version>[^<]*<\/version>/\1<version>'$v'<\/version>/' src/Npgsql/Npgsql.nuspec

  sed -i 's/AssemblyVersion("[^"]*")/AssemblyVersion("'$v'")/' src/CommonAssemblyInfo.cs
  sed -i 's/AssemblyFileVersion("[^"]*")/AssemblyFileVersion("'$v'")/' src/CommonAssemblyInfo.cs
  sed -i 's/AssemblyInformationalVersion("[^"]*")/AssemblyInformationalVersion("'$v'")/' src/CommonAssemblyInfo.cs
fi

git add teamcity_set_version.cmd
git add src/Npgsql/Npgsql.nuspec
git add src/CommonAssemblyInfo.cs

git commit -m "Bump version to $v"
