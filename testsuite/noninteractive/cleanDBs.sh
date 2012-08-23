#!/bin/sh

echo "Deleting all npgsql_tests_* databases..."
drop_ok=1
for dbname in `${PSQL} -U ${NPGSQL_UID} -h 127.0.0.1 -d ${NPGSQL_template_DB} -c "select datname from pg_database;" |grep ${NPGSQL_DB_PREFIX}`;
do
    echo -n "Deleting database ${dbname}..." && (${PSQL} -U ${NPGSQL_UID} -h ${NPGSQL_HOST} -d ${NPGSQL_template_DB} -c "drop database ${dbname} ;" > /dev/null 2>&1 && echo "OK" ) || (echo "FAILED" && exit 1);
done