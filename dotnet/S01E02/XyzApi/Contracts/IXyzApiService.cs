using S01E02.XyzApi.Models;

namespace S01E02.XyzApi.Contracts;

public interface IXyzApiService
{
    Task<XyzMessage> VerifyMessageAsync(
        XyzMessage message,
        CancellationToken cancellationToken = default);
}
