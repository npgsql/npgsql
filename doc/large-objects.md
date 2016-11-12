# Large Objects

The Large Objects feature is a way of storing large files in a PostgreSQL database. Files can normally be stored in bytea columns but there are two downsides; a file can only be 1 GB and the backend buffers the whole file when reading or writing a column, which may use significant amounts of RAM on the backend.

With the Large Objects feature, objects are instead stored in a separate system table in smaller chunks and provides a streaming API for the user. Each object is given an integral identifier that is used for accessing the object, that can, for example, be stored in a user's table containing information about this object.

## Example ##

```c#
// Retrieve a Large Object Manager for this connection
var manager = new NpgsqlLargeObjectManager(Conn);

// Create a new empty file, returning the identifier to later access it
uint oid = manager.Create();

// Reading and writing Large Objects requires the use of a transaction
using (var transaction = Conn.BeginTransaction()) {
	// Open the file for reading and writing
	using (var stream = manager.OpenReadWrite(oid)) {
		var buf = new byte[] { 1, 2, 3 };
		stream.Write(buf, 0, buf.Length);
		stream.Seek(0, System.IO.SeekOrigin.Begin);
		
		var buf2 = new byte[buf.Length];
		stream.Read(buf2, 0, buf2.Length);
		
		// buf2 now contains 1, 2, 3
	}
	// Save the changes to the object
	transaction.Commit();
}
```

## See also ##

See the [PostgreSQL documentation](http://www.postgresql.org/docs/current/static/largeobjects.html) for more information. All functionality are implemented and wrapped in the classes `NpgsqlLargeObjectManager` and `NpgsqlLargeObjectStream` using standard .NET Stream as base class.
