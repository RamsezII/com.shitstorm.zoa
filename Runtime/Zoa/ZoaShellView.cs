namespace _ZOA_
{
    public class ZoaShellView : ShellView
    {

        //----------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();
            shell = new ZoaShell();
        }
    }
}