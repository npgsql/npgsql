using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Npgsql.BackendMessages;
using Npgsql.TypeHandlers.NumericHandlers;

namespace Npgsql.TypeHandlers.InternalTypesHandlers
{
    /// <summary>
    /// An OIDVector is simply a regular array of uints, with the sole exception that its lower bound must
    /// be 0 (we send 1 for regular arrays).
    /// </summary>
    [TypeMapping("oidvector", NpgsqlDbType.Oidvector)]
    internal class OIDVectorHandler : ArrayHandler<uint>
    {
        static readonly ILog _log = LogManager.GetCurrentClassLogger();

        public OIDVectorHandler(TypeHandlerRegistry registry) : base(new UInt32Handler())
        {
            // TODO: We assume here that the oid type comes before oidvector
            var oidHandler = registry[NpgsqlDbType.Oid];
            if (oidHandler == registry.UnrecognizedTypeHandler)
            {
                _log.Warn("oid type not present when setting up oidvector type. oidvector will not work.");
                return;
            }
            LowerBound = 0;
            ElementHandler.OID = oidHandler.OID;
        }
    }
}
