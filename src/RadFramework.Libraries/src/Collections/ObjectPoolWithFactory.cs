using RadFramework.Libraries.Threading.ObjectRegistries;
using RadFramework.Libraries.Threading.Timers;

namespace RadFramework.Libraries.Threading.ObjectPools
{
    /// <summary>
    /// A simple object pool that maintains instances that can be reserved or wait in the pool.
    /// </summary>
    /// <typeparam name="TObject">The type of the objects to pool.</typeparam>
    public class ObjectPoolWithFactory<TObject> : IDisposable where TObject : class
    {
        /// <summary>
        /// Delegate that constructs a pool object.
        /// </summary>
        private readonly Func<TObject> _createObject;
        
        /// <summary>
        /// Delegate that disposes a pool object.
        /// </summary>
        private readonly Action<TObject> _disposeObject;
        
        /// <summary>
        /// The optimal size of the pool.
        /// </summary>
        private readonly int _optimalPoolSize;

        /// <summary>
        /// A dictionary that holds reference to pooled objects.
        /// </summary>
        private readonly ObjectReferenceRegistry<TObject> objectPoolRegistry = new ObjectReferenceRegistry<TObject>();
        
        /// <summary>
        /// A dictionary that holds reference to reserved objects that return to the pool later.
        /// </summary>
        private readonly ObjectReferenceRegistry<TObject> reservedPoolRegistry = new ObjectReferenceRegistry<TObject>();

        /// <summary>
        /// A timer that triggers PerformCleanup() in order to reach the optimal pool size.
        /// </summary>
        private readonly JobTimer _cleanupTimerBase;
        
        /// <summary>
        /// Indicates if the pool is disposed.
        /// </summary>
        private bool isDisposed;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="createObject">A delegate that constructs a pool object.</param>
        /// <param name="disposeObject">A delegate that disposes the pool object.</param>
        /// <param name="optimalPoolSize">The optimal count of pool instances.</param>
        public ObjectPoolWithFactory(Func<TObject> createObject, Action<TObject> disposeObject, int optimalPoolSize)
        {
            _createObject = createObject;
            _disposeObject = disposeObject;
            _optimalPoolSize = optimalPoolSize;
            _cleanupTimerBase = new JobTimer(5000, PerformCleanup, ThreadPriority.Normal);
        }

        /// <summary>
        /// Reserves and returns an object.
        /// The object either comes from the pool or is freshly created.
        /// </summary>
        /// <returns>A pool object.</returns>
        /// <exception cref="ObjectDisposedException">If the pool is disposed and this method gets called.</exception>
        public TObject Reserve()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("Release was called on a disposed ObjectPool.");
            }
            
            // lock pool and reservedPool
            lock (objectPoolRegistry)
            lock (reservedPoolRegistry)
            {
                // no objects available
                if (objectPoolRegistry.Count == 0)
                {
                    // construct object
                    var newObject = _createObject();
                
                    // add it to the reserved pool
                    reservedPoolRegistry.Register(newObject);
                    
                    // return to caller
                    return newObject;
                }

                // randomly pick object from pool
                TObject pooledInstance = objectPoolRegistry.First();
                
                // remove it from the pool and store it in reservedPool
                objectPoolRegistry.Unregister(pooledInstance);
                reservedPoolRegistry.Register(pooledInstance);
                
                // return to caller
                return pooledInstance;
            }
        }

        /// <summary>
        /// Releases a pool object back to the pool.
        /// </summary>
        /// <param name="object">The object to release.</param>
        /// <exception cref="ObjectDisposedException">If the pool is disposed and this method gets called.</exception>
        public void Release(TObject @object)
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("Release was called on a disposed ObjectPool.");
            }
            
            // Lock the whole pool.
            lock (objectPoolRegistry)
            lock (reservedPoolRegistry)
            {
                // add instance back to the pool
                objectPoolRegistry.Register(@object);
                
                // remove reservation of the object
                reservedPoolRegistry.Unregister(@object);
            }
        }

        /// <summary>
        /// Cleans the pool in an interval powered by a timer.
        /// </summary>
        private void PerformCleanup()
        {
            if (isDisposed)
            {
                return;
            }
            
            int overallInstanceCount = objectPoolRegistry.Count + reservedPoolRegistry.Count;
            
            if (overallInstanceCount > _optimalPoolSize)
            {
                int removeCount = overallInstanceCount - _optimalPoolSize;
                for (int i = 0; i < removeCount; i++)
                {
                    lock (objectPoolRegistry)
                    {
                        TObject obj = objectPoolRegistry.FirstOrDefault();

                        if (obj != null)
                        {
                            objectPoolRegistry.Unregister(obj);
                            _disposeObject(obj);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Disposes the object pool.
        /// </summary>
        public void Dispose()
        {
            isDisposed = true;
            _cleanupTimerBase.Dispose();

            lock (objectPoolRegistry)
            lock (reservedPoolRegistry)
            {
                foreach (var obj in objectPoolRegistry.Concat(reservedPoolRegistry))
                {
                    // execute the disposal delegate for each object in the pool.
                    _disposeObject(obj);
                }                
            }
        }
    }
}