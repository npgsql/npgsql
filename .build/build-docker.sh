#!/bin/sh
set -e

cp ./docker/initdb-npgsql.sh ./docker/11
cp ./docker/initdb-npgsql.sh ./docker/10
cp ./docker/initdb-npgsql.sh ./docker/9.6

cp ./docker/server.crt ./docker/11
cp ./docker/server.crt ./docker/10
cp ./docker/server.crt ./docker/9.6

cp ./docker/server.key ./docker/11
cp ./docker/server.key ./docker/10
cp ./docker/server.key ./docker/9.6

docker build -t austindrenski/npgsql-postgres:11  ./docker/11
docker build -t austindrenski/npgsql-postgres:10  ./docker/10
docker build -t austindrenski/npgsql-postgres:9.6 ./docker/9.6
