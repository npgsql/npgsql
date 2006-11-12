
// Npgsql.NpgsqlFactory.cs
//
// Author:
//	Francisco Jr. (fxjrlists@yahoo.com.br)
//
//	Copyright (C) 2002-2006 The Npgsql Development Team
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Data.Common;

namespace Npgsql
{
    /// <summary>
    /// A factory to create instances of various Npgsql objects.
    /// </summary>
    [Serializable]
    public sealed class NpgsqlFactory : DbProviderFactory
    {

        public static NpgsqlFactory Instance = new NpgsqlFactory();


        private NpgsqlFactory()
        {
            
        }

        

        /// <summary>
        /// Creates an NpgsqlCommand object.
        /// </summary>

        public override DbCommand CreateCommand()
        {
            return new NpgsqlCommand();
        }


        public override DbCommandBuilder CreateCommandBuilder()
        {
            return new NpgsqlCommandBuilder();
        }

        public override DbConnection CreateConnection()
        {
            return new NpgsqlConnection();
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            return new NpgsqlDataAdapter();
        }

        public override DbParameter CreateParameter()
        {
            return new NpgsqlParameter();
        }
    }

}
