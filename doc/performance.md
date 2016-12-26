# Performance

## Unix Domain Socket

If you're on Linux and are connecting to a PostgreSQL server on the same machine, you can boost performance by connecting via Unix domain socket rather than via a regular TCP/IP socket. To do this, simply specify the directory of your PostgreSQL sockets in the `Host` connection string parameter - if this parameter starts with a slash, it will be taken to mean a filesystem path.

## Reading Large Values

When reading results from PostgreSQL, Npgsql first writes raw binary data from the network into an internal read buffer, and then parses that data as you call methods such as `NpgsqlDataReader.GetString()`. While this allows for efficient network reads, it's worth thinking about the size of this buffer, which is 8K by default. By default, Npgsql attempts to read each row into the buffer. If the entire row fits in 8K, you'll have optimal performance. However, if a row is bigger than 8K, Npgsql will allocate a temporary one-time buffer for the row, and discard it immediately afterwards; this can create very significant memory churn that will slow down your application (future versions may reuse these large buffers). To avoid this, if you know you're going to be reading 16k rows, you can specify `Read Buffer Size=18000` in your connection string (leaving some margin for protocol overhead), this will ensure that the read buffer is reused and no extra allocation occur.

Another option is to pass `CommandBehavior.SequentialAccess` to `NpgsqlCommand.ExecuteReader()`. Sequential mode means that Npgsql will no longer read entire rows into its buffer, but will rather fill up the buffer as needed, read more data when it's empty. The same 8K read buffer will be used regardless of the row's total size, and Npgsql will take care of the details. However, when in sequential you must read the row's fields in the order in which you specified them; you cannot read the 2nd field and then go back to the 1st field, doing so will generate an exception. Similarly, you cannot read the same field twice - once you've read a field, it has been consumed.

For more information on `CommandBehavior.SequentialAccess`, see [this page](https://msdn.microsoft.com/en-us/library/87z0hy49(v=vs.110).aspx). If you decide to use this feature, be aware that it isn't used as often and may therefore contain bugs.

## Writing Large Values

Writing is somewhat similar - Npgsql has an internally write buffer (also 8K by default). When writing your query's SQL and parameters to PostgreSQL, Npgsql always writes "sequentially", that is, filling up the 8K buffer and flushing it when full. You can use `Write Buffer Size` to control the buffer's size, although there are less reasons for doing so. One reason could be to reduce the number of I/O systems performed when writing - a bigger buffer means less write calls. However, keep in mind that the underlying networking stack will split your data into packets regardless of how you write.

This option is better left untouched unless you know what you're doing.
