using _COBRA_;

namespace _ZOA_
{
    partial class ShellView
    {
        void OnChange()
        {
            if (GetStdin(out string stdin, out int cursor))
            {
                CodeReader reader = new(
                    lint_theme: lint_theme,
                    strict_syntax: false,
                    text: stdin,
                    script_path: null,
                    cursor_i: cursor
                );

                Signal sig_change = new(SIG_FLAGS.LINT, reader);
                shell.OnSignal(sig_change);

                stdin_field.lint.text = shell.prefixe._value.lint + sig_change.reader.GetLintResult();
            }
            else
                ResetStdin();

            ResizeStdin();
        }

        void OnTab()
        {

        }

        void OnSubmit()
        {
            if (GetStdin(out string stdin, out int cursor))
            {
                AddLine(stdin_field.text, stdin_field.lint.text);

                CodeReader reader1 = new(
                    lint_theme: lint_theme,
                    strict_syntax: false,
                    text: stdin,
                    script_path: null,
                    cursor_i: cursor
                );

                Signal sig_check = new(SIG_FLAGS.CHECK | SIG_FLAGS.LINT, reader1);
                shell.OnSignal(sig_check);

                if (sig_check.reader.sig_error != null)
                {
                    AddLine(sig_check.reader.sig_error, sig_check.reader.sig_error.SetColor(Colors.orange));
                    return;
                }

                CodeReader reader2 = new(
                    lint_theme: lint_theme,
                    strict_syntax: false,
                    text: stdin,
                    script_path: null,
                    cursor_i: cursor
                );

                Signal sig_exec = new(SIG_FLAGS.EXEC, reader2);
                shell.OnSignal(sig_exec);
            }
        }
    }
}