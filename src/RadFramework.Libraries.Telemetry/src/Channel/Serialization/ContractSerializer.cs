using System;
using MessagePack;

namespace RadFramework.Libraries.Telemetry
{
    public class ContractSerializer : IContractSerializer
    {
        public byte[] Serialize(Type telemtryContract, object model)
        {
            return MessagePackSerializer.NonGeneric.Serialize(telemtryContract, model);
        }

        public object Deserialize(Type telemtryContract, byte[] data)
        {
            return MessagePackSerializer.NonGeneric.Deserialize(telemtryContract, data);
        }
    }
}