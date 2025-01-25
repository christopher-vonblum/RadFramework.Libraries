using RadFramework.Libraries.Reflection.Caching;

namespace RadFramework.Libraries.Serialization;

public interface IContractSerializer
{
    byte[] Serialize(CachedType type, object model);
    object Deserialize(CachedType type, byte[] data);
    object Clone(CachedType type, object model);
}