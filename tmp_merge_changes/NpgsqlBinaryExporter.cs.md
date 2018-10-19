# NpgsqlBinaryExporter.cs

## Manually solved conflicts

### 1. `CancellationRequired` and `_typeHandlerCache`

Those two items were in conflict and need to be kept (both of them). `CancellationRequired` is part of the `ICancelable` interface, implemented on this commit, and `_typeHandlerCache` is part of two commits (["Cache type handlers on COPY import/export"](https://github.com/npgsql/npgsql/commit/544657f0c5f34ee1ab822e31cefb1893a42a577a) and ["General refactor of type handling hierarchy"](https://github.com/npgsql/npgsql/commit/c3dd08409aac99452e5005d38c895b1599c39bbd)).

## Automatic solved conflicts (differences)

### 1. `TypeHandlerRegistry` replacement for `ConnectorTypeMapper`

Happened on commit ["Big refactor of type mapping system"](https://github.com/npgsql/npgsql/commit/4a503c7000dae25cf2ddc860d25d2db93539c0d0).

### 2. Constructor improvements

This happened on commit ["Better exception on COPY API confusion"](https://github.com/npgsql/npgsql/commit/2a5029e2a27cf39c6cd879d3d119158fb21cf22b).

### 3. `DoRead<T>` refactoring

Happened on commit ["General refactor of type handling hierarchy"](https://github.com/npgsql/npgsql/commit/c3dd08409aac99452e5005d38c895b1599c39bbd).