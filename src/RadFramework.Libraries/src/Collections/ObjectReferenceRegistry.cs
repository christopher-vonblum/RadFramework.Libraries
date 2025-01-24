using System.Collections;

namespace RadFramework.Libraries.Collections
{
    /// <summary>
    /// A registry that holds references to objects with capabilities to check for objects and remove them.
    /// </summary>
    /// <typeparam name="TReferenceType">The type of the references that the registry holds</typeparam>
    public class ObjectReferenceRegistry<TReferenceType> : IEnumerable<TReferenceType>
    {
        /// <summary>
        /// The dictionary that holds the references.
        /// Value is sbyte so we dont waste resources.
        /// </summary>
        private readonly Dictionary<TReferenceType, sbyte> registry = new Dictionary<TReferenceType, sbyte>();

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
            lock(registry)
                registry[@object] = 0;
        }

        /// <summary>
        /// Checks if an object is registered.
        /// </summary>
        /// <param name="object">The object to check for</param>
        /// <returns>True if the object was registered</returns>
        public bool IsRegistered(TReferenceType @object)
        {
            return registry.ContainsKey(@object);
        }

        /// <summary>
        /// Unregisters an object.
        /// </summary>
        /// <param name="object">The object to unregister.</param>
        /// <returns>True if the object was registered</returns>
        public bool Unregister(TReferenceType @object)
        {
            lock (registry)
                return registry.Remove(@object);
        }

        public IEnumerator<TReferenceType> GetEnumerator()
        {
            return registry.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return registry.Keys.GetEnumerator();
        }
    }
}