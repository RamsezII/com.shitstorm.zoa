using _COBRA_;

namespace _ZOA_
{
    partial class ShellView
    {
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

                Signal sig_check = new(shell, SIG_FLAGS.CHECK, reader1, null);
                shell.OnSignal(sig_check);

                if (sig_check.reader.sig_error != null)
                {
                    sig_check.reader.LocalizeError();
                    AddLine(sig_check.reader.sig_long_error, sig_check.reader.sig_long_error.SetColor(Colors.orange));
                    return;
                }

                CodeReader reader2 = new(
                    lint_theme: lint_theme,
                    strict_syntax: false,
                    text: stdin,
                    script_path: null,
                    cursor_i: cursor
                );

                Signal sig_exec = new(shell, SIG_FLAGS.STDIN, reader2, AddLine);
                shell.OnSignal(sig_exec);

                if (sig_exec.reader.sig_error != null)
                {
                    sig_exec.reader.LocalizeError();
                    AddLine(sig_exec.reader.sig_long_error, sig_exec.reader.sig_long_error.SetColor(Colors.orange_red));
                    return;
                }

                AddToHistory(reader2.text);
            }
        }
    }
}