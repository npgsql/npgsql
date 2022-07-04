namespace Npgsql;

#pragma warning disable RS0016

/// <summary>
/// Specifies server type preference.
/// </summary>
public enum TargetSessionAttributes : byte
{
    /// <summary>
    /// Any successful connection is acceptable.
    /// </summary>
    Any = 0,

    /// <summary>
    /// Session must accept read-write transactions by default (that is, the server must not be in hot standby mode and the
    /// <c>default_transaction_read_only</c> parameter must be off).
    /// </summary>
    ReadWrite = 1,

    /// <summary>
    /// Session must not accept read-write transactions by default (the converse).
    /// </summary>
    ReadOnly = 2,

    /// <summary>
    /// Server must not be in hot standby mode.
    /// </summary>
    Primary = 3,

    /// <summary>
    /// Server must be in hot standby mode.
    /// </summary>
    Standby = 4,

    /// <summary>
    /// First try to find a primary server, but if none of the listed hosts is a primary server, try again in <see cref="Any"/> mode.
    /// </summary>
    PreferPrimary = 5,

    /// <summary>
    /// First try to find a standby server, but if none of the listed hosts is a standby server, try again in <see cref="Any"/> mode.
    /// </summary>
    PreferStandby = 6,
}
