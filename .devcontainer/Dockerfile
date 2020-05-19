#-------------------------------------------------------------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See https://go.microsoft.com/fwlink/?linkid=2090316 for license information.
#-------------------------------------------------------------------------------------------------------------

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal

ARG POSTGRESQL_MAJOR_VERSION=12
ARG POSTGIS_VERSION=3

# Avoid warnings by switching to noninteractive
ENV DEBIAN_FRONTEND=noninteractive

# Configure apt and install packages
RUN apt-get update \
    && apt-get -y install --no-install-recommends apt-utils dialog 2>&1 \
    #
    # Verify git, process tools, lsb-release (common in install instructions for CLIs) installed
    && apt-get -y install git openssh-client less iproute2 procps apt-transport-https gnupg2 curl lsb-release \
    #
    # Install PostgreSQL
    && wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | sudo apt-key add - \
    && sudo sh -c 'echo "deb http://apt.postgresql.org/pub/repos/apt/ focal-pgdg main" >> /etc/apt/sources.list.d/pgdg.list' \
    && sudo apt-get update -qq \
    && sudo apt-get install -qq postgresql-$POSTGRESQL_MAJOR_VERSION postgresql-POSTGRESQL_MAJOR_VERSION-postgis-$POSTGIS_VERSION \
    && sudo -u postgres psql -c "CREATE USER npgsql_tests SUPERUSER PASSWORD 'npgsql_tests'" \
    && sudo -u postgres psql -c "CREATE DATABASE npgsql_tests OWNER npgsql_tests" \
    #
    && export PGDATA=/etc/postgresql/$POSTGRESQL_MAJOR_VERSION/main \
    && sudo sed -i 's/#ssl = off/ssl = on/' $PGDATA/postgresql.conf \
    && sudo sed -i 's/#max_prepared_transactions = 0/max_prepared_transactions = 10/' $PGDATA/postgresql.conf \
    # Disable trust authentication, requiring MD5 passwords - some tests must fail if a password isn't provided.
    && sudo sh -c "echo 'local all all trust' > $PGDATA/pg_hba.conf" \
    && sudo sh -c "echo 'host all all all md5' >> $PGDATA/pg_hba.conf" \
    && sudo pg_ctlcluster $POSTGRESQL_MAJOR_VERSION main restart \
    #
    # Clean up
    && apt-get autoremove -y \
    && apt-get clean -y \
    && rm -rf /var/lib/apt/lists/*

# Switch back to dialog for any ad-hoc use of apt-get
ENV DEBIAN_FRONTEND=dialog
