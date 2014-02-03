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

echo Deleting database %NPGSQL_DB%...
%PSQL% -U %NPGSQL_UID% -h %NPGSQL_HOST% -d %NPGSQL_template_DB% -c "drop database %NPGSQL_DB% ;" >> %NPGSQL_TESTS_LOG% 2>&1
if not errorlevel 1 (echo OK) else (echo FAILED && exit /b 1)
