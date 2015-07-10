---
layout: doc
title: Migrating from 2.2 to 3.0
---

Version 3.0 represents a near-total rewrite of Npgsql. In addition to changing how Npgsql works internally and communicates
with PostgreSQL, a conscious effort was made to better align Npgsql with the ADO.NET specs/standard and with SqlClient
where that made sense. This means that you cannot expect to drop 3.0 as a replacement to 2.2 and expect things to work
- upgrade cautiously and test extensively before deploying anything to production.

The following is a *non-exhaustive* list of things that changed. If you run against a breaking change not documented here,
please let us know and we'll add it.

## SSL

* Npgsql 2.2 didn't perform validation on the server's certificate by default, so self-signed certificate were accepted.
  The new default is to perform validation. Specify the
  [Trust Server Certificate](connection-string-parameters.html#trust-server-certificate) connection string parameter to get back previous behavior.
* The "SSL" connection string parameter has been removed, use "SSL Mode" instead.
* The "SSL Mode" parameter's Allow option has been removed, as it wasn't doing anything.

## Type Handling

* Previously, Npgsql allowed writing a NULL by setting NpgsqlParameter.Value to `null`.
  This is [not allowed in ADO.NET](https://msdn.microsoft.com/en-us/library/system.data.common.dbparameter.value%28v=vs.110%29.aspx)
  and is no longer supported, set to `DBNull.Value` instead.
* Removed support for writing a parameter with an `IEnumerable<T>` value, since that would require Npgsql to enumerate it multiple
  time internally. `IList<T>` and IList are permitted.
* NpgsqlMacAddress has been removed and replaced by the standard .NET PhysicalAddress.
* Npgsql's BitString has been removed and replaced by the standard .NET BitArray.
* NpgsqlTime has been removed and replaced by the standard .NET TimeSpan.
* NpgsqlTimeZone has been removed.
* NpgsqlTimeTZ now holds 2 TimeSpans, rather than an NpgsqlTime and an NpgsqlTimeZone.
* NpgsqlTimeStamp no longer maps DateTime.{Max,Min}Value to {positive,negative}
  infinity. Use NpgsqlTimeStamp.Infinity and NpgsqlTimeStamp.MinusInfinity explicitly for that.
  You can also specify the "Convert Infinity DateTime" connection string parameter to retain the old behavior.
* Renamed NpgsqlInet's addr and mask to Address and Mask.
* NpgsqlPoint now holds Doubles instead of Singles (#437).
* NpgsqlDataReader.GetFieldType() and GetProviderSpecificFieldType() now return Array for arrays.
  Previously they returned int[], even for multidimensional arrays.
* NpgsqlDataReader.GetDataTypeName() now returns the name of the PostgreSQL type rather than its OID.

## Retired features

* Removed the "Preload Reader" feature, which loaded the entire resultset into memory. If you require this
  (inefficient) behavior, read the result into memory outside Npgsql. We plan on working on MARS support,
  see #462.
* The "Use Extended Types" parameter is no longer needed and isn't supported. To access PostgreSQL values
  that can't be represented by the standard CLR types, use the standard ADO.NET
  `NpgsqlDataReader.GetProviderSpecificValue` or even better, the generic
  `NpgsqlDataReader.GetFieldValue<T>`.
* Removed the feature where Npgsql automatically "dereferenced" a resultset of refcursors into multiple
  resultsets (this was used to emulate returning multiple resultsets from stored procedures).
* Removed the AlwaysPrepare connection string parameter
* Removed NpgsqlDataReader.LastInsertedOID, it did not allow accessing individual OIDs in multi-statement commands.
  Replaced with NpgsqlDataReader.Statements, which provides OID and affected row information on a statement-by-statement
  basis.

## Other

* The Entity Framework provider packages have been renamed to align with Microsoft's new naming.
  The new packages are *EntityFramework5.Npgsql* and *EntityFramework6.Npgsql*. EntityFramework7.Npgsql is in alpha.
* It is no longer possible to create database entities (tables, functions) and then use them in the same multi-query command -
  you must first send a command creating the entity, and only then send commands using it.
  See [#641](https://github.com/npgsql/npgsql/issues/641) for more details.
* Previously, Npgsql set DateStyle=ISO, lc_monetary=C and extra_float_digits=3 on all connections it created. This is no longer
  case, if you rely on these parameters you must send them yourself.
* Removed the obsolete `NpgsqlParameterCollection.Add(name, value)` method. Use `AddWithValue()` instead, which also exists
  in SqlClient.
* The savepoint manipulation methods on `NpgsqlTransaction` have been renamed from `Save`, and `Rollback` to
  `CreateSavepoint` and `RollbackToSavepoint`.
* The default CommandTimeout has changed from 20 seconds to 30 seconds, as in
  [ADO.NET](https://msdn.microsoft.com/en-us/library/system.data.idbcommand.commandtimeout(v=vs.110).aspx).
* `CommandType.TableDirect` now requires CommandText to contain the name of a table, as per the
  [MSDN docs](https://msdn.microsoft.com/en-us/library/system.data.commandtype%28v=vs.110%29.aspx).
  Multiple tables (join) aren't supported.
* `CommandType.StoredProcedure` now requires CommandText contain *only* the name of a function, without parentheses or parameter
  information, as per the [MSDN docs](https://msdn.microsoft.com/en-us/library/system.data.commandtype%28v=vs.110%29.aspx).
* Moved the `LastInsertedOID` property from NpgsqlCommand to NpgsqlReader, like the standard ADO.NET `RecordsAffected`.

