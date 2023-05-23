using RadFramework.Libraries.Threading;

namespace RadFramework.Libraries.Telemetry
{
    public interface ITelemtryPackageThreadShedulerRouter
    {
        IThreadSheduler GetShedulerByPackageKind(PackageKind packageKind);
    }
}