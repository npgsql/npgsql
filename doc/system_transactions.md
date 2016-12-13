# System.Transactions, TransactionScope and Distributed Transactions

## Introduction

As an alternative to the traditional way of starting transactions - `NpgsqlConnection.BeginTransaction()` - Npgsql also supports System.Transactions. System.Transactions provide TransactionScope, which provides an *ambient* mechanism for managing transactions, and also allows multiple participants (called enlistments) to the same transaction. Note that System.Transactions is currently unsupported in .NET Core. For more information on System.Transactions, [see this article](https://msdn.microsoft.com/en-us/library/ee818755(v=vs.110).aspx).

Npgsql's System.Transactions support has been fully rewritten for 3.2, and should be more reliable.

## Distributed Transactions and Recovery

One of the features of System.Transactions is that there can be multiple participants in the transaction; the system uses the two-phase commit protocol to ensure that either all participants commit, or all of them rollback. In System.Transactions, notifications about transaction events (commit, rollback, prepare for two-phase commit) are delivered to *resource managers*, which represent participants in the transaction (e.g. a database connection). Resource managers can be volatile or durable, with the latter meaning that they support recovery; that is, if there's a process crash between the two-phase prepare and commit phases, the resource manager will be able to recover the pending transaction and complete it.

PostgreSQL supports two-phase commit via [prepared transactions](https://www.postgresql.org/docs/current/static/sql-prepare-transaction.html). Unfortunately, Npgsql currently does not provide a durable resource manager, only a volatile one. This means that if your application crashes after a transaction has been prepared, but before it has been committed (or rolled back), a prepared transaction will be left orphaned in PostgreSQL. You must be aware of this and take the proper measures to purge the orphaned transactions, perhaps based on their age (e.g. any prepared transaction lingering for more than an hour is deemed to be orphaned and is rolled back. You can find out which prepared transactions are active by querying the [pg_prepared_xacts system view](https://www.postgresql.org/docs/current/static/view-pg-prepared-xacts.html). The implementation of a durable, out-of-process resource manager is definitely a goal and tracked by [#1378](https://github.com/npgsql/npgsql/issues/1378) - please let us know you're interested by commenting on that issue.

Note that this only scenarios where at least two connections are enlisted *simultaneously* on the same transaction; if you have just one connection at any given point, your transaction won't escalate and prepared transactions won't be used.
