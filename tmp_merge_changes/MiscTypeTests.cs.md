# MiscTypeTests.cs

This file is mostly filled with auto resolved conflicts using Visual Studio Merge Tool, but two of them required manual intervention.

## Manually solved conflicts

### 1. Removal of `Tid` test

This happened on commit ["Move some tests to their own files"](https://github.com/npgsql/npgsql/commit/923b797f3582bb9a5e4e116ee64eec10dc7f3c10), where some tests were moved into separate files. Should be removed from the code to stay as source version.

### 2. `Lsn` test

This test should stay as local change. It was created for a new type definition made on this pull request, `NpgsqlLsn` (Log Sequence Number) type.

## Automatic solved conflicts (differences)

### 1. PG type name handling changes

Those changes were made on the commit ["Numerous fixes to PG type name handling (#1945)"](https://github.com/npgsql/npgsql/commit/fc1e183103ac6246bcb5d7ceacbf509e18248583). Taken from source on any lines.

### 2. `Null` test changes

Those changes happened on a series of commits and were taken from source.

- ["Generic parameter for boxless writing"](https://github.com/npgsql/npgsql/commit/36930473f8fda1fa95464877d80694f3de3e98d8)
- ["Generic parameter accepts null"](https://github.com/npgsql/npgsql/commit/23645f73c2d0eae3f50cbb6cfbab59f9a06079b7)

### 3. `Hstore` test changes

Basically, the commit ["Add TestUtil.EnsureExtension function and fix test where it was missing"](https://github.com/npgsql/npgsql/commit/55c6c1c7e89d00761717a59dd4c8603ae04092be) added tools to reuse code when checking if an extension is installed.

### 4. `Record` test

This test was added with the commit ["Read PostgreSQL records as arrays of objects"](https://github.com/npgsql/npgsql/commit/4004861d2d340c7fc1abde7ee6879e551ecd2756) and is taken from the source.

### 5. `Domain` test

Added with the commit ["Big refactor of type mapping system"](https://github.com/npgsql/npgsql/commit/4a503c7000dae25cf2ddc860d25d2db93539c0d0).

### 6. Unrecognized types test changes

Those changes were part of a big refactor done in commit ["Big refactor of type mapping system"](https://github.com/npgsql/npgsql/commit/4a503c7000dae25cf2ddc860d25d2db93539c0d0).

### 7. Removal of `TsVector` test

This test was moved to a separate file on commit ["Move some tests to their own files"](https://github.com/npgsql/npgsql/commit/923b797f3582bb9a5e4e116ee64eec10dc7f3c10).