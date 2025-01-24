namespace RadFramework.Libraries.Ioc.Registrations
{
    public class Lazy<T>
    {
        private readonly Func<T> factory;
        private T instance;
        public Lazy(Func<T> factory)
        {
            this.factory = factory;
        }
        public T Value
        {
            get
            {
                if (instance == null)
                {
                    instance = factory();
                }

                return instance;
            }
        }
    }
}