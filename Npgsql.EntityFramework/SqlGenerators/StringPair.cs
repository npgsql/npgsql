using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.SqlGenerators
{
    /// <summary>
    /// Used for lookup in a Dictionary, since Tuple is not available in .NET 3.5
    /// </summary>
    internal class StringPair
    {
        private string _item1;
        private string _item2;

        public string Item1 { get { return _item1; } }
        public string Item2 { get { return _item2; } }

        public StringPair(string s1, string s2)
        {
            _item1 = s1;
            _item2 = s2;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            StringPair o = obj as StringPair;
            if (o == null)
                return false;

            return _item1 == o._item1 && _item2 == o._item2;
        }

        public override int GetHashCode()
        {
            return (_item1 + "." + _item2).GetHashCode();
        }
    }
}
