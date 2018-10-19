# NpgsqlDataReader.cs

This file had two conflicts that were solved manually because mixed changes on both sides.

## Manually solved conflicts

### 1. Replication check when reading at target line 403

We want to keep the target if operation here, except that the source code had a commit to reduce async layer when reading messages. So we end mixing the two changes into a single one here.

Target code is:

```csharp
if (_connector.Settings.ReplicationMode == ReplicationMode.None)
{
    await _connector.ReadExpecting<ParseCompleteMessage>(async);
    await _connector.ReadExpecting<BindCompleteMessage>(async);
}
else
    Debug.Assert(pStatement == null);

var msg = await _connector.ReadMessage(async);

```

Source code is:

```csharp
Expect<ParseCompleteMessage>(await Connector.ReadMessage(async));
Expect<BindCompleteMessage>(await Connector.ReadMessage(async));
msg = await Connector.ReadMessage(async);
```

Finally, merged code is:

```csharp
if (_connector.Settings.ReplicationMode == ReplicationMode.None)
{
    Expect<ParseCompleteMessage>(await Connector.ReadMessage(async));
    Expect<BindCompleteMessage>(await Connector.ReadMessage(async));
}
else
    Debug.Assert(pStatement == null);

msg = await Connector.ReadMessage(async);
```

### 2. Handling of new `BackendMessageCode.CompletedResponse` at target line 421

As `_rowDescription` now is a property called `RowDescription`, a manual change was required here as well.

## Automatic solved conflicts (differences)

### 1. Change of if constant checks (`#if NET451`, `#if NETCOREAPP1_0`, etc.)

Those checks were removed through a series of updates for new frameworks thata happened since the PR was created.

- ["Update NpgsqlDataReader to support GetColumnSchema() on .NET Standard 2.0 (#1767)"](https://github.com/npgsql/npgsql/commit/2f47a0df82fc3de5001748aeb103f14590143a46)
- ["Update target frameworks for `net452` and `netstandard2.0` (#2131)"](https://github.com/npgsql/npgsql/commit/91d23f90ef00eadc7c07966833959a5b3f877127)

### 2. New properties, accessibility and warning adjustments

Those were added in a series of commits as well, and we are taking every change from source.

- ["Tons of work rewriting read and the type handling system"](https://github.com/npgsql/npgsql/commit/827ddb275df592655ac16e1436ce60663698afdf)
- ["Optimized read when next row is in memory"](https://github.com/npgsql/npgsql/commit/2a6fec1c29c278378a7eb946c6547f8669d36037)
- ["Recycle NpgsqlDataReader instances"](https://github.com/npgsql/npgsql/commit/1768aa73a0861f9df2c0e031a5bc789fcfd78613)
- ["Big refactor of NpgsqlDataReader"](https://github.com/npgsql/npgsql/commit/dc5b97d9fe17ca9ef03ce10da19554df1233d0fa)
- ["Bulk Copy Return Rows (#2150)"](https://github.com/npgsql/npgsql/commit/f142df0b840e473e3ddc6f6546a9e879f2587b41)
- ["Refactored NpgsqlDataReader.GetStream()"](https://github.com/npgsql/npgsql/commit/3a0c856cee5cc15fa75d0ef4ab282f1d6d94b7dc)
- ["Fix issue with multiple column streams"](https://github.com/npgsql/npgsql/commit/b7f099705a777ffe127ada863b9f15e3722acc1b)

### 3. Temporary char buffer (`char[] _tempCharBuf;`)

Added in commit ["Refactor char decoding out of read buffer"](https://github.com/npgsql/npgsql/commit/183be594fafb388c984411f8e6f61ac1a3870596). Since it is currently kept on dev branch, we are taking this as well.

### 4. New internal constructor (`internal NpgsqlDataReader(NpgsqlConnector connector)`)

It was changed on commit ["Recycle NpgsqlDataReader instances"](https://github.com/npgsql/npgsql/commit/1768aa73a0861f9df2c0e031a5bc789fcfd78613). The actual constructor was changed to a void method called `Init`.

### 5. Removal of `PopulateOutputParameters` method

This method was moved inside a new variation of a reader, the `NpgsqlDefaultDataReader`. This reader was added with the commit ["Big refactor of NpgsqlDataReader"](https://github.com/npgsql/npgsql/commit/dc5b97d9fe17ca9ef03ce10da19554df1233d0fa#diff-affd37448ad8d85d7ec54c653f50684d), in the `NpgsqlDefaultDataReader.cs` file.

### 6. Read and Get routine changes, new Get methods, async Get methods

Those were made because of a series of small refactors and optimizations and taken from the source.

- ["Optimized read when next row is in memory"](https://github.com/npgsql/npgsql/commit/2a6fec1c29c278378a7eb946c6547f8669d36037)
- ["Small refactor of NpgsqlDataReader.Read()"](https://github.com/npgsql/npgsql/commit/8745e3b78828f2220c9e6c7cdd5563f83c7d080f)
- ["Fix reader consume for auto-prepared statements"](https://github.com/npgsql/npgsql/commit/a73d903048fd0d65bb46de4a096f9ef1d435a2e5)
- ["Big refactor of NpgsqlDataReader"](https://github.com/npgsql/npgsql/commit/dc5b97d9fe17ca9ef03ce10da19554df1233d0fa)

### 7. Removal of `IsDBNull` method

Same as #5, moved to `NpgsqlDefaultDataReader` on ["Big refactor of NpgsqlDataReader"](https://github.com/npgsql/npgsql/commit/dc5b97d9fe17ca9ef03ce10da19554df1233d0fa#diff-affd37448ad8d85d7ec54c653f50684d).

### 8. Changes on `GetDataTypeName` method

Mostly based of changes to data type name handling on commit ["Numerous fixes to PG type name handling (#1945)"](https://github.com/npgsql/npgsql/commit/fc1e183103ac6246bcb5d7ceacbf509e18248583).

### 9. `GetSchemaTable` changes

They came with the commit ["Reimplement NpgsqlDataReader.GetSchemaTable()"](https://github.com/npgsql/npgsql/commit/65a495bf7d621779495f7ed8452cc160d01ecc57), and just taken from source.

### 10. `ReadResult` enum removal

It was removed on commit ["Further simplify message processing in reader"](https://github.com/npgsql/npgsql/commit/cd787c49764c1c9eeacd3f234b61e61a6a3ffde8#diff-922404367f708ad04718b3837fa6168d).