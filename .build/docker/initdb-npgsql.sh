#!/bin/sh
set -e

# Generate server certificate
echo "Generating $PGDATA/server.crt and $PGDATA/server.key"
openssl req -new -x509 -days 365 -nodes -text -out $PGDATA/server.crt -keyout $PGDATA/server.key -subj '/C=US'
chmod 0600 $PGDATA/server.key
chown postgres $PGDATA/server.key

# Configure PostgreSQL
echo "Setting 'ssl = on' in $PGDATA/postgresql.conf"
sed -i 's/#ssl = off/ssl = on/' $PGDATA/postgresql.conf

echo "Setting 'max_prepared_transactions = 10' in $PGDATA/postgresql.conf"
sed -i 's/#max_prepared_transactions = 0/max_prepared_transactions = 10/' $PGDATA/postgresql.conf

echo "Configuring md5 authentication in $PGDATA/pg_hba.conf"
echo "host all all all md5" > $PGDATA/pg_hba.conf

# Perform all actions as $POSTGRES_USER
export PGUSER="$POSTGRES_USER"

# Standard test account for Npgsql
psql -c "CREATE ROLE npgsql_tests SUPERUSER LOGIN PASSWORD 'npgsql_tests';"
psql -c "CREATE DATABASE npgsql_tests OWNER npgsql_tests;"
