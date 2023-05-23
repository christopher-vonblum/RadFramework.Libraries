using System;
using MessagePack;

namespace Tests
{
    [MessagePackObject]
    public class ResourceConnectionRequest
    {
        [Key(0)]
        public virtual Guid ConnectionId { get; set; }
    }
}