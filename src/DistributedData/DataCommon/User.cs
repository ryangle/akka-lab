using System;

namespace DataCommon
{
    public class User
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Count { get; set; }
        public override int GetHashCode()
        {
            return (Name + Id).GetHashCode();
        }
        public override bool Equals(object obj)
        {
            var o = obj as User;
            if (o != null)
            {
                return o.Name == Name && o.Id == Id;
            }
            return false;
        }
    }
}
