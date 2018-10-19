# NpgsqlCommand.cs

The `SendReplication` method required manual merging here as it was very similar with other source method implementations and near them - that caused the conflict.

## Manually solved conflicts

### 1. `SendReplication`, `SendPrepare` and `SendDeriveParameters` methods

The `SendReplication` method location caused a conflict between itself and two other similar implementations in terms of code. Manual adjustments were made to maintain their source versions alongside the new `SendReplication` implementation.

## Automatic solved conflicts (differences)

### 1. Updates on framework constant checks (`#if NET451`, `#if NETCOREAPP1_0`, etc.)

Those checks were updated/removed through a series of updates for new frameworks thata happened since the PR was created.

### 2. Parameter derivation implementation

This was added as a feature of commit ["Implementation of parameter derivation for non function queries (#1698)"](https://github.com/npgsql/npgsql/commit/c5f2a8ef29653ec894f26fa79a1973cc81a6bd2d) and taken from source.

### 3. `PrepareAsync` method

Added in commit ["Add PrepareAsync()"](https://github.com/npgsql/npgsql/commit/cb3e6b6354c2df41a32b1a5ab5d0d76e08d45aa5s).

### 4. Removal of `Execute` method

This was done as part of the commit ["Refactor command execution to reduce async calls"](https://github.com/npgsql/npgsql/commit/2ff96346186d0f6b2779a9f282b8e45fbef14ae2#diff-8f379b76776460da9fcc7265372c6f40). Most of its implementation was moved to the `ExecuteDbDataReader` method.