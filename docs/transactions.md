# Transactions

## Basic Transactions

Transactions can be started by calling the standard ADO.NET method `NpgsqlConnection.BeginTransaction()`.

PostgreSQL doesn't support nested or concurrent transactions - only one transaction may be in progress at any given moment. Calling `BeginTransaction()` while a transaction is already in progress will throw an exception. Because of this, it isn't necessary to pass the `NpgsqlTransaction` object returned from `BeginTransaction()` to commands you execute - calling `BeginTransaction()` means that all subsequent commands will automatically participate in the transaction, until either a commit or rollback is performed. However, for maximum portability it's recommended to set the transaction on your commands.

Although concurrent transactions aren't supported, PostgreSQL supports the concept of *savepoints* - you may set named savepoints in a transaction and roll back to them later without rolling back the entire transaction. Savepoints can be created, rolled back to, and released via `NpgsqlTransaction.Save(name)`, `NpgsqlTransaction.Rollback(name)` and `NpgsqlTransaction.Release(name)` respectively. [See the PostgreSQL documentation for more details.](https://www.postgresql.org/docs/current/static/tutorial-transactions.html).

When calling `BeginTransaction()`, you may optionally set the *isolation level*. [See the docs for more details.](https://www.postgresql.org/docs/current/static/transaction-iso.html)

## System.Transactions and Distributed Transactions

In addition to `DbConnection.BeginTransaction()`, .NET includes System.Transactions, an alternative API for managing transactions - [read the MSDN docs to understand the concepts involved](https://msdn.microsoft.com/en-us/library/ee818746.aspx). Npgsql fully supports this API, and starting with version 3.3 will automatically enlist to ambient TransactionScopes (you can disable enlistment by specifying `Enlist=false` in your connection string).

When more than one connection (or resource) enlists in the same transaction, the transaction is said to be *distributed*. Distributed transactions allow you to perform changes atomically across more than one database (or resource) via a two-phase commit protocol - [here is the MSDN documentation](https://msdn.microsoft.com/en-us/library/windows/desktop/ms681205(v=vs.85).aspx). Npgsql supports distributed transactions - support has been rewritten for version 3.2, fixing many previous issues. However, at this time Npgsql enlists as a *volatile resource manager*, meaning that if your application crashes while performing, recovery will not be managed properly. For more information about this, [see this page and the related ones](https://msdn.microsoft.com/en-us/library/ee818750.aspx). If you would like to see better distributed transaction recovery (i.e. durable resource manager enlistment), please say so [on this issue](https://github.com/npgsql/npgsql/issues/1378) and subscribe to it for updates.

Note that if you open and close connections to the same database inside an ambient transaction, without ever having two connections open *at the same time*, Npgsql will internally reuse the same connection, avoiding the escalation to a full-blown distributed transaction. This is better for performance and for general simplicity.
