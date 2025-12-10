#if UNITY_EDITOR
using TMPro;
using UnityEditor;

public static partial class Util_zoa_e
{
    [MenuItem("CONTEXT/" + nameof(TMP_InputField) + "/" + nameof(ToZSpaced))]
    static void ToZSpaced(MenuCommand command)
    {
        TMP_InputField inputfield = (TMP_InputField)command.context;
        inputfield.text = inputfield.text.ZSpaced();
    }

    [MenuItem("CONTEXT/" + nameof(TMP_InputField) + "/" + nameof(RemoveZSpaces))]
    static void RemoveZSpaces(MenuCommand command)
    {
        TMP_InputField inputfield = (TMP_InputField)command.context;
        inputfield.text = inputfield.text.UnZSpaced();
    }
}
#endif