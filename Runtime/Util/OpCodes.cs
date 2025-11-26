using System;

namespace _ZOA_
{
    public enum OP_CODES : byte
    {
        _none_,
        BEFORE,
        AFTER,
        ASSIGN,
        NOT,
        ADD, SUBSTRACT,
        MULTIPLY, DIVIDE, MODULO,
        EQUAL, GREATER_THAN, LESSER_THAN,
        AND, OR, XOR,
    }

    [Flags]
    public enum OP_FLAGS : ushort
    {
        _none_,
        BEFORE = 1 << OP_CODES.BEFORE,
        AFTER = 1 << OP_CODES.AFTER,
        ASSIGN = 1 << OP_CODES.ASSIGN,
        NOT = 1 << OP_CODES.NOT,
        ADD = 1 << OP_CODES.ADD,
        SUBSTRACT = 1 << OP_CODES.SUBSTRACT,
        MULTIPLY = 1 << OP_CODES.MULTIPLY,
        DIVIDE = 1 << OP_CODES.DIVIDE,
        MODULO = 1 << OP_CODES.MODULO,
        EQUAL = 1 << OP_CODES.EQUAL,
        NOT_EQUAL = NOT | EQUAL,
        GREATER_THAN = 1 << OP_CODES.GREATER_THAN,
        LESSER_THAN = 1 << OP_CODES.LESSER_THAN,
        GREATER_OR_EQUAL = GREATER_THAN | EQUAL,
        LESS_OR_EQUAL = LESSER_THAN | EQUAL,
        AND = 1 << OP_CODES.AND,
        OR = 1 << OP_CODES.OR,
        XOR = 1 << OP_CODES.XOR,
    }
}