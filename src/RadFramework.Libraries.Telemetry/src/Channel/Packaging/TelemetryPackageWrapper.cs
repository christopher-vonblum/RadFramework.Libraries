using System;

namespace RadFramework.Libraries.Telemetry
{
    public class TelemetryPackageWrapper : ITelemetryPackageWrapper
    {
        private readonly IContractSerializer _modelSerializer;

        public TelemetryPackageWrapper(IContractSerializer modelSerializer)
        {
            _modelSerializer = modelSerializer;
        }

        public IPayloadPackage Wrap(PackageKind packageKind, object payload, Guid? packageId = null)
        {
            object package = payload;
            
            if (packageId.HasValue)
            {
                Type payloadType = package.GetType();
                
                package = new InterlinkedPackage
                {
                    Id = packageId.Value,
                    PayloadType = payloadType.AssemblyQualifiedName,
                    Payload = _modelSerializer.Serialize(payloadType, package)
                };
            }

            Type packageType = package.GetType();
            
            return new PayloadPackage
            {
                PackageKind = packageKind,
                PayloadType = packageType.AssemblyQualifiedName,
                Payload = _modelSerializer.Serialize(packageType, package)
            };
        }

        
        
        public (PackageKind packageType, object payload, Guid? packageId) Unwrap(byte[] package)
        {
            IPayloadPackage wrapperPackage = (IPayloadPackage)_modelSerializer.Deserialize(typeof(PayloadPackage), package);

            Guid? packageId = new Guid?();

            object payload = _modelSerializer.Deserialize(Type.GetType(wrapperPackage.PayloadType), wrapperPackage.Payload);
            
            if (payload is InterlinkedPackage interlinkedPackage)
            {
                packageId = interlinkedPackage.Id;
                payload = _modelSerializer.Deserialize(Type.GetType(interlinkedPackage.PayloadType), interlinkedPackage.Payload);
            }
            
            return (wrapperPackage.PackageKind, payload, packageId);
        }
    }
}