using AiDevsApi.Models;

namespace ApiTestTask.AiDevsApi.Contracts;

public interface IAiDevsApiService
{
    Task<ApiResponse> VerifyTaskAnswerAsync<TAnswer>(
        TaskAnswer<TAnswer> answer,
        CancellationToken cancellationToken = default);
}