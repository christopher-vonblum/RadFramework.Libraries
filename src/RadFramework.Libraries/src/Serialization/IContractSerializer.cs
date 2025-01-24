namespace RadFramework.Libraries.Serialization;

public interface IContractSerializer
{
    byte[] Serialize(Type type, object model);
    object Deserialize(Type type, byte[] data);
    object Clone(Type type, object model);
}