using System;
using System.Collections.Generic;
using System.Text;

namespace akkademy_db
{
    public class KeyNotFoundException : Exception
    {
        public readonly string Key;
        public KeyNotFoundException(string key)
            : base($"Key:{key} Not Found")
        {
            Key = key;
        }
    }
}
