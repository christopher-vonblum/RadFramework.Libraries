using System;
using MessagePack;

namespace RadFramework.Libraries.Telemetry
{
    [MessagePackObject]
    public class InterlinkedPackage : NestedPackage
    {
        [Key(2)]
        public Guid Id { get; set; }
    }
}