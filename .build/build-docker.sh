#!/bin/sh
set -e

CONTEXT_DIR=$(dirname $0)/docker

sudo docker build -t npgsql/postgres:11  --build-arg PG_VERSION=11  $CONTEXT_DIR
sudo docker build -t npgsql/postgres:10  --build-arg PG_VERSION=10  $CONTEXT_DIR
sudo docker build -t npgsql/postgres:9.6 --build-arg PG_VERSION=9.6 $CONTEXT_DIR

sudo docker push npgsql/postgres:11
sudo docker push npgsql/postgres:10
sudo docker push npgsql/postgres:9.6
