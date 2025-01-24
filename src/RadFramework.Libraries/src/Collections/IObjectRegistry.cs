﻿namespace RadFramework.Libraries.Threading.ObjectRegistries
{
    public interface IObjectRegistry<TReferenceType> : IEnumerable<TReferenceType>
        where TReferenceType : class
    {
        void Register(TReferenceType @object);
        bool IsRegistered(TReferenceType @object);
        bool Unregister(TReferenceType @object);
    }
}