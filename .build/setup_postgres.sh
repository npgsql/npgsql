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

# Start PostgreSQL
sudo service postgresql start

# Configure test account
sudo -u postgres psql -c "CREATE USER npgsql_tests WITH PASSWORD 'npgsql_tests' SUPERUSER"
