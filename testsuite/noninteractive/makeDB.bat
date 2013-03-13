@echo off


set PSQL=psql
set NPGSQL_HOST=127.0.0.1
set NPGSQL_PORT=5432
set NPGSQL_UID=npgsql_tests
set NPGSQL_PWD=npgsql_tests
set NPGSQL_template_DB=template1
set NPGSQL_DB=npgsql_tests
set NPGSQL_TESTS_LOG=npgsql_tests.log
set PGPASSWORD=%NPGSQL_PWD%
set PGUSER=%NPGSQL_UID%
set NPGSQL_TEST_STRING=-U %NPGSQL_UID% -h %NPGSQL_HOST% -d %NPGSQL_DB%


echo ==========================================================
echo Make sure you have a user named %NPGSQL_UID% with password %NPGSQL_PWD% with the privilege to create new databases. Such a user could be created by issuing the SQL command: 
echo     create user %NPGSQL_UID% with password '%NPGSQL_PWD%' createdb ;
echo ===========================================================

echo Creating test database '%NPGSQL_DB%'...
%PSQL% -U %NPGSQL_UID% -h %NPGSQL_HOST% -d %NPGSQL_template_DB% -c "create database %NPGSQL_DB% ;" > %NPGSQL_TESTS_LOG% 2>&1
if not errorlevel 1 (echo OK) else (echo FAILED && exit /b 1)

echo Adding test tables...
%PSQL% %NPGSQL_TEST_STRING% -f add_tables.sql >> %NPGSQL_TESTS_LOG% 2>&1
if not errorlevel 1 (echo OK) else (echo FAILED && exit /b 1)

echo Adding test functions...
%PSQL% %NPGSQL_TEST_STRING% -f add_functions.sql >> %NPGSQL_TESTS_LOG% 2>&1
if not errorlevel 1 (echo OK) else (echo FAILED && exit /b 1)

echo Adding test triggers...
%PSQL% %NPGSQL_TEST_STRING% -f add_triggers.sql >> %NPGSQL_TESTS_LOG% 2>&1
if not errorlevel 1 (echo OK) else (echo FAILED && exit /b 1)

echo Adding test views...
%PSQL% %NPGSQL_TEST_STRING% -f add_views.sql >> %NPGSQL_TESTS_LOG% 2>&1
if not errorlevel 1 (echo OK) else (echo FAILED && exit /b 1)

echo Adding test data...
%PSQL% %NPGSQL_TEST_STRING% -f add_data.sql >> %NPGSQL_TESTS_LOG% 2>&1
if not errorlevel 1 (echo OK) else (echo FAILED && exit /b 1)