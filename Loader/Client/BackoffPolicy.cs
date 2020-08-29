using Meyer.Common.HttpClient;
using Meyer.Common.HttpClient.Policies;
using Polly;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Meyer.BallChasing.Client
{
    public class BackOffRetryPolicy : IRetryPolicy
    {
        private int retryCount;

        public BackOffRetryPolicy(int retryCount = 10)
        {
            this.retryCount = retryCount;
        }

        public async Task<HttpClientResponse<R>> Execute<R>(Func<Task<HttpClientResponse<R>>> request)
        {
            return await Polly.Policy
                .Handle<IOException>()
                .Or<WebException>(e =>
                    e.Status == WebExceptionStatus.RequestCanceled ||
                    e.Status == WebExceptionStatus.SendFailure ||
                    e.Status == WebExceptionStatus.ConnectFailure ||
                    e.Status == WebExceptionStatus.Pending)
                .Or<TaskCanceledException>()
                .Or<HttpClientException>(e =>
                    e.HttpResponseMessage.StatusCode == HttpStatusCode.InternalServerError ||
                    e.HttpResponseMessage.StatusCode == HttpStatusCode.ServiceUnavailable ||
                    e.HttpResponseMessage.StatusCode == HttpStatusCode.GatewayTimeout ||
                    (int)e.HttpResponseMessage.StatusCode == 429
                )
                .WaitAndRetryAsync(retryCount, retryAttempt =>
                    TimeSpan.FromSeconds(retryAttempt * retryCount)
                )
                .ExecuteAsync(request);
        }
    }
}
