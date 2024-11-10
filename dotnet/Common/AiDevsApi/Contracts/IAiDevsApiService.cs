using Common.AiDevsApi.Models;

namespace Common.AiDevsApi.Contracts;

public interface IAiDevsApiService
{
    const string VerifyEndpoint = "/verify";

    Task<ApiResponse> VerifyTaskAnswerAsync<TAnswer>(
        TaskAnswer<TAnswer> answer,
        string endpoint = VerifyEndpoint,
        CancellationToken cancellationToken = default);
}
