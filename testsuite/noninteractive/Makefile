#
# Makefile
#
# Copyright (C) 2002 The Npgsql Development Team
# npgsql-general@gborg.postgresql.org
# http://gborg.postgresql.org/project/npgsql/projdisplay.php
#
# This library is free software; you can redistribute it and/or
# modify it under the terms of the GNU Lesser General Public
# License as published by the Free Software Foundation; either
# version 2.1 of the License, or (at your option) any later version.
# 
# This library is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
# Lesser General Public License for more details.
# 
# You should have received a copy of the GNU Lesser General Public
# License along with this library; if not, write to the Free Software
# Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
NPGSQL_LIB=Npgsql.dll
MONO=mono
CC=mcs
CPPFLAGS=-r $(NPGSQL_LIB) -r System.Data.dll
OBJECTS=
PSQL=psql
NPGSQL_template_DB=template1
NPGSQL_DB_PREFIX=npgsql_tests
NPGSQL_DB:=$(NPGSQL_DB_PREFIX)_$(shell date +%Y%m%d_%H%M%S)

ifndef $(NPGSQL_HOST)
	NPGSQL_HOST=127.0.0.1
endif

NPGSQL_PORT=5432
NPGSQL_UID=npgsql_tests
NPGSQL_PWD=npgsql_tests
NPGSQL_TEST_STRING=-U $(NPGSQL_UID) -h $(NPGSQL_HOST) -d $(NPGSQL_DB)
NPGSQL_TESTS_LOG=npgsql_tests.log
NPGSQL_TESTS_SHARED_SRC=NpgsqlTests.cs
NPGSQL_TESTS_SHARED_LIB=NpgsqlTests.dll
NPGSQL_TEST_STATUS_FILE=npgsql_tests_status

export CC CPPFLAGS NPGSQL_template_DB NPGSQL_TEST_STRING NPGSQL_TESTS_LOG \
	NPGSQL_DB_PREFIX NPGSQL_DB NPGSQL_HOST NPGSQL_UID NPGSQL_PWD PSQL \
	NPGSQL_TESTS_SHARED_SRC NPGSQL_TESTS_SHARED_LIB NPGSQL_TEST_STATUS_FILE MONO

export PGPASSWORD=$(NPGSQL_PWD)
export PGUSER=$(NPGSQL_UID)

all: compile_lib links db tests

db:
	@echo "==========================================================" && \
	echo "Make sure you have a user named $(NPGSQL_UID) with password $(NPGSQL_PWD) with the privilege to create new databases. Such a user could be created by issuing the SQL command: create user $(NPGSQL_UID) with password '$(NPGSQL_PWD)' createdb ;" && \
	echo "==========================================================="
	@echo -n "Creating test database '$(NPGSQL_DB)'..."
	@($(PSQL) -U $(NPGSQL_UID) -h $(NPGSQL_HOST) -d $(NPGSQL_template_DB) \
	-c "create database $(NPGSQL_DB) ;" > $(NPGSQL_TESTS_LOG) 2>&1 \
	&& echo "OK") || (echo "FAILED" && exit 1)
	@echo -n "Adding test tables..." && ($(PSQL) $(NPGSQL_TEST_STRING) \
	-f add_tables.sql >> $(NPGSQL_TESTS_LOG) 2>&1 && echo "OK") || \
	(echo "FAILED" && exit 1)
	@echo -n "Adding test functions..." && ($(PSQL) $(NPGSQL_TEST_STRING) \
	-f add_functions.sql >> $(NPGSQL_TESTS_LOG) 2>&1 && echo "OK") || \
	(echo "FAILED" && exit 1)
	@echo -n "Adding test triggers..." && ($(PSQL) $(NPGSQL_TEST_STRING) \
	-f add_triggers.sql >> $(NPGSQL_TESTS_LOG) 2>&1 && echo "OK") || \
	(echo "FAILED" && exit 1)
	@echo -n "Adding test views..." && ($(PSQL) $(NPGSQL_TEST_STRING) \
	-f add_views.sql >> $(NPGSQL_TESTS_LOG) 2>&1 && echo "OK") || \
	(echo "FAILED" && exit 1)
	@echo -n "Adding test data..." && ($(PSQL) $(NPGSQL_TEST_STRING) \
	-f add_data.sql >> $(NPGSQL_TESTS_LOG) 2>&1 && echo "OK") || \
	(echo "FAILED" && exit 1)

compile_lib:
	(cd ../../Npgsql; make)

links:
	@test -L $(NPGSQL_LIB) || ln -s ../../Npgsql/build/mono/$(NPGSQL_LIB) $(NPGSQL_LIB)

tests:
	@./runTests.sh 2>&1 | tee -a ${NPGSQL_TESTS_LOG}
	@grep OK ${NPGSQL_TEST_STATUS_FILE} || exit 1

clean:
	rm -rf *~ out_* *.exe $(NPGSQL_LIB) $(NPGSQL_TESTS_LOG) \
	$(NPGSQL_TESTS_SHARED_SRC) \
	$(NPGSQL_TESTS_SHARED_LIB)
	@./cleanDBs.sh
