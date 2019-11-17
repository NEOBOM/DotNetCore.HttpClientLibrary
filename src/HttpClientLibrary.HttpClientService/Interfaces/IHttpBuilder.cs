using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientLibrary.HttpClientService.Interfaces
{
    public interface IHttpBuilder
    {
        HttpResponseMessage GetSync(string uri, CancellationToken cancellationToken = default);

        Task<HttpResponseMessage> GetAsync(string uri, CancellationToken cancellationToken = default);

        HttpResponseMessage PostSync(string uri, string content, CancellationToken cancellationToken = default);

        Task<HttpResponseMessage> PostAsync(string uri, string content, CancellationToken cancellationToken = default);

        HttpResponseMessage PutSync(string uri, string content, CancellationToken cancellationToken = default);

        Task<HttpResponseMessage> PutAsync(string uri, string content, CancellationToken cancellationToken = default);

        HttpResponseMessage DeleteSync(string uri, CancellationToken cancellationToken = default);

        Task<HttpResponseMessage> DeleteAsync(string uri, CancellationToken cancellationToken = default);
    }
}
