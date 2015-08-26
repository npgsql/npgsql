---
layout: doc
title: Migrating from 3.0 to 3.1
---

* 3.0 renamed the savepoint manipulation methods on `NpgsqlTransaction` have been renamed from `Save`, and `Rollback` to
  `CreateSavepoint` and `RollbackToSavepoint`. This broke the naming conventions for these methods across other providers
  (SqlClient, Oracle...) and so in 3.0.2 the previous names were returned and the new names marked as obsolete.
  3.1 removes the the new names and leaves only `Save` and `Rollback`. See [#738](https://github.com/npgsql/npgsql/issues/738).
