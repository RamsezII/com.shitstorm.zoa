#if UNITY_EDITOR
using UnityEngine;

namespace _ZOA_
{
    partial class ShellView
    {
        [ContextMenu(nameof(OnValidate))]
        private void OnValidate() => _OnValidate();
        protected virtual void _OnValidate()
        {
            if (didStart)
                lint_theme.RebuildDictionary();
        }
    }
}
#endif