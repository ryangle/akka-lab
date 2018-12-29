using System;
using System.Collections.Generic;
using System.Text;

namespace akkademy_db
{
    public class SetRequest
    {
        public readonly string Key;
        public readonly object Value;
        public SetRequest(string key, object value)
        {
            Key = key;
            Value = value;
        }
        public override string ToString()
        {
            return $"Key:{Key},value:{Value}";
        }
    }
}
