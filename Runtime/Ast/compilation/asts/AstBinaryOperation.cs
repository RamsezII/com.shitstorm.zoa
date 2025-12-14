using _ZOA_.Ast.execution;

namespace _ZOA_.Ast.compilation
{
    internal class AstBinaryOperation : AstExpression
    {
        protected enum OpDepth : byte
        {
            Or,
            And,
            Comp,
            Add,
        }

        readonly OpDepth depth;
        readonly AstExpression astL, astR;

        //----------------------------------------------------------------------------------------------------------

        protected AstBinaryOperation(in OpDepth depth, in AstExpression astL, in AstExpression astR) : base(typeof(bool))
        {
            this.depth = depth;
            this.astL = astL;
            this.astR = astR;
        }

        //----------------------------------------------------------------------------------------------------------

        public static bool TryParseOr(in Signal signal, in TScope tscope, in TStack tstack, out AstExpression ast) => TryParseBinaryOperation(signal, tscope, tstack, OpDepth.Or, out ast);
        static bool TryParseBinaryOperation(in Signal signal, in TScope tscope, in TStack tstack, in OpDepth op_depth, out AstExpression ast_binaryOperation)
        {
            switch (op_depth)
            {
                case OpDepth.Or:
                case OpDepth.And:
                    if (TryParseBinaryOperation(signal, tscope, tstack, op_depth + 1, out ast_binaryOperation))
                        if (!signal.reader.TryReadString_match_out(out string op_name, as_function_argument: false, lint: signal.reader.lint_theme.keywords, match: "or"))
                            return true;
                        else
                        {
                            if (TryParseBinaryOperation(signal, tscope, tstack, op_depth, out var astR))
                            {
                                ast_binaryOperation = new AstBinaryOperation(op_depth, ast_binaryOperation, astR);
                                return true;
                            }
                            else
                                signal.reader.Error($"expected expression after '{op_name}' operator.");
                        }
                    break;

                case OpDepth.Comp:
                    break;

                case OpDepth.Add:
                    break;
            }

            signal.reader.Error($"could not parse expression");
            ast_binaryOperation = null;
            return false;
        }

        //----------------------------------------------------------------------------------------------------------

        internal override void OnExecution(in Janitor janitor)
        {
            base.OnExecution(janitor);
        }
    }
}