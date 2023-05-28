using System;

namespace JsonParser
{
    public class ParserBugException : Exception
    {
        public ParserBugException(string message) : base(message)
        {
            
        }
    }
}