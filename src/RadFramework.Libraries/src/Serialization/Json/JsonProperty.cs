namespace JsonParser
{
    public class JsonProperty
    {
        public string Name { get; set; }

        public object Value
        {
            get
            {
                return _Value.Value;
            }
            set
            {
                _Value.Value = value;
            }
        }

        internal Lazy<object> _Value;

        public JsonProperty()
        {
            _Value = new Lazy<object>(null);
        }
    }
}