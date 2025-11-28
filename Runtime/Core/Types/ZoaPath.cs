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

    public struct ZoaFPath
    {
        public string _value;

        //----------------------------------------------------------------------------------------------------------

        public ZoaFPath(string path)
        {
            _value = path;
        }

        //----------------------------------------------------------------------------------------------------------

        public static implicit operator ZoaFPath(in string path) => new(path);
        public static implicit operator string(in ZoaFPath path) => path._value;
    }

    public struct ZoaDPath
    {
        public string _value;

        //----------------------------------------------------------------------------------------------------------

        public ZoaDPath(string path)
        {
            _value = path;
        }

        //----------------------------------------------------------------------------------------------------------

        public static implicit operator ZoaDPath(in string path) => new(path);
        public static implicit operator string(in ZoaDPath path) => path._value;
    }
}