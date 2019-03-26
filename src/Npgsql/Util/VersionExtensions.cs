using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql.Util
{
    static class VersionExtensions
    {
        /// <summary>
        /// Allocation free helper function to find if version is greater than expected
        /// </summary>
        public static bool IsGreaterOrEqual(this Version version, int major, int minor, int build)
        {
            if (version.Major != major)
                return version.Major > major;

            if (version.Minor != minor)
                return version.Minor > minor;

            return version.Build != build ? version.Build > build : true;
        }
    }
}
