# Acquire Postgres Package Manager by BigSQL
powershell -c "Invoke-Expression ((New-Object Net.WebClient).DownloadString('https://s3.amazonaws.com/pgcentral/install.ps1'))";

# Install PostgreSQL
bigsql/pgc list
bigsql/pgc install pg11

# Install PostGIS
bigsql/pgc list --extensions pg11
bigsql/pgc install postgis25-pg11

# Initialize PostgreSQL
bigsql/pg11/bin/initdb -D PGDATA -E UTF8 -U postgres

# Configure PostgreSQL
# TODO: For some reason PostgreSQL doesn't start when these are included?
#echo "max_prepared_transactions = 10" >> PGDATA/postgresql.conf
#echo "ssl = true"                     >> PGDATA/postgresql.conf
#cp .build/server.crt PGDATA/
#cp .build/server.key PGDATA/

# Start PostgreSQL
bigsql/pg11/bin/pg_ctl -D PGDATA -l logfile start

# Configure test account
bigsql/pg11/bin/psql -U postgres -c "CREATE USER vsts"
bigsql/pg11/bin/psql -U postgres -c "CREATE USER npgsql_tests WITH PASSWORD 'npgsql_tests' SUPERUSER"
bigsql/pg11/bin/psql -U postgres -c "CREATE DATABASE npgsql_tests OWNER npgsql_tests"
bigsql/pg11/bin/psql -U postgres -d npgsql_tests -c "CREATE EXTENSION citext"
bigsql/pg11/bin/psql -U postgres -d npgsql_tests -c "CREATE EXTENSION postgis"
