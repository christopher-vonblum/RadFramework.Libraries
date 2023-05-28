using System;

namespace JsonParser
{
    public class Lazy<T>
    {
        private readonly Func<T> _instanceFactory;
        private object _instance;
        
        public Lazy(Func<T> instanceFactory)
        {
            _instanceFactory = instanceFactory;
        }

        public Lazy(T instance)
        {
            this._instance = instance;
        }

        public T Value
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (T)_instanceFactory();
                }

                return (T)_instance;
            }
            set
            {
                _instance = value;
            }
        }
    }
}