using System.Text;

namespace RadFramework.Libraries.Serialization.Json.Parser
{
    public class JsonParserCursor
    {
        public string CurrentJson
        {
            get
            {
                return Json.Substring(Index);
            }
        }

        public char CurrentChar
        {
            get
            {
                return Json[Index];
            }
        }

        public string Json { get; set; }
        
        public int Index { get; set; }

        public JsonParserCursor(string json, int index)
        {
            Json = json;
            Index = index;
        }
        
        public void SkipObjectOrArray()
        {
            char currentChar = CurrentChar;
            
            if (!(currentChar.Equals('{') || currentChar.Equals('[')))
            {
                throw new ParserBugException("Expected char is { or [");
            }
            
            int nesting = 0;
            
            do
            {
                currentChar = CurrentChar;
                
                if (currentChar.Equals('{') || currentChar.Equals('['))
                {
                    nesting++;
                }
                else if (currentChar.Equals('}') || currentChar.Equals(']'))
                {
                    nesting--;
                }
                else if (currentChar.Equals('\"'))
                {
                    SkipString();
                }
                else if (currentChar.Equals(' '))
                {
                    SkipWhitespacesAndNewlines();
                    continue;
                }
                
                Index++;
                
            } while (nesting != 0);
        }
        
        public void SkipWhitespacesAndNewlines()
        {
            char currentChar = CurrentChar;
            
            while (currentChar.Equals(' ') || currentChar.Equals('\n') || currentChar.Equals('\r'))
            {
                Index++;
                currentChar = CurrentChar;
            }
        }
        
        public string ReadString()
        {
            if (!CurrentChar.Equals('"'))
            {
                throw new ParserBugException("Expected char is \"");
            }
            
            Index++;

            StringBuilder builder = new StringBuilder();
            
            while(true)
            {
                if (IsStringEnd())
                {
                    break;
                }

                builder.Append(CurrentChar);
                
                Index++;
            }
            
            Index++;

            return builder.ToString();
        }

        public void SkipString()
        {
            if (!CurrentChar.Equals('"'))
            {
                throw new ParserBugException("Expected char is \"");
            }
            
            // skip first "
            Index++;
            
            while(true)
            {
                if (IsStringEnd())
                {
                    break;
                }
                
                Index++;
            }
            
            Index++;
        }
        
        private bool IsStringEnd()
        {
            return CurrentChar == '"' 
                   && (Index != 0 && Json[Index - 1] != '\\');
        }
    }
}