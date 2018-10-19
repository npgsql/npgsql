# NpgsqlConnectionStringBuilder

One manually solved conflict here. Merge tool did not recognize `Timezone` and `ReplicationMode` properties as two things to be kept on the final source.

## Manually solved conflicts

### 1. `Timezone` and `ReplicationMode` properties

`Timezone` was added in commit ["Added timezone param and envvar"](https://github.com/npgsql/npgsql/commit/ed101fa1c0bb76f9fe87c03ca65dde42ed6b1051) and stayed like this until actual version. `ReplicationMode` was added as a part of this PR, so needs to stay as is too. Both sides were taken here.

## Automatic solved conflicts (differences)

### 1. Removal and changing of framework define constants (`#if NET451`, etc.)

Those changes were expected as the component upgraded itself for new framework versions. Taking source side here.

### 2. Adding of `PassFile` property

This property was added as a new feature implementation on ["Add PgPassFile connection string parameter"](https://github.com/npgsql/npgsql/commit/7282bbc59e17dc31e54a6fe24c07213fe115206e) with the name `PgPassFile`, and later on changed to `PassFile` on commit ["Minor touchup on pgpass work"](https://github.com/npgsql/npgsql/commit/d6a7960ba463132bce084f0e43966c4447fb1b2c).

### 3. Suport for TCP keep alive

Properties like `TcpKeepAlive`, `TcpKeepAliveTime` and others were added on commits ["Support for TCP keepalive"](https://github.com/npgsql/npgsql/commit/84e00282a1042ce4147a8f5b5c1583877122a3fc) and ["Keepalive using system defaults"](https://github.com/npgsql/npgsql/commit/610068c1b8b0941813127d0871912f1c33945b00). Keeping them as source.

### 4. `LoadTableComposites` Property

Added in commit ["Support for unmapped composites"](https://github.com/npgsql/npgsql/commit/77457649c5c3b5c2460e965e2dce80285c3c6457) and kept as well.

### 5. Obsolete marked properties

They were marked as obsolete in version 3.0 of npgsql.

### 6. `ServerCompatibilityMode.NoLoading` enum item

Added in ["Added mode where types aren't loaded from database"](https://github.com/npgsql/npgsql/commit/0fcf927dc38f4cbe6a4a66f1673f7c2b6830e33b) as a feature to support pseudo-PostgreSQL databases which don't fully implement type catalogs.

### 7. `ReplicationMode` type

Added to support logical replication feature.
