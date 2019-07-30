#!/bin/sh
set -e

# Perform all actions as $POSTGRES_USER
export PGUSER="$POSTGRES_USER"

# Standard test account for Npgsql
psql -c "CREATE ROLE npgsql_tests SUPERUSER LOGIN PASSWORD 'npgsql_tests';"
psql -c "CREATE DATABASE npgsql_tests OWNER npgsql_tests;"

# Domain account for Azure Pipelines.
psql -c "CREATE ROLE vsts SUPERUSER LOGIN;"
psql -c "CREATE DATABASE vsts OWNER vsts;"
