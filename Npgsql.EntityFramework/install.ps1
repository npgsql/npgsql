param($installPath, $toolsPath, $package, $project)

Add-EFProvider $project 'Npgsql' 'Npgsql.NpgsqlServices, Npgsql.EntityFramework'
