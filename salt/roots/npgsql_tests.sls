postgresql-repo:
  pkgrepo.managed:
    - name: deb http://apt.postgresql.org/pub/repos/apt/ trusty-pgdg main
    - dist: trusty-pgdg
    - file: /etc/apt/sources.list.d/postgresql.list
    - key_url: https://www.postgresql.org/media/keys/ACCC4CF8.asc

postgresql-9.4:
  pkg.installed

postgresql:
  service.running:
    - watch:
      - file: /etc/postgresql/9.4/main/postgresql.conf
      - file: /etc/postgresql/9.4/main/pg_hba.conf

npgsql_tests:
  postgres_user.present:
    - password: npgsql_tests
    - superuser: True
  postgres_database.present:
    - owner: npgsql_tests

/etc/postgresql/9.4/main/postgresql.conf:
  file.append:
    - text: listen_addresses = '*'

/etc/postgresql/9.4/main/pg_hba.conf:
  file.append:
    - text: host all npgsql_tests 0.0.0.0/0 md5
