#!/bin/sh
set -e

CONTEXT_DIR=$(dirname $0)/docker

sudo docker build -t npgsql/postgres:9.6             --build-arg PG_VERSION=9.6 --build-arg POSTGIS_MAJOR=3   $CONTEXT_DIR
sudo docker build -t npgsql/postgres:9.6-postgis-3   --build-arg PG_VERSION=9.6 --build-arg POSTGIS_MAJOR=3   $CONTEXT_DIR
sudo docker build -t npgsql/postgres:9.6-postgis-2.5 --build-arg PG_VERSION=9.6 --build-arg POSTGIS_MAJOR=2.5 $CONTEXT_DIR
sudo docker build -t npgsql/postgres:10              --build-arg PG_VERSION=10  --build-arg POSTGIS_MAJOR=3   $CONTEXT_DIR
sudo docker build -t npgsql/postgres:10-postgis-3    --build-arg PG_VERSION=10  --build-arg POSTGIS_MAJOR=3   $CONTEXT_DIR
sudo docker build -t npgsql/postgres:10-postgis-2.5  --build-arg PG_VERSION=10  --build-arg POSTGIS_MAJOR=2.5 $CONTEXT_DIR
sudo docker build -t npgsql/postgres:11              --build-arg PG_VERSION=11  --build-arg POSTGIS_MAJOR=3   $CONTEXT_DIR
sudo docker build -t npgsql/postgres:11-postgis-3    --build-arg PG_VERSION=11  --build-arg POSTGIS_MAJOR=3   $CONTEXT_DIR
sudo docker build -t npgsql/postgres:11-postgis-2.5  --build-arg PG_VERSION=11  --build-arg POSTGIS_MAJOR=2.5 $CONTEXT_DIR
sudo docker build -t npgsql/postgres:12              --build-arg PG_VERSION=12  --build-arg POSTGIS_MAJOR=3   $CONTEXT_DIR
sudo docker build -t npgsql/postgres:12-postgis-3    --build-arg PG_VERSION=12  --build-arg POSTGIS_MAJOR=3   $CONTEXT_DIR
sudo docker build -t npgsql/postgres:12-postgis-2.5  --build-arg PG_VERSION=12  --build-arg POSTGIS_MAJOR=2.5 $CONTEXT_DIR

sudo docker push npgsql/postgres:9.6
sudo docker push npgsql/postgres:9.6-postgis-3
sudo docker push npgsql/postgres:9.6-postgis-2.5
sudo docker push npgsql/postgres:10
sudo docker push npgsql/postgres:10-postgis-3
sudo docker push npgsql/postgres:10-postgis-2.5
sudo docker push npgsql/postgres:11
sudo docker push npgsql/postgres:11-postgis-3
sudo docker push npgsql/postgres:11-postgis-2.5
sudo docker push npgsql/postgres:12
sudo docker push npgsql/postgres:12-postgis-3
sudo docker push npgsql/postgres:12-postgis-2.5
