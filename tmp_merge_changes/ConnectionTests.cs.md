# ConnectionTests.cs

This file had automatically solved conflicts using Visual Studio Merge Tool. All of them were resolved by taking source changes.

## Automatic solved conflicts (differences)

### 1. `TestUtil.IgnoreExceptOnBuildServer` changed to `Assert.Ignore`

This change happened on the commit ["Make some tests non-mandatory"](https://github.com/npgsql/npgsql/commit/be68721922a0a22e5465065e8b27eaff06fc41eb#diff-42b1cc5c98139a8b8727bab9b2244b68), so is expected to stay as the source.

### 2. Updates on framework constant checks (`#if NET451`, `#if NETCOREAPP1_0`, etc.)

Those checks were updated/removed through a series of updates for new frameworks thata happened since the PR was created.

- ["Fixes for Linux/CoreCLR"](https://github.com/npgsql/npgsql/commit/b80938bdf357fecc93a331972dbbdb746c4946d9)
- ["Preparations for targeting .NET Standard 2.0"](https://github.com/npgsql/npgsql/commit/36d81a9ef302932dd5f2cac92c2b926a2d76badb)
- ["Remove some old ifdefs"](https://github.com/npgsql/npgsql/commit/f7ba55b1445947ca048b27a4cfcdb8836277469a)
- ["Ported test suite to netcoreapp10"](https://github.com/npgsql/npgsql/commit/498a9876fda16476155ec919ecc66501b39cd238)

Expected to stay as source.

### 3. Travis environment check added

Added in ["Skip tests failing on Travis only"](https://github.com/npgsql/npgsql/commit/5f68d61a493a0e57a883d350135734aba873dff9). Expected to staty as source.

### 4. Non-Parallelizable changes

Tests which were parallelizable and changed to non-parallelizable through attributes happened on ["Upgrade to NUnit 3.7.1"](https://github.com/npgsql/npgsql/commit/f8165a444cac7788a3bb3a6e0c78ef75b128fc7b). Expected to stay as source.

### 5. `"using (TestUtil.SetEnvironmentVariable("PGCLIENTENCODING", "SQL_ASCII"))"`

This change happened via definition of an `EnvironmentVariableResetter`, which would take care of resetting environment variables to their previous state after changing them for test purposes. Before this change, this variable resetting was being done manually. Expected to stay as source.

### 6. Timezone environment variables tests

Just added as was added in source commit ["Added timezone param and envvar"](https://github.com/npgsql/npgsql/commit/ed101fa1c0bb76f9fe87c03ca65dde42ed6b1051).

### 7. Empty ctor test

Added in source commit ["Fix NRE with connection default ctor"](https://github.com/npgsql/npgsql/commit/094d54566f71c163c139e1a917b94ad7acf05e1e). Expected to stay as there.

### 8. Changes to `NoDatabaseDefaultsToUsername` test

New asserts added in source commit ["Fix NpgsqlConnection.Database"](https://github.com/npgsql/npgsql/commit/21865e590dfca3ce9636cb0190b8f50537eb1512). Expected to take those changes from source.

### 9. Test isolation and disposable changes (`using (var conn = OpenConnection(csb))`, etc.)

They were done in commit ["Fix test isolation issue"](https://github.com/npgsql/npgsql/commit/d81d3d517ef618698cc364997803e3ccc09a524f). Expected to take those changes.

### 10. Using `ManualResetEvent` to retrieve `PostgresNotice` result

Commited in ["Make test more resilient"](https://github.com/npgsql/npgsql/commit/9a6ba2d733b89388ccecb0bda9a5007145699837). Taking those changes as expected.

### 11. `NpgsqlConnectionStringBuilder` usage

Many tests started to use this builder for connection strings. It was added on commit ["Some fixes to connection code"](https://github.com/npgsql/npgsql/commit/2540d5cbe4ea1a5ceadbc18cef5af4c149223bf2), that were taken as well.

### 12. Calling `ToString()` on `NpgsqlConnectionStringBuilder` instantiation

This happened after the change on ["Switch to string-only connection strings"](https://github.com/npgsql/npgsql/commit/140564eb13009d4aa590bd66a69dec406ed6b9f1) wich makes NpgsqlConnection only accept plain strings as connection strings.

### 13. `ReloadTypes` test changes

This came with commit ["ReloadTypes() takes effect on other connections"](https://github.com/npgsql/npgsql/commit/e5239056d510613588dd38f5d6d24d152715ddeb) that checks if `ReloadTypes` takes effect on other connections. Taken from source.

### 14. `DatabaseInfoIsShared` test

This test was added on ["Refactor DatabaseInfo and type loading"](https://github.com/npgsql/npgsql/commit/12e39c233b7b77645b568efd2294a87a338353e9) and taken here as well.

### 15. `NoTypeLoading` test

This test was added on ["Added mode where types aren't loaded from database"](https://github.com/npgsql/npgsql/commit/0fcf927dc38f4cbe6a4a66f1673f7c2b6830e33b) and taken here as well.

### 16. TCP support tests

Added on ["Support for TCP keepalive"](https://github.com/npgsql/npgsql/commit/84e00282a1042ce4147a8f5b5c1583877122a3fc) and ["Keepalive using system defaults"](https://github.com/npgsql/npgsql/commit/610068c1b8b0941813127d0871912f1c33945b00). Taken as well.
