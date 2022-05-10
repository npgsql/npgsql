using System;

namespace Npgsql.Util;

static class VersionExtensions
{
    /// <summary>
    /// Allocation free helper function to find if version is greater than expected
    /// </summary>
    public static bool IsGreaterOrEqual(this Version version, int major, int minor = 0)
        => version.Major != major
            ? version.Major > major
            : version.Minor >= minor;
}