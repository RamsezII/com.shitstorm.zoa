namespace _ZOA_
{
    public struct ZoaPath
    {
        public string _value;

        //----------------------------------------------------------------------------------------------------------

        public ZoaPath(string path)
        {
            _value = path;
        }

        //----------------------------------------------------------------------------------------------------------

        public static implicit operator ZoaPath(in string path) => new(path);
        public static implicit operator string(in ZoaPath path) => path._value;
    }
}