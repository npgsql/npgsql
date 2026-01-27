#!/bin/sh
set -e

cp /certs/* "$PGDATA/"
chmod 600 "$PGDATA/server.key"

cat >> "$PGDATA/postgresql.conf" <<'EOF'
ssl = on
ssl_ca_file = 'ca.crt'
ssl_cert_file = 'server.crt'
ssl_key_file = 'server.key'
password_encryption = scram-sha-256
wal_level = logical
max_wal_senders = 50
logical_decoding_work_mem = 64kB
wal_sender_timeout = 3s
synchronous_standby_names = 'npgsql_test_sync_standby'
synchronous_commit = local
max_prepared_transactions = 100
max_connections = 500
unix_socket_directories = '/tmp,@/npgsql_unix'
EOF

cat > "$PGDATA/pg_hba.conf" <<'EOF'
local all all trust
host all npgsql_tests_scram all scram-sha-256
hostssl all npgsql_tests_ssl all md5
hostnossl all npgsql_tests_ssl all reject
hostnossl all npgsql_tests_nossl all md5
hostssl all npgsql_tests_nossl all reject
host all all all md5
host replication all all md5
EOF

psql -U "$POSTGRES_USER" -d "$POSTGRES_DB" <<'SQL'
SET password_encryption = 'md5';
CREATE USER npgsql_tests SUPERUSER PASSWORD 'npgsql_tests';
CREATE USER npgsql_tests_ssl SUPERUSER PASSWORD 'npgsql_tests_ssl';
CREATE USER npgsql_tests_nossl SUPERUSER PASSWORD 'npgsql_tests_nossl';
SET password_encryption = 'scram-sha-256';
CREATE USER npgsql_tests_scram SUPERUSER PASSWORD 'npgsql_tests_scram';
SELECT 'CREATE DATABASE npgsql_tests OWNER npgsql_tests'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'npgsql_tests') \gexec
SQL

if psql -U "$POSTGRES_USER" -d "$POSTGRES_DB" -tAc "SELECT 1 FROM pg_available_extensions WHERE name='postgis'" | grep -q 1; then
  psql -U "$POSTGRES_USER" -d "$POSTGRES_DB" -c "CREATE EXTENSION IF NOT EXISTS postgis;"
fi

