using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HttpClientLibrary.HttpClientService
{
    public sealed class HttpClientConfig
    {
        public HttpClientConfig(string baseAddress)
        {
            BaseAddress = new Uri(baseAddress);
        }

        public HttpClientConfig(string baseAddress, string contentType)
        {
            BaseAddress = new Uri(baseAddress);
            ContentType = contentType;
        }

        public HttpClientConfig(string baseAddress, string contentType, string mediaTypeAccept)
        {
            BaseAddress = new Uri(baseAddress);
            ContentType = contentType;
            MediaTypeAccept = mediaTypeAccept;
        }

        /// <summary>
        /// Url base address
        /// </summary>
        public Uri BaseAddress { get; private set; }

        /// <summary>
        /// Content type
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string MediaTypeAccept { get; private set; }

        /// <summary>
        /// Bearer Token
        /// </summary>
        public string BearerToken { get; set; }

        /// <summary>
        /// TimeOut request.
        /// </summary>
        public TimeSpan TimeOut { get; set; }

        /// <summary>
        /// Max connection per server.
        /// </summary>
        public int MaxConnectionsPerServer { get; set; }

        /// <summary>
        /// Proxy to request.
        /// </summary>
        public WebProxy Proxy { get; private set; }

        /// <summary>
        /// Cookies
        /// </summary>
        public CookieCollection Cookies { get; private set; }

        /// <summary>
        /// Max automatic redirections
        /// </summary>
        public int MaxAutomaticRedirections { get; set; }

        /// <summary>
        /// Browser user agent request.
        /// </summary>
        public string BrowserUserAgent { get; set; }

        /// <summary>
        /// Add proxy for request.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public void AddProxy(string uri, string userName = "", string password = "")
        {
            Proxy = new WebProxy(uri, false)
            {
                Address = new Uri(uri),
                UseDefaultCredentials = false,
                Credentials = string.IsNullOrEmpty(userName) ? null : new NetworkCredential(userName, password)
            };
        }

        /// <summary>
        /// Create a new cookie into collection cookies
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <param name="domain"></param>
        public void AddCookie(string name, string value, string path = null, string domain = null)
        {
            Cookie cookie = null;

            if (path == null && domain == null)
                cookie = new Cookie(name, value);
            else
                cookie = new Cookie(name, value, path, domain);

            if (Cookies == null)
                Cookies = new CookieCollection();

            Cookies.Add(cookie);
        }
    }
}
