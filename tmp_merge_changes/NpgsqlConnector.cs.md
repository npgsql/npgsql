# NpgsqlConnector.cs

Two conflicts had to be manually addressed here. One of them was a property name change (breaking change), and the creation of internal fields to manage replictation server messages.

## Manually solved conflicts

### 1. Change of `CurrentCopyOperation` property name to `CurrentCancelableOperation` (target line 156)

This change happened because of the replication support. This property used to hold a reference to a importer/exporter object when the connector is on COPY mode, but now holds the same reference when it is on REPLICATION mode as well (XML docs also updated for this change). This will be a breaking change, and may need subsequent work after the merge. The new property name was choosed as it looks more suitable.

### 2. Creation of `_copyBothResponseMessage` field

This field was added as a part of the replication process handling. It is used to parse server messages with `BackendMessageCode.CopyBothResponse`.

## Automatic solved conflicts (differences)

### 1. Support for SSL/TLS callbacks on conections

They are taken and added with the commit ["Fix cancel with SSL/TLS"](https://github.com/npgsql/npgsql/commit/6d7fd2ba918a287e73dca3680b80ce18ab389218).

### 2. `ReadBuffer`/`WriteBuffer` rename

They were renamed to `NpqsqlReadBuffer` and `NpgsqlWriteBuffer` on ["Big refactor of type mapping system"](https://github.com/npgsql/npgsql/commit/4a503c7000dae25cf2ddc860d25d2db93539c0d0)

### 3. Removal of `ServerVersion` property

This property was removed on ["Refactor DatabaseInfo and type loading"](https://github.com/npgsql/npgsql/commit/12e39c233b7b77645b568efd2294a87a338353e9#diff-dbae64e4e10e0d9c3e6dd5bbbc2aeb45). It was moved to a new structure called `NpgsqlDatabaseInfo`. This structure is now used as a property called `DatabaseInfo` inside the connector.

### 4. `TypeMapper` property

Added in commit ["Big refactor of type mapping system"](https://github.com/npgsql/npgsql/commit/4a503c7000dae25cf2ddc860d25d2db93539c0d0#diff-dbae64e4e10e0d9c3e6dd5bbbc2aeb45) and taken from source.

### 5. `PostgresParameters` property

It was renamed from `BackendParameters` in commit ["Refactor DatabaseInfo and type loading"](https://github.com/npgsql/npgsql/commit/12e39c233b7b77645b568efd2294a87a338353e9)

### 6. Update of `_dataRowSequentialMessage` and `_dataRowNonSequentialMessage`

They were changed to a single field called `_dataRowMessage` on commit ["Big refactor of NpgsqlDataReader"](https://github.com/npgsql/npgsql/commit/dc5b97d9fe17ca9ef03ce10da19554df1233d0fa#diff-dbae64e4e10e0d9c3e6dd5bbbc2aeb45).

"Instead of having a single NpgsqlDataReader and logic spread out across DataRowNonSequentialMessage and DataRowSequentialMessage, we now have NpgsqlDefaultDataReader and NpgsqlSequentialDataReader. DataRow no longer contains any logic."