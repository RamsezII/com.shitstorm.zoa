using _ARK_;

namespace _ZOA_
{
    partial class Shell
    {
        public string workdir = ArkPaths.instance.Value.dpath_home;

        //----------------------------------------------------------------------------------------------------------

        public void RefreshPrefixe()
        {
            prefixe.Value = new(
                text: $"{ArkMachine.user_name.Value}:{workdir}$ ",
                lint: $"{ArkMachine.user_name.Value.SetColor("#73CC26")}:{workdir.SetColor("#73B2D9")}$ "
            );
        }
    }
}