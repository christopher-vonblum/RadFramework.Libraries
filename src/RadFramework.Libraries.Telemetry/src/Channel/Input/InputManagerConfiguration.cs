using System;

namespace RadFramework.Libraries.Telemetry
{
    public class InputManagerConfiguration
    {
        public Action<byte[]> OnPackageReceived { get; set; }
    }
}