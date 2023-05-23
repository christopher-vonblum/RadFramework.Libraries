using MessagePack;

namespace RadFramework.Libraries.Telemetry
{
    [MessagePackObject]
    public class PayloadPackage : NestedPackage, IPayloadPackage
    {
        [Key(2)]
        public PackageKind PackageKind { get; set; }
    }
}