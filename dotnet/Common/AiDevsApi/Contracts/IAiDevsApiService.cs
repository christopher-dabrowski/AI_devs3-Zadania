using Common.AiDevsApi.Models;

namespace Common.AiDevsApi.Contracts;

public interface IAiDevsApiService
{
    const string VerifyEndpoint = "/verify";
    const string ReportEndpoint = "/report";

    HttpClient HttpClient { get; }

    Task<ApiResponse> VerifyTaskAnswerAsync<TAnswer>(
        TaskAnswer<TAnswer> answer,
        string endpoint = ReportEndpoint,
        CancellationToken cancellationToken = default);
}
