using System;

namespace Npgsql.Linq
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    class DbFunctionStoreNameAttribute : Attribute
    {
        public string StoreName { get; set; }

        public DbFunctionStoreNameAttribute(string storeName)
        {
            StoreName = storeName;
        }
    }
}