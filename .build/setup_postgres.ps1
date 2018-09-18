powershell -c "Invoke-Expression ((New-Object Net.WebClient).DownloadString('https://s3.amazonaws.com/pgcentral/install.ps1'))";

# Install PostgreSQL
bigsql/pgc install pg10

# Install PostGIS
bigsql/pgc install postgis24-pg10

# Start PostgreSQL
bigsql/pg10/bin/initdb -D PGDATA -E UTF8 -U postgres
bigsql/pg10/bin/pg_ctl register -D PGDATA -N postgresql -S demand
net start postgresql

# Configure test account
bigsql/pg10/bin/psql -U postgres -c "CREATE USER npgsql_tests WITH PASSWORD 'npgsql_tests' SUPERUSER";

# Create database
bigsql/pg10/bin/psql -U postgres -c "CREATE DATABASE npgsql_tests OWNER npgsql_tests";
bigsql/pg10/bin/psql -U postgres -c "CREATE EXTENSION postgis" npgsql_tests;

