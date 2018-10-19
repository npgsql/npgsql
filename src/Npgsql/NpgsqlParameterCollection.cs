#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using JetBrains.Annotations;
using NpgsqlTypes;

namespace Npgsql
{
    /// <summary>
    /// Represents a collection of parameters relevant to a <see cref="NpgsqlCommand">NpgsqlCommand</see>
    /// as well as their respective mappings to columns in a DataSet.
    /// This class cannot be inherited.
    /// </summary>
    public sealed class NpgsqlParameterCollection : DbParameterCollection, IList<NpgsqlParameter>
    {
        readonly List<NpgsqlParameter> _internalList = new List<NpgsqlParameter>(5);

        // Dictionary lookups for GetValue to improve performance
        [CanBeNull]
        Dictionary<string, int> _lookup;
        [CanBeNull]
        Dictionary<string, int> _lookupIgnoreCase;

        /// <summary>
        /// Initializes a new instance of the NpgsqlParameterCollection class.
        /// </summary>
        internal NpgsqlParameterCollection()
        {

            InvalidateHashLookups();
        }

        /// <summary>
        /// Invalidate the hash lookup tables.  This should be done any time a change
        /// may throw the lookups out of sync with the list.
        /// </summary>
        internal void InvalidateHashLookups()
        {
            _lookup = null;
            _lookupIgnoreCase = null;
        }

        #region NpgsqlParameterCollection Member

        /// <summary>
        /// Gets the <see cref="NpgsqlParameter">NpgsqlParameter</see> with the specified name.
        /// </summary>
        /// <param name="parameterName">The name of the <see cref="NpgsqlParameter">NpgsqlParameter</see> to retrieve.</param>
        /// <value>The <see cref="NpgsqlParameter">NpgsqlParameter</see> with the specified name, or a null reference if the parameter is not found.</value>
        [PublicAPI]
        public new NpgsqlParameter this[string parameterName]
        {
            get
            {
                var index = IndexOf(parameterName);

                if (index == -1)
                    throw new ArgumentException("Parameter not found");

                return _internalList[index];
            }
            set
            {
                var index = IndexOf(parameterName);

                if (index == -1)
                    throw new ArgumentException("Parameter not found");

                var oldValue = _internalList[index];

                if (value.ParameterName != oldValue.ParameterName)
                    InvalidateHashLookups();

                _internalList[index] = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="NpgsqlParameter">NpgsqlParameter</see> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the <see cref="NpgsqlParameter">NpgsqlParameter</see> to retrieve.</param>
        /// <value>The <see cref="NpgsqlParameter">NpgsqlParameter</see> at the specified index.</value>
        [PublicAPI]
        public new NpgsqlParameter this[int index]
        {
            get => _internalList[index];
            set
            {
                if (value.Collection != null)
                    throw new InvalidOperationException("The parameter already belongs to a collection");

                var oldValue = _internalList[index];

                if (oldValue == value)
                    return;

                if (value.ParameterName != oldValue.ParameterName)
                    InvalidateHashLookups();

                _internalList[index] = value;
                value.Collection = this;
                oldValue.Collection = null;
            }
        }

        /// <summary>
        /// Adds the specified <see cref="NpgsqlParameter">NpgsqlParameter</see> object to the <see cref="NpgsqlParameterCollection">NpgsqlParameterCollection</see>.
        /// </summary>
        /// <param name="value">The <see cref="NpgsqlParameter">NpgsqlParameter</see> to add to the collection.</param>
        /// <returns>The index of the new <see cref="NpgsqlParameter">NpgsqlParameter</see> object.</returns>
        public NpgsqlParameter Add(NpgsqlParameter value)
        {
            if (value.Collection != null)
                throw new InvalidOperationException("The parameter already belongs to a collection");

            _internalList.Add(value);
            value.Collection = this;
            InvalidateHashLookups();
            return value;
        }

        /// <inheritdoc />
        void ICollection<NpgsqlParameter>.Add(NpgsqlParameter item)
            => Add(item);

        /// <summary>
        /// Adds a <see cref="NpgsqlParameter">NpgsqlParameter</see> to the <see cref="NpgsqlParameterCollection">NpgsqlParameterCollection</see> given the specified parameter name and value.
        /// </summary>
        /// <param name="parameterName">The name of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.</param>
        /// <param name="value">The Value of the <see cref="NpgsqlParameter">NpgsqlParameter</see> to add to the collection.</param>
        /// <returns>The paramater that was added.</returns>
        [PublicAPI]
        public NpgsqlParameter AddWithValue(string parameterName, object value)
            => Add(new NpgsqlParameter(parameterName, value));

        /// <summary>
        /// Adds a <see cref="NpgsqlParameter">NpgsqlParameter</see> to the <see cref="NpgsqlParameterCollection">NpgsqlParameterCollection</see> given the specified parameter name, data type and value.
        /// </summary>
        /// <param name="parameterName">The name of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.</param>
        /// <param name="parameterType">One of the NpgsqlDbType values.</param>
        /// <param name="value">The Value of the <see cref="NpgsqlParameter">NpgsqlParameter</see> to add to the collection.</param>
        /// <returns>The paramater that was added.</returns>
        [PublicAPI]
        public NpgsqlParameter AddWithValue(string parameterName, NpgsqlDbType parameterType, object value)
            => Add(new NpgsqlParameter(parameterName, parameterType) { Value = value });

        /// <summary>
        /// Adds a <see cref="NpgsqlParameter">NpgsqlParameter</see> to the <see cref="NpgsqlParameterCollection">NpgsqlParameterCollection</see> given the specified parameter name and value.
        /// </summary>
        /// <param name="parameterName">The name of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.</param>
        /// <param name="value">The Value of the <see cref="NpgsqlParameter">NpgsqlParameter</see> to add to the collection.</param>
        /// <param name="parameterType">One of the NpgsqlDbType values.</param>
        /// <param name="size">The length of the column.</param>
        /// <returns>The paramater that was added.</returns>
        [PublicAPI]
        public NpgsqlParameter AddWithValue(string parameterName, NpgsqlDbType parameterType, int size, object value)
            => Add(new NpgsqlParameter(parameterName, parameterType, size) { Value = value });

        /// <summary>
        /// Adds a <see cref="NpgsqlParameter">NpgsqlParameter</see> to the <see cref="NpgsqlParameterCollection">NpgsqlParameterCollection</see> given the specified parameter name and value.
        /// </summary>
        /// <param name="parameterName">The name of the <see cref="NpgsqlParameter">NpgsqlParameter</see>.</param>
        /// <param name="value">The Value of the <see cref="NpgsqlParameter">NpgsqlParameter</see> to add to the collection.</param>
        /// <param name="parameterType">One of the NpgsqlDbType values.</param>
        /// <param name="size">The length of the column.</param>
        /// <param name="sourceColumn">The name of the source column.</param>
        /// <returns>The paramater that was added.</returns>
        [PublicAPI]
        public NpgsqlParameter AddWithValue(string parameterName, NpgsqlDbType parameterType, int size, string sourceColumn, object value)
            => Add(new NpgsqlParameter(parameterName, parameterType, size, sourceColumn) { Value = value });

        /// <summary>
        /// Adds a <see cref="NpgsqlParameter">NpgsqlParameter</see> to the <see cref="NpgsqlParameterCollection">NpgsqlParameterCollection</see> given the specified value.
        /// </summary>
        /// <param name="value">The Value of the <see cref="NpgsqlParameter">NpgsqlParameter</see> to add to the collection.</param>
        /// <returns>The paramater that was added.</returns>
        [PublicAPI]
        public NpgsqlParameter AddWithValue(object value)
            => Add(new NpgsqlParameter() { Value = value });

        /// <summary>
        /// Adds a <see cref="NpgsqlParameter">NpgsqlParameter</see> to the <see cref="NpgsqlParameterCollection">NpgsqlParameterCollection</see> given the specified data type and value.
        /// </summary>
        /// <param name="parameterType">One of the NpgsqlDbType values.</param>
        /// <param name="value">The Value of the <see cref="NpgsqlParameter">NpgsqlParameter</see> to add to the collection.</param>
        /// <returns>The paramater that was added.</returns>
        [PublicAPI]
        public NpgsqlParameter AddWithValue(NpgsqlDbType parameterType, object value)
            => Add(new NpgsqlParameter { NpgsqlDbType = parameterType, Value = value });

        /// <summary>
        /// Adds a <see cref="NpgsqlParameter">NpgsqlParameter</see> to the <see cref="NpgsqlParameterCollection">NpgsqlParameterCollection</see> given the parameter name and the data type.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="parameterType">One of the DbType values.</param>
        /// <returns>The index of the new <see cref="NpgsqlParameter">NpgsqlParameter</see> object.</returns>
        [PublicAPI]
        public NpgsqlParameter Add(string parameterName, NpgsqlDbType parameterType)
            => Add(new NpgsqlParameter(parameterName, parameterType));

        /// <summary>
        /// Adds a <see cref="NpgsqlParameter">NpgsqlParameter</see> to the <see cref="NpgsqlParameterCollection">NpgsqlParameterCollection</see> with the parameter name, the data type, and the column length.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="parameterType">One of the DbType values.</param>
        /// <param name="size">The length of the column.</param>
        /// <returns>The index of the new <see cref="NpgsqlParameter">NpgsqlParameter</see> object.</returns>
        [PublicAPI]
        public NpgsqlParameter Add(string parameterName, NpgsqlDbType parameterType, int size)
            => Add(new NpgsqlParameter(parameterName, parameterType, size));

        /// <summary>
        /// Adds a <see cref="NpgsqlParameter">NpgsqlParameter</see> to the <see cref="NpgsqlParameterCollection">NpgsqlParameterCollection</see> with the parameter name, the data type, the column length, and the source column name.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="parameterType">One of the DbType values.</param>
        /// <param name="size">The length of the column.</param>
        /// <param name="sourceColumn">The name of the source column.</param>
        /// <returns>The index of the new <see cref="NpgsqlParameter">NpgsqlParameter</see> object.</returns>
        [PublicAPI]
        public NpgsqlParameter Add(string parameterName, NpgsqlDbType parameterType, int size, string sourceColumn)
            => Add(new NpgsqlParameter(parameterName, parameterType, size, sourceColumn));

        #endregion

        #region IDataParameterCollection Member

        /// <inheritdoc />
        [PublicAPI]
        // ReSharper disable once ImplicitNotNullOverridesUnknownExternalMember
        public override void RemoveAt(string parameterName)
        {
            if (parameterName == null)
                throw new ArgumentNullException(nameof(parameterName));

            RemoveAt(IndexOf(parameterName));
        }

        /// <inheritdoc />
        public override bool Contains(string parameterName)
        {
            if (parameterName == null)
                throw new ArgumentNullException(nameof(parameterName));

            return IndexOf(parameterName) != -1;
        }

        /// <inheritdoc />
        public override int IndexOf([CanBeNull] string parameterName)
        {
            if (parameterName == null)
                return -1;

            if (parameterName.Length > 0 && (parameterName[0] == ':' || parameterName[0] == '@'))
                parameterName = parameterName.Remove(0, 1);

            // Using a dictionary is much faster for 5 or more items
            if (_internalList.Count >= 5)
            {
                if (_lookup == null)
                {
                    _lookup = new Dictionary<string, int>();
                    for (var i = 0 ; i < _internalList.Count ; i++)
                    {
                        var item = _internalList[i];

                        // Store only the first of each distinct value
                        if (!_lookup.ContainsKey(item.TrimmedName))
                            _lookup.Add(item.TrimmedName, i);
                    }
                }

                // Try to access the case sensitive parameter name first
                if (_lookup.TryGetValue(parameterName, out var retIndex))
                    return retIndex;

                // Case sensitive lookup failed, generate a case insensitive lookup
                if (_lookupIgnoreCase == null)
                {
                    _lookupIgnoreCase = new Dictionary<string, int>(PGUtil.InvariantCaseIgnoringStringComparer);
                    for (var i = 0 ; i < _internalList.Count ; i++)
                    {
                        var item = _internalList[i];

                        // Store only the first of each distinct value
                        if (!_lookupIgnoreCase.ContainsKey(item.TrimmedName))
                            _lookupIgnoreCase.Add(item.TrimmedName, i);
                    }
                }

                // Then try to access the case insensitive parameter name
                if (_lookupIgnoreCase.TryGetValue(parameterName, out retIndex))
                    return retIndex;

                return -1;
            }

            // First try a case-sensitive match
            for (var i = 0; i < _internalList.Count; i++)
                if (parameterName == _internalList[i].TrimmedName)
                    return i;

            // If not fond, try a case-insensitive match
            for (var i = 0; i < _internalList.Count; i++)
                if (string.Equals(parameterName, _internalList[i].TrimmedName, StringComparison.OrdinalIgnoreCase))
                    return i;

            return -1;
        }

        #endregion

        #region IList Member

        /// <inheritdoc />
        public override bool IsReadOnly => false;

        /// <summary>
        /// Removes the specified <see cref="NpgsqlParameter">NpgsqlParameter</see> from the collection using a specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the parameter.</param>
        public override void RemoveAt(int index)
        {
            if (_internalList.Count - 1 < index)
                throw new IndexOutOfRangeException();
            Remove(_internalList[index]);
        }

        /// <inheritdoc />
        public override void Insert(int index, object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (!(value is NpgsqlParameter param))
                throw new InvalidCastException($"{nameof(value)} must be an NpgsqlParameter");
            if (param.Collection != null)
                throw new InvalidOperationException("The parameter already belongs to a collection");

            param.Collection = this;
            _internalList.Insert(index, param);
            InvalidateHashLookups();
        }

        /// <summary>
        /// Removes the specified <see cref="NpgsqlParameter">NpgsqlParameter</see> from the collection.
        /// </summary>
        /// <param name="parameterName">The name of the <see cref="NpgsqlParameter">NpgsqlParameter</see> to remove from the collection.</param>
        [PublicAPI]
        public void Remove(string parameterName)
        {
            var index = IndexOf(parameterName);
            if (index < 0)
                throw new InvalidOperationException("No parameter with the specified name exists in the collection");
            RemoveAt(index);
        }

        /// <summary>
        /// Removes the specified <see cref="NpgsqlParameter">NpgsqlParameter</see> from the collection.
        /// </summary>
        /// <param name="value">The <see cref="NpgsqlParameter">NpgsqlParameter</see> to remove from the collection.</param>
        public override void Remove(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (!(value is NpgsqlParameter param))
                throw new InvalidCastException($"{nameof(value)} must be an NpgsqlParameter");

            Remove(param);
        }

        /// <inheritdoc />
        public override bool Contains(object value)
            => value is NpgsqlParameter param && _internalList.Contains(param);

        /// <summary>
        /// Gets a value indicating whether a <see cref="NpgsqlParameter">NpgsqlParameter</see> with the specified parameter name exists in the collection.
        /// </summary>
        /// <param name="parameterName">The name of the <see cref="NpgsqlParameter">NpgsqlParameter</see> object to find.</param>
        /// <param name="parameter">A reference to the requested parameter is returned in this out param if it is found in the list.  This value is null if the parameter is not found.</param>
        /// <returns><b>true</b> if the collection contains the parameter and param will contain the parameter; otherwise, <b>false</b>.</returns>
        [ContractAnnotation("=>true,parameter:notnull; =>false,parameter:null")]
        public bool TryGetValue(string parameterName, [CanBeNull] out NpgsqlParameter parameter)
        {
            var index = IndexOf(parameterName);

            if (index != -1)
            {
                parameter = _internalList[index];
                return true;
            }
            parameter = null;
            return false;
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public override void Clear()
        {
            foreach (var toRemove in _internalList)
            {
                // clean up the parameter so it can be added to another command if required.
                toRemove.Collection = null;
            }
            _internalList.Clear();
            InvalidateHashLookups();
        }

        /// <inheritdoc />
        public override int IndexOf(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (!(value is NpgsqlParameter param))
                throw new InvalidCastException($"{nameof(value)} must be an NpgsqlParameter");
            return _internalList.IndexOf(param);
        }

        /// <inheritdoc />
        public override int Add(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (!(value is NpgsqlParameter param))
                throw new InvalidCastException($"{nameof(value)} must be an NpgsqlParameter");
            Add(param);
            return Count - 1;
        }

        /// <inheritdoc />
        public override bool IsFixedSize => false;

        #endregion

        #region ICollection Member

        /// <inheritdoc />
        public override bool IsSynchronized => (_internalList as ICollection).IsSynchronized;

        /// <summary>
        /// Gets the number of <see cref="NpgsqlParameter">NpgsqlParameter</see> objects in the collection.
        /// </summary>
        /// <value>The number of <see cref="NpgsqlParameter">NpgsqlParameter</see> objects in the collection.</value>
        public override int Count => _internalList.Count;

        /// <inheritdoc />
        public override void CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            ((ICollection)_internalList).CopyTo(array, index);
        }

        /// <inheritdoc />
        bool ICollection<NpgsqlParameter>.IsReadOnly => false;

        /// <inheritdoc />
        public override object SyncRoot => ((ICollection)_internalList).SyncRoot;

        #endregion

        #region IEnumerable Member

        IEnumerator<NpgsqlParameter> IEnumerable<NpgsqlParameter>.GetEnumerator()
            => _internalList.GetEnumerator();

        /// <inheritdoc />
        public override IEnumerator GetEnumerator() => _internalList.GetEnumerator();

        #endregion

        /// <inheritdoc />
        public override void AddRange(Array values)
        {
            foreach (NpgsqlParameter parameter in values)
                Add(parameter);
        }

        /// <inheritdoc />
        protected override DbParameter GetParameter([CanBeNull] string parameterName)
            => parameterName == null
                ? throw new ArgumentNullException(nameof(parameterName))
                : this[parameterName];

        /// <inheritdoc />
        protected override DbParameter GetParameter(int index)
            => this[index];

        /// <inheritdoc />
        protected override void SetParameter(string parameterName, DbParameter value)
        {
            if (parameterName == null)
                throw new ArgumentNullException(nameof(parameterName));
            this[parameterName] = (NpgsqlParameter) value;
        }

        /// <inheritdoc />
        protected override void SetParameter(int index, DbParameter value)
            => this[index] = (NpgsqlParameter) value;

        /// <summary>
        /// Report the offset within the collection of the given parameter.
        /// </summary>
        /// <param name="item">Parameter to find.</param>
        /// <returns>Index of the parameter, or -1 if the parameter is not present.</returns>
        [PublicAPI]
        public int IndexOf(NpgsqlParameter item)
            => _internalList.IndexOf(item);

        /// <summary>
        /// Insert the specified parameter into the collection.
        /// </summary>
        /// <param name="index">Index of the existing parameter before which to insert the new one.</param>
        /// <param name="item">Parameter to insert.</param>
        [PublicAPI]
        public void Insert(int index, NpgsqlParameter item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.Collection != null)
                throw new Exception("The parameter already belongs to a collection");

            _internalList.Insert(index, item);
            item.Collection = this;
            InvalidateHashLookups();
        }

        /// <summary>
        /// Report whether the specified parameter is present in the collection.
        /// </summary>
        /// <param name="item">Parameter to find.</param>
        /// <returns>True if the parameter was found, otherwise false.</returns>
        [PublicAPI]
        public bool Contains(NpgsqlParameter item) => _internalList.Contains(item);

        /// <summary>
        /// Remove the specified parameter from the collection.
        /// </summary>
        /// <param name="item">Parameter to remove.</param>
        /// <returns>True if the parameter was found and removed, otherwise false.</returns>
        [PublicAPI]
        public bool Remove(NpgsqlParameter item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.Collection != this)
                throw new InvalidOperationException("The item does not belong to this collection");

            if (_internalList.Remove(item))
            {
                item.Collection = null;
                InvalidateHashLookups();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Convert collection to a System.Array.
        /// </summary>
        /// <param name="array">Destination array.</param>
        /// <param name="arrayIndex">Starting index in destination array.</param>
        [PublicAPI]
        public void CopyTo(NpgsqlParameter[] array, int arrayIndex)
            => _internalList.CopyTo(array, arrayIndex);

        /// <summary>
        /// Convert collection to a System.Array.
        /// </summary>
        /// <returns>NpgsqlParameter[]</returns>
        [PublicAPI]
        public NpgsqlParameter[] ToArray() => _internalList.ToArray();

        internal void CloneTo(NpgsqlParameterCollection other)
        {
            other._internalList.Clear();
            foreach (var param in _internalList)
            {
                var newParam = param.Clone();
                newParam.Collection = this;
                other._internalList.Add(newParam);
            }
            other._lookup = _lookup;
            other._lookupIgnoreCase = _lookupIgnoreCase;
        }

        internal bool HasOutputParameters
        {
            get
            {
                foreach (var p in _internalList)
                    if (p.IsOutputDirection)
                        return true;
                return false;
            }
        }
    }
}
