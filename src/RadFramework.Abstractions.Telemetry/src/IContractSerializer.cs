using System;

namespace RadFramework.Libraries.Telemetry
{
    
    public interface IContractSerializer
    {
        byte[] Serialize(Type telemtryContract, object model);
        object Deserialize(Type telemtryContract, byte[] data);
    }
}