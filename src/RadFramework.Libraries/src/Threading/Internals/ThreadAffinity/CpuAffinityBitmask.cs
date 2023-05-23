namespace RadFramework.Libraries.Threading.Internals.ThreadAffinity
{
    public class CpuAffinityBitmask
    {
        private int processors = 0;

        public void Set(int cpu)
        {
            processors |= processors << cpu;
        }

        public void Unset(int cpu)
        {
            processors &= processors << cpu;
        }
    }
}