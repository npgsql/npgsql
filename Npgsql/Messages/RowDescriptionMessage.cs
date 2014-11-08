using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Npgsql.TypeHandlers;
using NpgsqlTypes;

namespace Npgsql.Messages
{
    /// <summary>
    /// This class represents a RowDescription message sent from
    /// the PostgreSQL.
    /// </summary>
    internal sealed class RowDescriptionMessage : IServerMessage
    {
        List<FieldDescription> _fields;
        Dictionary<string, int> _nameIndex;

        public RowDescriptionMessage(NpgsqlBufferedStream buf, TypeHandlerRegistry typeHandlerRegistry)
        {
            _fields = new List<FieldDescription>();
            _nameIndex = new Dictionary<string, int>();

            var numFields = buf.ReadInt16();
            for (var i = 0; i != numFields; ++i)
            {
                var field = new FieldDescription {
                    Name = buf.ReadNullTerminatedString(),
                    TableOID = buf.ReadInt32(),
                    ColumnAttributeNumber = buf.ReadInt16(),
                    OID = buf.ReadInt32(),
                    TypeSize = buf.ReadInt16(),
                    TypeModifier = buf.ReadInt32(),
                    FormatCode = (FormatCode) buf.ReadInt16()
                };

                field.Handler = typeHandlerRegistry[field.OID];

                _fields.Add(field);
                _nameIndex[field.Name] = i;
            }
        }

        public FieldDescription this[int index]
        {
            get { return _fields[index]; }
        }

        public int NumFields
        {
            get { return _fields.Count; }
        }

        public BackEndMessageCode Code { get { return BackEndMessageCode.RowDescription; } }
    }

    internal sealed class FieldDescription
    {
        internal string Name { get; set; }
        internal int OID { get; set; }
        internal short TypeSize { get; set; }
        internal int TypeModifier { get; set; }
        internal int TableOID { get; set; }
        internal short ColumnAttributeNumber { get; set; }
        internal FormatCode FormatCode { get; set; }
        internal TypeHandler Handler { get; set; }

        public bool IsBinaryFormat { get { return FormatCode == FormatCode.Binary; } }
        public bool IsTextFormat   { get { return FormatCode == FormatCode.Text;   } }
    }
}
