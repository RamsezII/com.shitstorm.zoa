using _ARK_;
using _SGUI_;
using UnityEngine;

namespace _ZOA_
{
    partial class ShellView
    {
        bool OnClipboardOperation(Event e, IMGUI_global.ClipboardOperations operation)
        {
            switch (operation)
            {
                case IMGUI_global.ClipboardOperations.Copy:
                    OnCtrlC();
                    return true;

                case IMGUI_global.ClipboardOperations.Cut:
                    OnCtrlX();
                    return true;

                case IMGUI_global.ClipboardOperations.Paste:
                    OnCtrlV();
                    return true;
            }
            return false;
        }

        void OnCtrlC()
        {
            LoggerOverlay.Log($"COPY ({transform.GetPath(true)})");
        }

        void OnCtrlV()
        {
            LoggerOverlay.Log($"PASTE ({transform.GetPath(true)})");
        }

        void OnCtrlX()
        {
            LoggerOverlay.Log($"CUT ({transform.GetPath(true)})");
        }
    }
}