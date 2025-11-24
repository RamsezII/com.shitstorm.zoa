using System;
using System.Collections.Generic;

namespace _ZOA_
{
    sealed partial class ZoaShell : Shell
    {
        /*

            Instruction
            │
            ├── Assignation (ex: x = ...)
            │     └── Expression
            │           └── ...
            │
            └── Expression
                └── Or
                    └── And
                        └── Comparison
                            └── Addition (addition, subtraction)
                                └── Term (multiplication, division, modulo)
                                    └── Facteur
                                        ├── Littéral (nombre)
                                        ├── Variable
                                        ├── Parenthèse
                                        └── Appel de fonction

        */

        enum INSTR_CODES : byte
        {
            _none_,
            Block,
            Instruction,
            Expression,
            Assignation,
            Or,
            And,
            Comparison,
            Addition,
            Term,
            Literal,
            Variable,
            Parenthesis,
            FunctionCall,
        }

        enum OP_CODES : byte
        {
            assign,
            not,
            add, sub,
            mul, div, mod,
            eq, gt, lt,
            and, or, xor,
        }

        [Flags]
        public enum OP_FLAGS : ushort
        {
            unknown,
            assign = 1 << OP_CODES.assign,
            not = 1 << OP_CODES.not,
            add = 1 << OP_CODES.add,
            sub = 1 << OP_CODES.sub,
            mul = 1 << OP_CODES.mul,
            div = 1 << OP_CODES.div,
            mod = 1 << OP_CODES.mod,
            eq = 1 << OP_CODES.eq,
            neq = not | eq,
            gt = 1 << OP_CODES.gt,
            lt = 1 << OP_CODES.lt,
            ge = gt | eq,
            le = lt | eq,
            and = 1 << OP_CODES.and,
            or = 1 << OP_CODES.or,
            xor = 1 << OP_CODES.xor,
        }

        //----------------------------------------------------------------------------------------------------------

        public override void OnSignal(in Signal signal)
        {
            if (TryParseBlock(signal, new MemScope(mem_scope), out Executor executor) && signal.reader.sig_error == null)
                front_executor = executor;
            else
                signal.reader.sig_error ??= $"could not parse {nameof(signal)}";
        }
    }
}