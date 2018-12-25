using System;
using System.Collections.Generic;
using System.Text;

namespace akkademy_db
{
    public class GetRequest
    {
        public readonly string Key;
        public GetRequest(string key)
        {
            Key = key;
        }
    }
}
