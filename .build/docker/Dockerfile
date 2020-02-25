ARG PG_VERSION

FROM postgres:$PG_VERSION
LABEL description="Npgsql PostgreSQL image"

ARG POSTGIS_MAJOR
ENV POSTGIS_MAJOR=$POSTGIS_MAJOR

RUN apt-get update && \
    apt-get upgrade -y && \
    apt-get install -y --no-install-recommends \
        postgresql-$PG_MAJOR-postgis-$POSTGIS_MAJOR \
        postgresql-$PG_MAJOR-postgis-$POSTGIS_MAJOR-scripts \
        postgis && \
    rm -rf /var/lib/apt/lists/*

RUN mkdir -p /docker-entrypoint-initdb.d
COPY ./initdb-npgsql.sh /docker-entrypoint-initdb.d/initdb-npgsql.sh
