import os

if not os.path.exists('/var/lib/pgadmin/storage'):
    os.mkdir('/var/lib/pgadmin/storage')

if not os.path.exists('/var/lib/pgadmin/storage/pgadmin4_pgadmin.org'):
    os.mkdir('/var/lib/pgadmin/storage/pgadmin4_pgadmin.org')

pgpassfile = open('/var/lib/pgadmin/storage/pgadmin4_pgadmin.org/.pgpass','a+')
pgpassfile.write("db:5432:npgsql_tests:npgsql_tests:npgsql_tests\n")
pgpassfile.close()
os.chmod('/var/lib/pgadmin/storage/pgadmin4_pgadmin.org/.pgpass', 0o600)
