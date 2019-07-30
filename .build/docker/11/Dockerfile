FROM postgres:11
LABEL description="Npgsql CI build image"

ENV POSTGIS_MAJOR 2.5
ENV POSTGIS_VERSION 2.5.2+dfsg-1~exp1.pgdg90+1

RUN apt-get update \
      && apt-cache showpkg postgresql-$PG_MAJOR-postgis-$POSTGIS_MAJOR \
      && apt-get install -y --no-install-recommends \
           postgresql-$PG_MAJOR-postgis-$POSTGIS_MAJOR=$POSTGIS_VERSION \
           postgresql-$PG_MAJOR-postgis-$POSTGIS_MAJOR-scripts=$POSTGIS_VERSION \
           postgis=$POSTGIS_VERSION \
      && rm -rf /var/lib/apt/lists/*

# Data directory needs to be empty for initialization scripts to run, so stash one level up.
COPY ./server.crt $PGDATA/../server.crt
COPY ./server.key $PGDATA/../server.key
RUN chown root:postgres $PGDATA/../server.key
RUN chmod 0640 $PGDATA/../server.key

RUN mkdir -p /docker-entrypoint-initdb.d
COPY ./initdb-npgsql.sh /docker-entrypoint-initdb.d/initdb-npgsql.sh

CMD ["postgres", "-c", "max_prepared_transactions=10", "-c", "ssl=true", "-c", "ssl_cert_file=../server.crt", "-c", "ssl_key_file=../server.key"]
