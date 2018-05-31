# COPY

PostgreSQL has a feature allowing efficient bulk import or export of data to and from a table. This is usually a much faster way of getting data in and out of a table than using INSERT and SELECT. See documentation for the [COPY command](http://www.postgresql.org/docs/current/static/sql-copy.html) for more details.

Npgsql supports three COPY operation modes: binary, text and raw binary.

## Binary COPY

This mode uses the efficient PostgreSQL binary format to transfer data in and out of the database. The user uses an API to read and write rows and fields, which Npgsql decodes and encodes. When you've finished, you must call `Complete()` to save the data; not doing so will cause the COPY operation to be rolled back when the writer is disposed (this behavior is important in case an exception is thrown).

*IMPORTANT*: Note that it is the your responsibility to read and write the correct type! If you use COPY to write an int32 into a string field you may get an exception, or worse, silent data corruption. It is also highly recommended to use the overload of `Write()` which accepts an `NpgsqlDbType`, allowing you to unambiguously specify exactly what type you want to write. Test your code throroughly.

```c#
// Import two columns to table data
using (var writer = conn.BeginBinaryImport("COPY data (field_text, field_int2) FROM STDIN (FORMAT BINARY)"))
{
    writer.StartRow();
    writer.Write("Hello");
    writer.Write(8, NpgsqlDbType.Smallint);

    writer.StartRow();
    writer.Write("Goodbye");
    writer.WriteNull();

    writer.Complete();
}

// Export two columns to table data
using (var reader = Conn.BeginBinaryExport("COPY data (field_text, field_int2) TO STDOUT (FORMAT BINARY)"))
{
    reader.StartRow();
    Console.WriteLine(reader.Read<string>());
    Console.WriteLine(reader.Read<int>(NpgsqlDbType.Smallint));

    reader.StartRow();
    reader.Skip();
    Console.WriteLine(reader.IsNull);   // Null check doesn't consume the column
    Console.WriteLine(reader.Read<int>());

    reader.StartRow();    // Last StartRow() returns -1 to indicate end of data
}
```


## Text COPY

This mode uses the PostgreSQL text or csv format to transfer data in and out of the database. It is the user's responsibility to format the text or CSV appropriately, Npgsql simply provides a TextReader or Writer. This mode is less efficient than binary copy, and is suitable mainly if you already have the data in a CSV or compatible text format and don't care about performance.

```c#
using (var writer = conn.BeginTextImport("COPY data (field_text, field_int4) FROM STDIN")) {
    writer.Write("HELLO\t1\n");
    writer.Write("GOODBYE\t2\n");
}

using (var reader = conn.BeginTextExport("COPY data (field_text, field_int4) TO STDOUT")) {
    Console.WriteLine(reader.ReadLine());
    Console.WriteLine(reader.ReadLine());
}
```

## Raw Binary COPY

In this mode, data transfer is binary, but Npgsql does no encoding or decoding whatsoever - data is exposed as a raw .NET Stream. This mode makes sense only for bulk data and restore a table: the table is saved as a blob, which can later be restored. If you need to actually make sense of the data, you should be using regular binary mode instead (not raw).

Example:

```c#
int len;
var data = new byte[10000];
// Export table1 to data array
using (var inStream = conn.BeginRawBinaryCopy("COPY table1 TO STDOUT (FORMAT BINARY)")) {
    // We assume the data will fit in 10000 bytes, in real usage you would read repeatedly, writine to a file.
    len = inStream.Read(data, 0, data.Length);
}

// Import data array into table2
using (var outStream = conn.BeginRawBinaryCopy("COPY table2 FROM STDIN (FORMAT BINARY)")) {
    outStream.Write(data, 0, len);
}
```

## Cancel

Import operations can be cancelled at any time by calling the `Cancel()` method on the importer object. No data is committed to the database before the importer is closed or disposed.

Export operations can be cancelled as well, also by calling `Cancel()`.

## Other

See the CopyTests.cs test fixture for more usage samples.
