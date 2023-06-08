namespace RadFramework.Libraries.Threading.Internals.ThreadAffinity
{
    public class BitMask
    {
        public int Value { get; private set; } = 0;

        public void Set(int cpu)
        {
            Value |= 1 << cpu;
        }

        public bool IsSet(int cpu)
        {
            if ((Value | (1 << cpu)) == Value)
            {
                return true;
            }

            return false;
        }
        
        public void Unset(int cpu)
        {
            Value -= 1 << cpu;
        }
    }
}