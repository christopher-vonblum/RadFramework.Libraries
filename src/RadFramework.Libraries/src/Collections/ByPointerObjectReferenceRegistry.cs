using System.Collections;
using System.Runtime.InteropServices;

namespace RadFramework.Libraries.Threading.ObjectRegistries
{
    /// <summary>
    /// A registry that holds references to objects with capabilities to check for objects and remove them.
    /// </summary>
    /// <typeparam name="TReferenceType">The type of the references that the registry holds</typeparam>
    public class ByPointerObjectReferenceRegistry<TReferenceType> : IEnumerable<TReferenceType>
    {
        /// <summary>
        /// The dictionary that holds the references.
        /// </summary>
        private readonly Dictionary<IntPtr, (GCHandle, TReferenceType)> registry = new Dictionary<IntPtr, (GCHandle, TReferenceType)>();

        /// <summary>
        /// The count of registered objects.
        /// </summary>
        public int Count => registry.Count;

        /// <summary>
        /// Registers an object.
        /// </summary>
        /// <param name="object">The object to register</param>
        public void Register(TReferenceType @object)
        {
            lock (registry)
            {
                GCHandle handle = GCHandle.Alloc(@object, GCHandleType.Pinned);
                registry[GCHandle.ToIntPtr(handle)] = (handle, @object);
            }
        }

        /// <summary>
        /// Checks if an object is registered.
        /// </summary>
        /// <param name="object">The object to check for</param>
        /// <returns>True if the object was registered</returns>
        public bool IsRegistered(TReferenceType @object)
        {
            GCHandle handle = GCHandle.Alloc(@object, GCHandleType.Pinned);
            bool containsObject = registry.ContainsKey(GCHandle.ToIntPtr(handle));
            handle.Free();
            return containsObject;
        }

        /// <summary>
        /// Unregisters an object.
        /// </summary>
        /// <param name="object">The object to unregister.</param>
        /// <returns>True if the object was registered</returns>
        public bool Unregister(TReferenceType @object)
        {
            lock (registry)
            {
                GCHandle handle = GCHandle.Alloc(@object, GCHandleType.Pinned);
                bool removed = registry.Remove(GCHandle.ToIntPtr(handle));
                handle.Free();
                return removed;
            }
        }

        public IEnumerator<TReferenceType> GetEnumerator()
        {
            return registry.Values.Select(v => v.Item2).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return registry.Values.Select(v => v.Item2).GetEnumerator();
        }
    }
}