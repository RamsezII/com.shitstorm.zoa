using _UTIL_;
using System;
using UnityEngine;

namespace _ZOA_
{
    public readonly struct ZoaNumber : IEquatable<ZoaNumber>
    {
        public readonly object _value;
#if UNITY_EDITOR
        public object _Value => _value;
        [ShowProperty(nameof(_Value))] public object _show_value => _value;
#endif

        //----------------------------------------------------------------------------------------------------------

        public ZoaNumber(in object value)
        {
            _value = value;
            Type type = value.GetType();
            if (type != typeof(int) && type != typeof(float))
                throw new InvalidCastException($"{typeof(ZoaNumber)} expected {typeof(int)} or {typeof(float)}, got {type}");
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/" + nameof(_ZOA_) + "/" + nameof(_Test))]
        static void _Test()
        {
            ZoaNumber a = 5f;
            ZoaNumber b = 5;
            float c = a + b;
            Debug.Log(a == b);
            Debug.Log(a == 5);
            Debug.Log(a == 5f);
            Debug.Log(c);
        }
#endif

        //----------------------------------------------------------------------------------------------------------

        public static ZoaNumber operator +(in ZoaNumber a, in ZoaNumber b) => Operation(a, b, OP_FLAGS.ADD);
        public static ZoaNumber operator -(in ZoaNumber a, in ZoaNumber b) => Operation(a, b, OP_FLAGS.SUBSTRACT);
        public static ZoaNumber operator *(in ZoaNumber a, in ZoaNumber b) => Operation(a, b, OP_FLAGS.MULTIPLY);
        public static ZoaNumber operator /(in ZoaNumber a, in ZoaNumber b) => Operation(a, b, OP_FLAGS.DIVIDE);
        public static bool operator ==(in ZoaNumber a, in ZoaNumber b) => a.Equals(b);
        public static bool operator !=(in ZoaNumber a, in ZoaNumber b) => !a.Equals(b);

        public static implicit operator ZoaNumber(in int v) => new(v);
        public static implicit operator ZoaNumber(in float v) => new(v);

        public static implicit operator float(in ZoaNumber z) => z._value switch
        {
            int i => i,
            float f => f,
            _ => throw new InvalidCastException()
        };

        public static explicit operator int(in ZoaNumber z) => z._value switch
        {
            int i => i,
            float f => (int)f,
            _ => throw new InvalidCastException()
        };

        public override int GetHashCode() => _value.GetHashCode();

        public override bool Equals(object obj) => base.Equals((ZoaNumber)obj);
        public bool Equals(ZoaNumber other)
        {
            switch (_value)
            {
                case int ai:
                    switch (other._value)
                    {
                        case int bi:
                            return ai == bi;
                        case float bf:
                            return ai == bf;
                    }
                    break;

                case float af:
                    switch (other._value)
                    {
                        case int bi:
                            return af == bi;
                        case float bf:
                            return af == bf;
                    }
                    break;
            }

            return false;
        }

        public static bool TryConvert(in ZoaNumber a, in ZoaNumber b, out int conv_a, out int conv_b)
        {
            if (a._value is int a1 && b._value is int b1)
            {
                conv_a = a1;
                conv_b = b1;
                return true;
            }
            conv_a = default;
            conv_b = default;
            return false;
        }

        public static bool TryConvert(in ZoaNumber a, in ZoaNumber b, out float conv_a, out float conv_b)
        {
            if (a._value is float a1 && b._value is float b1)
            {
                conv_a = a1;
                conv_b = b1;
                return true;
            }
            conv_a = default;
            conv_b = default;
            return false;
        }

        public static bool TryConvert(in ZoaNumber a, in ZoaNumber b, out int conv_a, out float conv_b)
        {
            if (a._value is int a1 && b._value is float b1)
            {
                conv_a = a1;
                conv_b = b1;
                return true;
            }
            conv_a = default;
            conv_b = default;
            return false;
        }

        public static bool TryConvert(in ZoaNumber a, in ZoaNumber b, out float conv_a, out int conv_b)
        {
            if (a._value is float a1 && b._value is int b1)
            {
                conv_a = a1;
                conv_b = b1;
                return true;
            }
            conv_a = default;
            conv_b = default;
            return false;
        }

        public static ZoaNumber Operation(in ZoaNumber a, in ZoaNumber b, in OP_FLAGS code)
        {
            var exc = new InvalidOperationException($"invalid operation \"{code}\" on {a._value.GetType()} and {b._value.GetType()}");

            if (a._value is int a1 && b._value is int b1)
                return new(code switch
                {
                    OP_FLAGS.ADD => a1 + b1,
                    OP_FLAGS.SUBSTRACT => a1 - b1,
                    OP_FLAGS.MULTIPLY => a1 * b1,
                    OP_FLAGS.DIVIDE => a1 / b1,
                    OP_FLAGS.MODULO => a1 % b1,
                    OP_FLAGS.EQUAL => a1 == b1,
                    OP_FLAGS.GREATER_THAN => a1 > b1,
                    OP_FLAGS.GREATER_OR_EQUAL => a1 >= b1,
                    OP_FLAGS.LESSER_THAN => a1 < b1,
                    OP_FLAGS.LESS_OR_EQUAL => a1 <= b1,
                    OP_FLAGS.AND => a1 & b1,
                    OP_FLAGS.OR => a1 | b1,
                    OP_FLAGS.XOR => a1 ^ b1,
                    _ => throw exc,
                });

            if (a._value is float a2 && b._value is float b2)
                return new(code switch
                {
                    OP_FLAGS.ADD => a2 + b2,
                    OP_FLAGS.SUBSTRACT => a2 - b2,
                    OP_FLAGS.MULTIPLY => a2 * b2,
                    OP_FLAGS.DIVIDE => a2 / b2,
                    OP_FLAGS.MODULO => a2 % b2,
                    OP_FLAGS.EQUAL => a2 == b2,
                    OP_FLAGS.GREATER_THAN => a2 > b2,
                    OP_FLAGS.GREATER_OR_EQUAL => a2 >= b2,
                    OP_FLAGS.LESSER_THAN => a2 < b2,
                    OP_FLAGS.LESS_OR_EQUAL => a2 <= b2,
                    _ => throw exc,
                });

            if (a._value is int a3 && b._value is float b3)
                return new(code switch
                {
                    OP_FLAGS.ADD => a3 + b3,
                    OP_FLAGS.SUBSTRACT => a3 - b3,
                    OP_FLAGS.MULTIPLY => a3 * b3,
                    OP_FLAGS.DIVIDE => a3 / b3,
                    OP_FLAGS.MODULO => a3 % b3,
                    OP_FLAGS.EQUAL => a3 == b3,
                    OP_FLAGS.GREATER_THAN => a3 > b3,
                    OP_FLAGS.GREATER_OR_EQUAL => a3 >= b3,
                    OP_FLAGS.LESSER_THAN => a3 < b3,
                    OP_FLAGS.LESS_OR_EQUAL => a3 <= b3,
                    _ => throw exc,
                });

            if (a._value is float a4 && b._value is int b4)
                return new(code switch
                {
                    OP_FLAGS.ADD => a4 + b4,
                    OP_FLAGS.SUBSTRACT => a4 - b4,
                    OP_FLAGS.MULTIPLY => a4 * b4,
                    OP_FLAGS.DIVIDE => a4 / b4,
                    OP_FLAGS.MODULO => a4 % b4,
                    OP_FLAGS.EQUAL => a4 == b4,
                    OP_FLAGS.GREATER_THAN => a4 > b4,
                    OP_FLAGS.GREATER_OR_EQUAL => a4 >= b4,
                    OP_FLAGS.LESSER_THAN => a4 < b4,
                    OP_FLAGS.LESS_OR_EQUAL => a4 <= b4,
                    _ => throw exc,
                });

            throw exc;
        }
    }
}