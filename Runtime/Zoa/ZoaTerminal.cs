using _ARK_;
using _SGUI_;
using UnityEngine;

namespace _ZOA_
{
    public partial class ZoaTerminal : SguiWindow1
    {

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            var button = OSView.instance.AddSoftwareButton<ZoaTerminal>(new("ZoaTerminal"));
            ArkShortcuts.AddShortcut(
                shortcutName: "Zoa",
                nameof_button: "o",
                action: () => button.InstantiateSoftware()
            );
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void OnAwake()
        {
            base.OnAwake();
            trad_title.SetTrad("ZOA");
        }
    }
}