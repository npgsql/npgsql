// created on 29/11/2007

// Npgsql.Cache.cs
//
// Author:
//    Glen Parker (glenebob@gmail.com)
//    Ben Sagal (bensagal@gmail.com)
//    Tao Wang (dancefire@gmail.com)
//
//    Copyright (C) 2007 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
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

namespace Npgsql
{
    internal class Cache<TEntity>
    {
        private class LRUKeyEntityPair
        {
            internal int LRUKey;
            internal readonly TEntity Entity;

            internal LRUKeyEntityPair(int lruKey, TEntity entity)
            {
                this.LRUKey = lruKey;
                this.Entity = entity;
            }
        }

        private int _cache_size;
        private Dictionary<string, LRUKeyEntityPair> table;
        private SortedDictionary<int, string> lru;
        private int nextLRUKey = 0;

        /// <summary>
        /// Set Cache Size. The default value is 20.
        /// </summary>
        public int CacheSize
        {
            get { return _cache_size; }
            set
            {
                if (value < 0) { throw new ArgumentOutOfRangeException("CacheSize"); }

                lock (this)
                {
                    _cache_size = value;

                    while (_cache_size < this.Count)
                    {
                        SortedDictionary<int, string>.KeyCollection.Enumerator it = lru.Keys.GetEnumerator();

                        it.MoveNext();

                        int lruKey = it.Current;

                        table.Remove(lru[lruKey]);
                        lru.Remove(lruKey);
                    }
                }
            }
        }

        /// <summary>
        /// Lookup cached entity. null will returned if not match.
        /// For both get{} and set{} apply LRU rule.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public TEntity this[string key]
        {
            get
            {
                TEntity existing;

                if (TryGetValue(key, out existing))
                {
                    return existing;
                }

                throw new KeyNotFoundException();
            }
            set
            {
                lock (this)
                {
                    LRUKeyEntityPair existing;

                    if (table.TryGetValue(key, out existing))
                    {
                        if (existing.LRUKey < nextLRUKey - 1)
                        {
                            lru.Remove(existing.LRUKey);
                            existing.LRUKey = nextLRUKey;
                            lru.Add(existing.LRUKey, key);
                            nextLRUKey++;
                        }
                    }
                    else
                    {
                        if (this.CacheSize > 0)
                        {
                            if (table.Count == this.CacheSize)
                            {
                                SortedDictionary<int, string>.KeyCollection.Enumerator it = lru.Keys.GetEnumerator();

                                it.MoveNext();

                                int lruKey = it.Current;

                                table.Remove(lru[lruKey]);
                                lru.Remove(lruKey);
                            }
                        }

                        table.Add(key, new LRUKeyEntityPair(nextLRUKey, value));
                        lru.Add(nextLRUKey, key);
                        nextLRUKey++;
                    }
                }
            }
        }

        public bool TryGetValue(string key, out TEntity value)
        {
            lock (this)
            {
                LRUKeyEntityPair existing;

                if (table.TryGetValue(key, out existing))
                {
                    if (existing.LRUKey < nextLRUKey - 1)
                    {
                        lru.Remove(existing.LRUKey);
                        existing.LRUKey = nextLRUKey;
                        lru.Add(nextLRUKey, key);
                        nextLRUKey++;
                    }

                    value = existing.Entity;

                    return true;
                }
            }

            value = default(TEntity);

            return false;
        }

        public int Count
        {
            get { return table.Count; }
        }

        public Cache()
        : this(20)
        {
        }

        public Cache(int cacheSize)
        {
            this._cache_size = cacheSize;
            table = new Dictionary<string, LRUKeyEntityPair>();
            lru = new SortedDictionary<int,string>();
        }
    }
}
