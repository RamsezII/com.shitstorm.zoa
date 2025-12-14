using _ARK_;
using System;
using UnityEngine;

namespace _ZOA_
{
    partial class ShellView
    {
        public void SetStdinText(string text)
        {
            if (character_wrap)
                Util.ForceCharacterWrap(ref text);
            stdin_field.text = text;
        }

        public void SetStdinLint(string lint)
        {
            if (character_wrap)
                Util.ForceCharacterWrap(ref lint);
            stdin_field.lint.text = lint;
        }

        char OnValidateStdin_char(string text, int charIndex, char addedChar)
        {
            switch (shell.status._value)
            {
                case CMD_STATUS.WAIT_FOR_STDIN:
                    try
                    {
                        switch (addedChar)
                        {
                            case '\t':
                                OnTab();
                                return '\0';

                            case '\n':
                                OnSubmit();
                                ResetStdin();
                                return '\0';

                            case ' ' when character_wrap:
                                return Util.NOWRAP_CHAR;

                            default:
                                return addedChar;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e, this);
                        return '\0';
                    }

                case CMD_STATUS.BLOCKED:
                    ResetStdin();
                    return '\0';

                case CMD_STATUS.NETWORKING:
                    return '\0';

                default:
                    return '\0';
            }
        }

        void OnStdinChanged(string value)
        {
            if (!CheckPrefixe())
                return;

            if (!flag_history.PullValue())
                ResetHistoryNav();

            switch (shell.status._value)
            {
                case CMD_STATUS.WAIT_FOR_STDIN:
                    OnChange();
                    break;

                case CMD_STATUS.BLOCKED:
                    ResetStdin();
                    return;

                case CMD_STATUS.NETWORKING:
                    break;
            }
        }

        protected virtual void OnSelectStdin(string text)
        {
            IMGUI_global.instance.inputs_users.AddElement(OnImguiInputs);

            NUCLEOR.delegates.LateUpdate_onEndOfFrame_once += () =>
            {
                int min_pos = shell.prefixe._value.text?.Length ?? 0;
                if (stdin_field.caretPosition < min_pos)
                    stdin_field.caretPosition = min_pos;
            };
        }

        bool GetStdin(out string stdin, out int cursor_i)
        {
            int pref_len = shell.prefixe._value.text?.Length ?? 0;
            stdin = stdin_field.text[pref_len..];
            cursor_i = stdin_field.caretPosition - pref_len;
            Util.RemoveCharacterWrap(ref stdin);
            return !string.IsNullOrWhiteSpace(stdin);
        }

        void ResetStdin()
        {
            LintedString prefixe = GetShellPrefixe();
            string pref_text = prefixe.text;
            string pref_lint = prefixe.lint;

            if (character_wrap)
            {
                Util.ForceCharacterWrap(ref pref_text);
                Util.ForceCharacterWrap(ref pref_lint);
            }

            if (!stdin_field.text.Equals(pref_text, StringComparison.Ordinal))
                stdin_field.text = pref_text;

            stdin_field.lint.text = pref_lint;

            stdin_field.caretPosition = pref_text.Length;

            ResizeStdin();
        }

        void ResizeStdin()
        {
            Rect prect = scrollview.viewport.rect;

            float stdin_h = stdin_field.textComponent.GetInvisibleHeight();

            float bottom_height = content_rT.anchoredPosition.y - stdout_h - stdin_h - offset_bottom_h + prect.height;
            stdin_h = Mathf.Max(stdin_h, prect.height);

            stdin_field.rT.sizeDelta = new(0, stdin_h);
            content_rT.sizeDelta = new(0, stdout_h + stdin_h);

            if (bottom_height < 0)
                content_rT.anchoredPosition += new Vector2(0, -bottom_height);
        }

        bool CheckPrefixe()
        {
            string current = stdin_field.text;
            string prefixe = GetShellPrefixe().text;

            if (character_wrap)
                Util.ForceCharacterWrap(ref prefixe);

            if (current.StartsWith(prefixe, StringComparison.Ordinal))
                return true;

            if (current.Length < prefixe.Length)
                current = prefixe;
            else
                current = prefixe + current[prefixe.Length..];

            if (!string.Equals(current, stdout_field.text, StringComparison.Ordinal))
                stdin_field.text = current;

            if (stdin_field.caretPosition < prefixe.Length)
                stdin_field.caretPosition = prefixe.Length;

            return false;
        }
    }
}