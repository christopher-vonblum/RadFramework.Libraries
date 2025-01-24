namespace RadFramework.Libraries.Telemetry
{
    class PackageShedulerRouterMock : ITelemtryPackageThreadShedulerRouter
    {
        private readonly IThreadSheduler _sheduler;

        public PackageShedulerRouterMock(IThreadSheduler sheduler)
        {
            _sheduler = sheduler;
        }
        
        public IThreadSheduler GetShedulerByPackageKind(PackageKind packageKind)
        {
            return _sheduler;
        }
    }
}