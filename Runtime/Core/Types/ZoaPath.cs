namespace _ZOA_
{
    public readonly struct ZoaPath
    {
        public readonly string _value;
        public override readonly string ToString() => _value;
        public ZoaPath(in string path) => _value = path;
        public static implicit operator ZoaPath(in string path) => new(path);
        public static implicit operator string(in ZoaPath path) => path._value;
    }

    public readonly struct ZoaFPath
    {
        public readonly string _value;
        public override readonly string ToString() => _value;
        public ZoaFPath(in string path) => _value = path;
        public static implicit operator ZoaFPath(in string path) => new(path);
        public static implicit operator string(in ZoaFPath path) => path._value;
    }

    public readonly struct ZoaDPath
    {
        public readonly string _path;
        public override readonly string ToString() => _path;
        public ZoaDPath(in string path) => _path = path;
        public static implicit operator ZoaDPath(in string path) => new(path);
        public static implicit operator string(in ZoaDPath path) => path._path;

        [UnityEditor.MenuItem("Assets/" + nameof(_ZOA_) + "/" + nameof(_PathTest))]
        static void _PathTest()
        {
            ZoaPath a = new("D:/projects");
            object b = a;
            string c = b.ToString();
            UnityEngine.Debug.Log(c);
        }
    }
}