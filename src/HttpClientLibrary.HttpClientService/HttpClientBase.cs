using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientLibrary.HttpClientService
{
    public abstract class HttpClientBase
    {
        protected readonly HttpClient _httpClient;

        protected WebProxy _webProxy = null;
        protected CookieCollection _cookies = null;
        protected HttpContent _content = null;

        protected string _mediaTypeAccept = null;
        protected string _contentType = null;
        protected int _maxConnectionsPerServer = 1500;
        protected int _maxAutomaticRedirections = 3;
        protected string _bearerToken = null;
        protected string _browserUserAgent = null;
        protected TimeSpan _timeOut = TimeSpan.FromMilliseconds(3000);

        public HttpClientBase(Uri baseAddress) : this(baseAddress, "text/html")
        {
        }

        public HttpClientBase(Uri baseAddress, string mediaTypeAccept)
        {
            _mediaTypeAccept = mediaTypeAccept;
            _contentType = mediaTypeAccept;

            _httpClient = CreateHttpClienWithHandler(baseAddress);
        }

        public HttpClientBase(HttpClientConfig httpClientConfig)
        {
            _mediaTypeAccept = httpClientConfig.MediaTypeAccept;
            _contentType = httpClientConfig.ContentType;
            _webProxy = httpClientConfig.Proxy;
            _bearerToken = httpClientConfig.BearerToken;
            _timeOut = httpClientConfig.TimeOut;
            _maxConnectionsPerServer = httpClientConfig.MaxConnectionsPerServer;
            _maxAutomaticRedirections = httpClientConfig.MaxAutomaticRedirections;
            _browserUserAgent = httpClientConfig.BrowserUserAgent;

            _httpClient = CreateHttpClienWithHandler(httpClientConfig.BaseAddress);
        }

        /// <summary>
        /// Send a GET request to specified uri through synchronous operation
        /// </summary>
        /// <param name="uri">Uri request</param>
        /// <param name="cancellationToken">Cancellationtoken can be used to receive operation cancell</param>
        /// <returns></returns>
        public virtual HttpResponseMessage GetSync(string uri, CancellationToken cancellationToken = default)
        {
            return GetAsync(uri, cancellationToken).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Send a GET request to specified uri through asynchronous operation
        /// </summary>
        /// <param name="uri">Uri request</param>
        /// <param name="cancellationToken">Cancellationtoken can be used to receive operation cancell</param>
        /// <returns></returns>
        public virtual Task<HttpResponseMessage> GetAsync(string uri, CancellationToken cancellationToken = default)
        {
            return _httpClient.GetAsync(uri, cancellationToken);
        }

        /// <summary>
        /// Send a POST request to specified uri through synchronous operation
        /// </summary>
        /// <param name="uri">Uri request</param>
        /// <param name="content">Content to send</param>
        /// <param name="cancellationToken">Cancellationtoken can be used to receive operation cancell</param>
        /// <returns></returns>
        public virtual HttpResponseMessage PostSync(string uri, string content, CancellationToken cancellationToken = default)
        {
            return PostAsync(uri, content, cancellationToken).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Send a POST request to specified uri through asynchronous operation.
        /// </summary>
        /// <param name="uri">Uri request</param>
        /// <param name="content">Content to send</param>
        /// <param name="cancellationToken">Cancellationtoken can be used to receive operation cancell</param>
        /// <returns></returns>
        public virtual Task<HttpResponseMessage> PostAsync(string uri, string content, CancellationToken cancellationToken = default)
        {
            return _httpClient.PostAsync(uri, GetStringContent(content), cancellationToken);
        }

        /// <summary>
        /// Send a PUT request to specified uri through synchronous operation.
        /// </summary>
        /// <param name="uri">Uri request</param>
        /// <param name="content">Content to send</param>
        /// <param name="cancellationToken">Cancellationtoken can be used to receive operation cancell</param>
        /// <returns></returns>
        public virtual HttpResponseMessage PutSync(string uri, string content, CancellationToken cancellationToken = default)
        {
            return PutAsync(uri, content, cancellationToken).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Send a PUT request to specified uri through asynchronous operation.
        /// </summary>
        /// <param name="uri">Uri request</param>
        /// <param name="content">Content to send</param>
        /// <param name="cancellationToken">Cancellationtoken can be used to receive operation cancell</param>
        /// <returns></returns>
        public virtual Task<HttpResponseMessage> PutAsync(string uri, string content, CancellationToken cancellationToken = default)
        {
            return _httpClient.PutAsync(uri, GetStringContent(content), cancellationToken);
        }

        /// <summary>
        /// Send a DELETE request to specified uri through synchronous operation.
        /// </summary>
        /// <param name="uri">Uri request</param>
        /// <param name="cancellationToken">Cancellationtoken can be used to receive operation cancell</param>
        /// <returns></returns>
        public virtual HttpResponseMessage DeleteSync(string uri, CancellationToken cancellationToken = default)
        {
            return DeleteAsync(uri, cancellationToken).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Send a DELETE request to specified uri through asynchronous operation.
        /// </summary>
        /// <param name="uri">Uri request</param>
        /// <param name="cancellationToken">Cancellationtoken can be used to receive operation cancell</param>
        /// <returns></returns>
        public virtual Task<HttpResponseMessage> DeleteAsync(string uri, CancellationToken cancellationToken = default)
        {
            return _httpClient.DeleteAsync(uri, cancellationToken);
        }

        /// <summary>
        /// Send request to specified uri through synchronous operation.
        /// </summary>
        /// <param name="uri">Uri request</param>
        /// <param name="content">Content to send</param>
        /// <param name="httpMethod"></param>
        /// <param name="cancellationToken">Cancellationtoken can be used to receive operation cancell</param>
        /// <returns></returns>
        protected virtual HttpResponseMessage Send(string uri, string content, HttpMethod httpMethod, CancellationToken cancellationToken)
        {
            return SendAsync(uri, content, httpMethod, cancellationToken).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Send request to specified uri through asynchronous operation.
        /// </summary>
        /// <param name="uri">Uri request</param>
        /// <param name="content">Content to send</param>
        /// <param name="httpMethod"></param>
        /// <param name="cancellationToken">Cancellationtoken can be used to receive operation cancell</param>
        /// <returns></returns>
        protected virtual Task<HttpResponseMessage> SendAsync(string uri, string content, HttpMethod httpMethod, CancellationToken cancellationToken)
        {
            return _httpClient.SendAsync(CreateRequestMessage(uri, content, httpMethod), cancellationToken);
        }

        #region Private

        /// <summary>
        /// Create a httpClient with handler.
        /// </summary>
        /// <param name="baseAddress">Url for create a httpclient with base address</param>
        /// <returns></returns>
        private HttpClient CreateHttpClienWithHandler(Uri baseAddress)
        {
            var http = new HttpClient(CreateHttpClientHandler(baseAddress))
            {
                BaseAddress = baseAddress,
                Timeout = _timeOut
            };

            http.DefaultRequestHeaders.TryAddWithoutValidation("Accept", _mediaTypeAccept);
            http.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", _contentType);

            if (!string.IsNullOrEmpty(_browserUserAgent))
                http.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", _browserUserAgent);

            if (!string.IsNullOrEmpty(_bearerToken))
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);

            return http;
        }

        /// <summary>
        /// Create a HttpClientsHandler
        /// </summary>
        /// <param name="baseAddress">Base Address</param>
        /// <returns>HttpClientHandler</returns>
        private HttpClientHandler CreateHttpClientHandler(Uri baseAddress)
        {
            ServicePointManager.DefaultConnectionLimit = 15000;
            var handler = new HttpClientHandler();

            if (_webProxy != null)
            {
                handler.Proxy = _webProxy;
                handler.UseProxy = true;
                handler.PreAuthenticate = true;
                handler.UseDefaultCredentials = false;
            }

            if (_cookies != null)
            {
                handler.CookieContainer.Add(_cookies);
                handler.UseCookies = true;
            }

            if (baseAddress.AbsoluteUri.Contains("https"))
            {
                handler.SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
                handler.ServerCertificateCustomValidationCallback = ValidateServerCertificate;
            }

            handler.AllowAutoRedirect = _maxAutomaticRedirections > 0;
            handler.MaxAutomaticRedirections = _maxAutomaticRedirections;
            handler.MaxConnectionsPerServer = _maxConnectionsPerServer;
            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            return handler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="content">Content to send</param>
        /// <param name="method"></param>
        /// <returns></returns>
        private HttpRequestMessage CreateRequestMessage(string requestUri, string content, HttpMethod method)
        {
            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(requestUri),
                Content = GetStringContent(content)
            };

            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content">Content to convert</param>
        /// <returns></returns>
        private HttpContent GetStringContent(string content)
        {
            return (content != null) ? new StringContent(content, Encoding.UTF8, _contentType) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslpolicyerrors"></param>
        /// <returns></returns>
        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            return true;
        }

        #endregion

    }
}
