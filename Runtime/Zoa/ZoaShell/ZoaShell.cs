using _SGUI_;
using UnityEngine;

namespace _ZOA_
{
    public sealed partial class ZoaShell : Shell
    {

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            SguiTerminal.onSoftwareButtonAddWindowList += list =>
            {
                var button = list.AddButton();
                button.trad.SetTrads(new("ZOA"));
                button.button.onClick.AddListener(() =>
                {
                    SguiTerminal terminal = (SguiTerminal)OSView.instance.softwaresButtons[typeof(SguiTerminal)].InstantiateSoftware();
                    ShellView shellView = terminal.rt_shellview.gameObject.AddComponent<ShellView>();
                    shellView.shell = new ZoaShell();
                });
            };
        }

        //----------------------------------------------------------------------------------------------------------

        public ZoaShell()
        {
            front_execution = null;
        }
    }
}