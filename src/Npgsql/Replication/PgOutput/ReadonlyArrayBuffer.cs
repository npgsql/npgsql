using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql.Replication.PgOutput.Messages;

namespace Npgsql.Replication.PgOutput
{
    class ReadOnlyArrayBuffer<T> : IReadOnlyList<T>
    {
        public static readonly ReadOnlyArrayBuffer<T> Empty = new(Array.Empty<T>());
        T[] _items;
        int _size;

        public ReadOnlyArrayBuffer()
            => _items = Array.Empty<T>();

        ReadOnlyArrayBuffer(T[] items)
        {
            _items = items;
            _size = items.Length;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < _size; i++)
            {
                yield return _items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count
        {
            get => _size;
            internal set
            {
                if (_items.Length < value)
                    _items = new T[value];

                _size = value;
            }
        }

        public T this[int index]
        {
            get => index < _size ? _items[index] : throw new IndexOutOfRangeException();
            internal set => _items[index] = value;
        }

        public ReadOnlyArrayBuffer<T> Clone()
        {
            var newItems = new T[_size];
            if (_size > 0)
                Array.Copy(_items, newItems, _size);
            return new(newItems);
        }
    }
}
