#!/bin/bash

# Register PostgreSQL Apt Repository
sudo sh -c 'echo "deb http://apt.postgresql.org/pub/repos/apt/ xenial-pgdg main" >> /etc/apt/sources.list'
wget -O - http://apt.postgresql.org/pub/repos/apt/ACCC4CF8.asc | sudo apt-key add -
sudo apt-get -y update

# Install PostgreSQL
sudo apt-get -y install postgresql-11

# Install PostGIS
sudo apt-get -y install postgresql-11-postgis-2.5
sudo apt-get -y install postgresql-11-postgis-2.5-scripts

# Configure PostgreSQL
echo "max_prepared_transactions = 10" >> /etc/postgresql/11/main/postgresql.conf
echo "ssl = true"                     >> /etc/postgresql/11/main/postgresql.conf
cp .build/server.crt /var/lib/postgresql/11/main/
cp .build/server.key /var/lib/postgresql/11/main/

# Start PostgreSQL
sudo service postgresql start

# Configure test account
sudo -u postgres psql -c "CREATE USER vsts"
sudo -u postgres psql -c "CREATE USER npgsql_tests WITH PASSWORD 'npgsql_tests' SUPERUSER"
sudo -u postgres psql -c "CREATE DATABASE npgsql_tests OWNER npgsql_tests"
sudo -u postgres psql -d npgsql_tests -c "CREATE EXTENSION citext"
sudo -u postgres psql -d npgsql_tests -c "CREATE EXTENSION postgis"
