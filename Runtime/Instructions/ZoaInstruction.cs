namespace _ZOA_
{
    public abstract class ZoaInstruction
    {

    }

    public sealed class ContractCall : ZoaInstruction
    {
        public readonly Contract contract;
    }
}