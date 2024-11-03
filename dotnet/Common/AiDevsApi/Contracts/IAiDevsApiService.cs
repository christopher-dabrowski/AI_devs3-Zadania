using Common.AiDevsApi.Models;

namespace Common.AiDevsApi.Contracts;

public interface IAiDevsApiService
{
    Task<ApiResponse> VerifyTaskAnswerAsync<TAnswer>(
        TaskAnswer<TAnswer> answer,
        CancellationToken cancellationToken = default);
}