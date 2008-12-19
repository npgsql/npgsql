// created on 12/6/2002 at 20:29

// Npgsql.NpgsqlRowDescription.cs
//
// Author:
//	Francisco Jr. (fxjrlists@yahoo.com.br)
//
//	Copyright (C) 2002 The Npgsql Development Team
//	npgsql-general@gborg.postgresql.org
//	http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
// 
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
// 
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NpgsqlTypes;

namespace Npgsql
{
	/// <summary>
	/// This class represents a RowDescription message sent from
	/// the PostgreSQL.
	/// </summary>
	///
	internal abstract class NpgsqlRowDescription : IServerResponseObject
	{
	    private abstract class KanaWidthConverter
	    {
	        //This is private to NpgsqlRowDescription as it is only used there.
	        //It may prove desirable to move it into PGUtil.cs or elsewhere and make
	        //it internal if any other classes want this functionality.
	        
	        //The only place that .NET's framework supports kana-width insensitivity directly seems to
	        //be Microsoft.VisualBasic namespace! (presumably for backwards compatibility with VB6). Hence we
	        //have to roll our own to support the spec on GetOrdinal() and indexing of records by field-name.
	        
	        //Lookup table of narrow equivalents of wide Katakana characters. The approach is semi-algorithmic and
	        //semi look-up.
	        private static readonly char[] FULL_WIDTH_TO_NARROW_WIDTH_KATAKANA_LOOKUP = new char[]
	        {
                '\u3002', '\u300C', '\u300D', '\u3001', '\u30FB', '\u30F2', '\u30A1', '\u30A3', // U+FF61 - U+FF68
                '\u30A5', '\u30A7', '\u30A9', '\u30E3', '\u30E5', '\u30E7', '\u30C3', '\u30FC', // U+FF69 - U+FF70
                '\u30A2', '\u30A4', '\u30A6', '\u30A8', '\u30AA', '\u30AB', '\u30AD', '\u30AF', // U+FF71 - U+FF78
                '\u30B1', '\u30B3', '\u30B5', '\u30B7', '\u30B9', '\u30BB', '\u30BD', '\u30BF', // U+FF79 - U+FF80
                '\u30C1', '\u30C4', '\u30C6', '\u30C8', '\u30CA', '\u30CB', '\u30CC', '\u30CD', // U+FF81 - U+FF88
                '\u30CE', '\u30CF', '\u30D2', '\u30D5', '\u30D8', '\u30DB', '\u30DE', '\u30DF', // U+FF89 - U+FF90
                '\u30E0', '\u30E1', '\u30E2', '\u30E4', '\u30E6', '\u30E8', '\u30E9', '\u30EA', // U+FF91 - U+FF98
                '\u30EB', '\u30EC', '\u30ED', '\u30EF', '\u30F3', '\u309B', '\u309C'            // U+FF99 - U+FF9F
	        };
	        protected string ToKanaNarrow(string input)
	        {
	            StringBuilder sb = new StringBuilder(input.Length);
	            foreach(char ch in GetKanaNarrowChars(input))
	                sb.Append(ch);
	            return sb.ToString();
	        }
	        //Enumerate as in many cases this lets use skip out after just a few tests (as soon as one returns false).
	        protected IEnumerable<char> GetKanaNarrowChars(string str)
	        {
	            if(str != null)
    	            for(int idx = 0; idx != str.Length; ++idx)
    	                if(str[idx] <= '\uFF60' || str[idx] >= '\uFFA0')
    	                    yield return str[idx];
    	                else
    	                {
    	                    if(idx == str.Length - 1)
    	                        yield return FULL_WIDTH_TO_NARROW_WIDTH_KATAKANA_LOOKUP[str[idx] - '\uFF61'];
    	                    else
    	                        switch(str[idx + 1])
    	                        {
    	                            case '\u3098': case '\u3099': case '\uFF9E':
        	                            if(str[idx] == '\uFF73')
        	                            {
        	                                yield return '\u30F4';
        	                                ++idx;
        	                            }
        	                            else if(str[idx] >= '\uFF76' && str[idx] <= '\uFF84' || str[idx] >= '\uFF8A' && str[idx] <= '\uFF8E')
        	                                yield return (char)(1 + (int)FULL_WIDTH_TO_NARROW_WIDTH_KATAKANA_LOOKUP[str[idx++] - '\uFF61']);
        	                            else
        	                                yield return FULL_WIDTH_TO_NARROW_WIDTH_KATAKANA_LOOKUP[str[idx] - '\uFF61'];
    	                                break;
    	                            case '\u309A': case'\u309C': case '\uFF9F':
    	                                switch(str[idx])
    	                                {
    	                                    case '\uFF8A': case '\uFF8B': case '\uFF8C': case '\uFF8D': case '\uFF8E':
            	                                yield return (char)(2 + (int)FULL_WIDTH_TO_NARROW_WIDTH_KATAKANA_LOOKUP[str[idx++] - '\uFF61']);
            	                                break;
            	                            default:
            	                                yield return FULL_WIDTH_TO_NARROW_WIDTH_KATAKANA_LOOKUP[str[idx] - '\uFF61'];
            	                                break;
    	                                }
        	                            break;
    	                            default:
    	                                yield return FULL_WIDTH_TO_NARROW_WIDTH_KATAKANA_LOOKUP[str[idx] - '\uFF61'];
                                        break;
    	                        }
    	                }
	        }
	    }
	    private sealed class KanaWidthInsensitiveComparer : KanaWidthConverter, IEqualityComparer<string>
	    {
	        public static readonly KanaWidthInsensitiveComparer INSTANCE = new KanaWidthInsensitiveComparer();
	        private KanaWidthInsensitiveComparer(){}
            public bool Equals(string x, string y)
            {//Do not use length comparison as short-cut, kana-width-folding can increase length by up to double.
                if(x == null)
                    return y == null;
                else if(y == null)
                    return false;

                else
                {
                    IEnumerator<char> yEnum = GetKanaNarrowChars(y).GetEnumerator();
                    foreach(char xCh in GetKanaNarrowChars(x))
                        if(!yEnum.MoveNext() || xCh != yEnum.Current)
                            return false;
                    return !yEnum.MoveNext(); // match if y is out of chars, mismatch if there are more to go.
                }
            }
            public int GetHashCode(string obj)
            {
                int ret = 0;
                string test = ToKanaNarrow(obj);
                foreach (char ch in GetKanaNarrowChars(obj))
                {
                    //The ideal amount to shift each value is one that would evenly spread it throughout
                    //the resultant bytes. Using the current result % 32 is essentially using a random value
                    //but one that will be the same on subsequent calls.
                    ret ^= PGUtil.RotateShift((int)ch, ret%(sizeof (int) * 8));
                }
                return ret;
            }
	    }
	    private sealed class KanaWidthCaseInsensitiveComparator : KanaWidthConverter, IEqualityComparer<string>
	    {
	        public static readonly KanaWidthCaseInsensitiveComparator INSTANCE = new KanaWidthCaseInsensitiveComparator();
	        private KanaWidthCaseInsensitiveComparator(){}
            public bool Equals(string x, string y)
            {//Do not use length comparison as short-cut, case-folding can increase length by up to triple.
                if(x == null)
                    return y == null;
                else if(y == null)
                    return false;
                else
                    //Don't reinvent case-folding when .NET does it for us. Not perfect, but then neither is postgres in this regard.
                    //See note below on why we use OrdinalIgnoreCase rather than InvariantCultureIgnoreCase.
                    return string.Equals(ToKanaNarrow(x), ToKanaNarrow(y), StringComparison.OrdinalIgnoreCase);
            }
            public int GetHashCode(string obj)
            {
                //Don't reinvent case-folding when .NET does it for us. Not perfect, but then neither is postgres in this regard.
                
                //FIXME: Are we happy with this?: string.Compare("ﬂ", "ss", StringComparison.InvariantCultureIgnoreCase) returns 0
                //(as it should) but "ﬂ".ToUpperInvariant() returns "ﬂ", which is a bug in string.ToUpperInvariant that
                //will mean "Weiﬂbier" will not match WEISSBIER, as it should. Is this an issue?
                //Using OrdinalIgnoreCase seems to at least be consistent in making Equals(string, string) correspond
                //with GetHashCode(string), and avoids locale-based security issues, but is not perfect.
                //Then again, Postgres select upper('ﬂ') also returns "ﬂ", so maybe this is exactly what we want?
                
                //Due to possible confusion, if we decide this is okay and remove the fix request above, then above notes should remain in the comments.
                return ToKanaNarrow(obj).ToLowerInvariant().GetHashCode();
            }
	    }
		/// <summary>
		/// This struct represents the internal data of the RowDescription message.
		/// </summary>
		public abstract class FieldData
		{
			private string _name; // Protocol 2/3
			private int _typeOID; // Protocol 2/3
			private short _typeSize; // Protocol 2/3
			private int _typeModifier; // Protocol 2/3
			private int _tableOID; // Protocol 3
			private short _columnAttributeNumber; // Protocol 3
			private FormatCode _formatCode; // Protocol 3. 0 text, 1 binary
			private NpgsqlBackendTypeInfo _typeInfo; // everything we know about this field type

			public string Name
			{
				get { return _name; }
				protected set { _name = value; }
			}

			public int TypeOID
			{
				get { return _typeOID; }
				protected set { _typeOID = value; }
			}

			public short TypeSize
			{
				get { return _typeSize; }
				protected set { _typeSize = value; }
			}

			public int TypeModifier
			{
				get { return _typeModifier; }
				protected set { _typeModifier = value; }
			}

			public int TableOID
			{
				get { return _tableOID; }
				protected set { _tableOID = value; }
			}

			public short ColumnAttributeNumber
			{
				get { return _columnAttributeNumber; }
				protected set { _columnAttributeNumber = value; }
			}

			public FormatCode FormatCode
			{
				get { return _formatCode; }
				protected set { _formatCode = value; }
			}

			public NpgsqlBackendTypeInfo TypeInfo
			{
				get { return _typeInfo; }
				protected set { _typeInfo = value; }
			}
		}

		private readonly FieldData[] fields_data;
		private readonly Dictionary<string, int> field_name_index_table;
		private readonly Dictionary<string, int> caseInsensitiveNameIndexTable;
		private readonly Version _compatVersion;
		
		private readonly static Version KANA_FIX_VERSION = new Version(2, 0, 2, 1);
		private readonly static Version GET_ORDINAL_THROW_EXCEPTION = KANA_FIX_VERSION;

		protected NpgsqlRowDescription(Stream stream, NpgsqlBackendTypeMapping type_mapping, Version compatVersion)
		{
			int num = ReadNumFields(stream);
			fields_data = new FieldData[num];
			if((_compatVersion = compatVersion) < KANA_FIX_VERSION)
			{
                field_name_index_table = new Dictionary<string, int>(num, StringComparer.InvariantCulture);
                caseInsensitiveNameIndexTable = new Dictionary<string, int>(num, StringComparer.InvariantCultureIgnoreCase);
			}
			else
			{
    			field_name_index_table = new Dictionary<string, int>(num, KanaWidthInsensitiveComparer.INSTANCE);
    			caseInsensitiveNameIndexTable = new Dictionary<string, int>(num, KanaWidthCaseInsensitiveComparator.INSTANCE);
			}
			for (int i = 0; i != num; ++i)
			{
				FieldData fd = BuildFieldData(stream, type_mapping);
				fields_data[i] = fd;
				if (!field_name_index_table.ContainsKey(fd.Name))
				{
					field_name_index_table.Add(fd.Name, i);
					if (!caseInsensitiveNameIndexTable.ContainsKey(fd.Name))
					{
						caseInsensitiveNameIndexTable.Add(fd.Name, i);
					}
				}
			}
		}

		protected abstract FieldData BuildFieldData(Stream stream, NpgsqlBackendTypeMapping typeMapping);
		protected abstract int ReadNumFields(Stream stream);

		public FieldData this[int index]
		{
			get { return fields_data[index]; }
		}

		public int NumFields
		{
			get { return (Int16) fields_data.Length; }
		}

		public bool HasOrdinal(string fieldName)
		{
		    return caseInsensitiveNameIndexTable.ContainsKey(fieldName);
		}
		public int TryFieldIndex(string fieldName)
		{
		    return HasOrdinal(fieldName) ? FieldIndex(fieldName) : -1;
		}
		public int FieldIndex(String fieldName)
		{
			int ret = -1;
			if(field_name_index_table.TryGetValue(fieldName, out ret) || caseInsensitiveNameIndexTable.TryGetValue(fieldName, out ret))
			    return ret;
			else if(_compatVersion < GET_ORDINAL_THROW_EXCEPTION)
			    return -1;
			else
			    throw new IndexOutOfRangeException("Field not found");
		}
	}

	internal sealed class NpgsqlRowDescriptionV2 : NpgsqlRowDescription
	{
		public NpgsqlRowDescriptionV2(Stream stream, NpgsqlBackendTypeMapping typeMapping, Version compatVersion)
			: base(stream, typeMapping, compatVersion)
		{
		}

		private sealed class FieldDataV2 : FieldData
		{
			public FieldDataV2(Stream stream, NpgsqlBackendTypeMapping typeMapping)
			{
				Name = PGUtil.ReadString(stream);
				TypeInfo = typeMapping[TypeOID = PGUtil.ReadInt32(stream)];
				TypeSize = PGUtil.ReadInt16(stream);
				TypeModifier = PGUtil.ReadInt32(stream);
			}
		}

		protected override FieldData BuildFieldData(Stream stream, NpgsqlBackendTypeMapping type_mapping)
		{
			return new FieldDataV2(stream, type_mapping);
		}

		protected override int ReadNumFields(Stream stream)
		{
			return PGUtil.ReadInt16(stream);
		}
	}

	internal sealed class NpgsqlRowDescriptionV3 : NpgsqlRowDescription
	{
		private sealed class FieldDataV3 : FieldData
		{
			public FieldDataV3(Stream stream, NpgsqlBackendTypeMapping typeMapping)
			{
				Name = PGUtil.ReadString(stream);
				TableOID = PGUtil.ReadInt32(stream);
				ColumnAttributeNumber = PGUtil.ReadInt16(stream);
				TypeInfo = typeMapping[TypeOID = PGUtil.ReadInt32(stream)];
				TypeSize = PGUtil.ReadInt16(stream);
				TypeModifier = PGUtil.ReadInt32(stream);
				FormatCode = (FormatCode) PGUtil.ReadInt16(stream);
			}
		}

		public NpgsqlRowDescriptionV3(Stream stream, NpgsqlBackendTypeMapping typeMapping, Version compatVersion)
			: base(stream, typeMapping, compatVersion)
		{
		}

		protected override FieldData BuildFieldData(Stream stream, NpgsqlBackendTypeMapping typeMapping)
		{
			return new FieldDataV3(stream, typeMapping);
		}

		protected override int ReadNumFields(Stream stream)
		{
			PGUtil.EatStreamBytes(stream, 4);
			return PGUtil.ReadInt16(stream);
		}
	}
}
