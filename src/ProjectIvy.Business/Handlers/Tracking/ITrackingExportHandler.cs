using System.Threading;
using System.Threading.Tasks;

namespace ProjectIvy.Business.Handlers.Tracking;

public interface ITrackingExportHandler : IHandler
{
    Task<byte[]> GetMap(DateOnly date, string timezone = null, CancellationToken cancellationToken = default);
}
