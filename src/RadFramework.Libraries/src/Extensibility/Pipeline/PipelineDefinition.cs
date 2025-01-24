using System;
using System.Collections.Generic;

namespace RadFramework.Libraries.Extensibility.Pipeline
{
    [Serializable]
    public class PipelineDefinition
    {
        public IEnumerable<PipeDefinition> Definitions => definitions;
        private List<PipeDefinition> definitions = new();

        public PipelineDefinition(IEnumerable<PipeDefinition> definitions)
        {
            this.definitions = new List<PipeDefinition>(definitions);
        }

        public PipelineDefinition()
        {
        }
        
        public void InsertAfter<TPipe>(string afterKey, string key = null)
        {
            int afterIndex = definitions.FindIndex(definition => definition.Key == afterKey);
            definitions.Insert(afterIndex + 1, new PipeDefinition(typeof(TPipe), key));
        }
        
        public void InsertAfter<TAfter, TPipe>(string key = null)
        {
            int afterIndex = definitions.FindIndex(definition => definition.Type == typeof(TAfter));
            definitions.Insert(afterIndex + 1, new PipeDefinition(typeof(TPipe), key));
        }

        public void InsertBefore<TPipe>(string beforeKey, string key = null)
        {
            int afterIndex = definitions.FindIndex(definition => definition.Key == beforeKey);
            definitions.Insert(afterIndex - 1, new PipeDefinition(typeof(TPipe), key));
        }
        
        public void InsertBefore<TBefore, TPipe>(string key = null)
        {
            int afterIndex = definitions.FindIndex(definition => definition.Type == typeof(TBefore));
            definitions.Insert(afterIndex - 1, new PipeDefinition(typeof(TPipe), key));
        }

        public void Replace<TReplace, TPipe>(string key = null)
        {
            int replaceIndex = definitions.FindIndex(definition => definition.Type == typeof(TReplace));
            definitions.RemoveAt(replaceIndex);
            definitions.Insert(replaceIndex, new PipeDefinition(typeof(TPipe), key));
        }
        
        public void Prepend<TPipe>(string key = null)
        {
            definitions.Insert(0, new PipeDefinition(typeof(TPipe), key));
        }
        
        public void Append<TPipe>(string key = null)
        {
            definitions.Add(new PipeDefinition(typeof(TPipe), key));
        }
        public void Append(Type tPipe, string key = null)
        {
            definitions.Add(new PipeDefinition(tPipe, key));
        }
    }
}