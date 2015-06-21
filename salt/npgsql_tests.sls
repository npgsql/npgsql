postgresql-repo:
  pkgrepo.managed:
    - name: deb http://apt.postgresql.org/pub/repos/apt/ trusty-pgdg main
    - dist: trusty-pgdg
    - file: /etc/apt/sources.list.d/postgresql.list
    - key_url: https://www.postgresql.org/media/keys/ACCC4CF8.asc

{% for pgsql_version in grains['pgsql_versions'] %}
{% set pgsql_port = 5400 + pgsql_version|replace(".", "")|int %}

postgresql-{{ pgsql_version }}:
  pkg.installed:
    - require_in:
      - service: postgresql

/etc/postgresql/{{ pgsql_version }}/main/postgresql.conf:
  file.append:
    - text:
      - listen_addresses = '*'
      - port = {{ pgsql_port }}
    - watch_in:
      - service: postgresql

npgsql_tests-{{ pgsql_version }}:
  postgres_user.present:
    - name: npgsql_tests
    - password: npgsql_tests
    - superuser: True
    - db_port: {{ pgsql_port }}
    - require:
      - service: postgresql
  postgres_database.present:
    - name: npgsql_tests
    - owner: npgsql_tests
    - db_port: {{ pgsql_port }}
    - require:
      - service: postgresql

/etc/postgresql/{{ pgsql_version }}/main/pg_hba.conf:
  file.append:
    - text: host all npgsql_tests 0.0.0.0/0 md5
    - watch_in:
      - service: postgresql

{% endfor %}

postgresql:
  service.running
