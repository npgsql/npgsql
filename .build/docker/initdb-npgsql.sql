-- Standard test account for Npgsql
CREATE ROLE npgsql_tests SUPERUSER LOGIN PASSWORD 'npgsql_tests';
CREATE DATABASE npgsql_tests OWNER npgsql_tests;

-- Domain account for Azure Pipelines.
CREATE ROLE vsts SUPERUSER LOGIN;
CREATE DATABASE vsts OWNER vsts;
