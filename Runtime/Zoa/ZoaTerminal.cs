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
            OSView.instance.GetSoftwareButton<ZoaTerminal>(force: true);
        }

        //----------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();
            trad_title.SetTrad("ZOA");
        }
    }
}