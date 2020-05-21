#-------------------------------------------------------------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See https://go.microsoft.com/fwlink/?linkid=2090316 for license information.
#-------------------------------------------------------------------------------------------------------------

FROM postgres:12

ARG POSTGRESQL_MAJOR_VERSION=12
# ARG POSTGIS_VERSION=3

# Avoid warnings by switching to noninteractive
#ENV DEBIAN_FRONTEND=noninteractive
#USER postgres
# Configure PostgreSQL for tests
RUN /etc/init.d/postgresql start \
    && psql -c "CREATE USER npgsql_tests SUPERUSER PASSWORD 'npgsql_tests'" \
    && psql -c "CREATE DATABASE npgsql_tests OWNER npgsql_tests" \
    && export PGDATA=/etc/postgresql/$POSTGRESQL_MAJOR_VERSION/main \
    && sed -i 's/#ssl = off/ssl = on/' $PGDATA/postgresql.conf \
    && sed -i 's/#max_prepared_transactions = 0/max_prepared_transactions = 10/' $PGDATA/postgresql.conf \
    # Disable trust authentication, requiring MD5 passwords - some tests must fail if a password isn't provided.
    && sh -c "echo 'local all all trust' > $PGDATA/pg_hba.conf" \
    && sh -c "echo 'host all all all md5' >> $PGDATA/pg_hba.conf" \
    && /etc/init.d/postgresql restart \
    #
    # Clean up
    # && apt-get autoremove -y \
    # && apt-get clean -y \
    # && rm -rf /var/lib/apt/lists/*
# Switch back to dialog for any ad-hoc use of apt-get
# ENV DEBIAN_FRONTEND=dialog
