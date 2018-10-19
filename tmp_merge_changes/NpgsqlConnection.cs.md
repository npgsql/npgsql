# NpgsqlConnection.cs

One manual operation was needed at removal of a method that was moved to another file.

## Manually solved conflicts

### 1. Removal of `CloseOngoingOperations` method

It was moved to `NpgsqlConnector` class, as a necessary change to close the reader before rolling back a transaction (["Close reader before rolling back in tx dispose"](https://github.com/npgsql/npgsql/commit/a2e19e346ba644f98b2d305a49d5c481b54929c8#diff-6daf78c3f8b7fabad257e702ed1ef44e)).

## Automatic solved conflicts (differences)

### 1. Updates on framework constant checks (`#if NET451`, `#if NETCOREAPP1_0`, etc.)

Those checks were updated/removed through a series of updates for new frameworks thata happened since the PR was created.

### 2. Default values for fields/properties like `_userFacingConnectionString`

This was added to solve an issue that cause `NullReferenceException` when using the default constructor at commit ["Fix NRE with connection default ctor"](https://github.com/npgsql/npgsql/commit/094d54566f71c163c139e1a917b94ad7acf05e1e).

### 3. `PoolManager` and `Connector` property remade as field, pool handling chagnes

This happened after a commit where pooling was remaded as a lock-free thing (["Reimplemented pooling as lock-free"](https://github.com/npgsql/npgsql/commit/19c436e41a64392c7e9a068882976880e2d3d96b)).

### 4. `TypeMapper` property

Added as a feature of commit ["Big refactor of type mapping system"](https://github.com/npgsql/npgsql/commit/4a503c7000dae25cf2ddc860d25d2db93539c0d0).

### 5. `ClosedToOpenEventArgs` and `OpenToClosedEventArgs` static fields

Those were added to cache these events whenever OnStateChange was fired, increasing performance (["Cache StateChangeEventArgs on connection"](https://github.com/npgsql/npgsql/commit/57359166c454801605467f0dc425487758598e8c)).

### 6. Parameterless constructor optimization

Happened on commit ["NpgsqlConnection parameterless ctor optimization"](https://github.com/npgsql/npgsql/commit/cf3992577436718417c361049773cbab47668f6a) and taken from source.

### 7. Removal of line that sets `RSACryptoServiceProvider.UserMachineKeyStore`

This was added as a workaround for a very old bug, and was removed in commit ["Stop setting RSACryptoServiceProvider.UseMachineKeyStore"](https://github.com/npgsql/npgsql/commit/5ba5f21570077cd42e6a7010464d4df08697491c#diff-6daf78c3f8b7fabad257e702ed1ef44e).

### 8. Changes on the Synchronization Context

Those were added as improvements on the commit ["Fix sync context switching"](https://github.com/npgsql/npgsql/commit/c0ca8656c6cb2449c7c4e8b016876067dfce61a2), and taken from source.

### 9. Removal of `UseSslStream`, `UseConformantStrings` and `public bool SupportsEStringPrefix` properties

This happened on commit ["Remove some old useless connection capacity APIs"](https://github.com/npgsql/npgsql/commit/5fab5a8b2f43ef43eecf92eeef3e252fd2b76c62#diff-6daf78c3f8b7fabad257e702ed1ef44e) as a breaking change of version 3.3.

### 10. Adding `Settings.Username` as a second option for `Database` property

This was changed on commit ["Fix NpgsqlConnection.Database"](https://github.com/npgsql/npgsql/commit/21865e590dfca3ce9636cb0190b8f50537eb1512), which uses username as database when not explicitly set.

### 11. Checks for active enlisting transactions

This was an improvement made on commit ["Better check for active enlisting transaction"](https://github.com/npgsql/npgsql/commit/73c4883f13a1e0075316e125fd1a6201e870a1dd).

### 12. `HasIntegerDateTimes` property

Added in commit ["Big refactor of type mapping system"](https://github.com/npgsql/npgsql/commit/4a503c7000dae25cf2ddc860d25d2db93539c0d0).

### 13. `Timezone` property

Added as a feature of commit ["NodaTime type plugin"](https://github.com/npgsql/npgsql/commit/1b02912edb375cf12d6ba2e68ef13bd934b810c1).

### 14. Replication features

Added as a code from this commit.

### 15. `GetSchema` removed

This was moved to `NpgsqlSchema` class in commit ["Small refactor of GetSchema()"](https://github.com/npgsql/npgsql/commit/723153bfefaf4d429fe02da722cacf03c5c2d463).