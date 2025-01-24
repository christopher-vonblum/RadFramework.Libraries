namespace RadFramework.Libraries.Serialization.Json
{
    public class JsonProperty : IJsonObjectTreeModel
    {
        public string Name { get; set; }

        public object Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }

        internal object _Value;

        public JsonProperty()
        {
            _Value = null;
        }
    }
}