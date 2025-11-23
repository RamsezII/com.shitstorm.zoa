using System;

namespace _ZOA_
{
    partial class ShellView
    {
        void OnChange()
        {
            if (GetStdin(out string stdin, out int cursor))
            {
                Signal signal = new(SIG_FLAGS.LINT, stdin, cursor);
                shell.OnSignal(signal);

                string new_stdin = shell.prefixe._value.text + signal.output_text;
                string new_lint = shell.prefixe._value.lint + signal.output_lint;

                if (!stdin_field.text.Equals(new_stdin, StringComparison.Ordinal))
                    stdin_field.text = new_stdin;
                stdin_field.lint.text = new_lint;
            }
            else
                stdin_field.lint.text = shell.prefixe._value.lint;

            ResizeStdin();
        }

        void OnTab()
        {

        }

        void OnSubmit()
        {
            if (GetStdin(out string stdin, out int cursor))
            {
                Signal sig_check = new(SIG_FLAGS.CHECK | SIG_FLAGS.LINT, stdin, cursor);
                shell.OnSignal(sig_check);

                AddLine(shell.prefixe._value.text + sig_check.output_text, shell.prefixe._value.lint + sig_check.output_lint);

                Signal sig_exec = new(SIG_FLAGS.EXEC, stdin, cursor);
                shell.OnSignal(sig_exec);
            }
        }
    }
}