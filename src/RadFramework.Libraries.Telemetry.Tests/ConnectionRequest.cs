using System;
using MessagePack;

namespace Tests
{
    [MessagePackObject]
    public class ConnectionRequest
    {
        [Key(0)]
        public virtual Guid ConnectionId { get; set; }
        [Key(1)]
        public virtual int ResourceCount { get; set; }
    }
}