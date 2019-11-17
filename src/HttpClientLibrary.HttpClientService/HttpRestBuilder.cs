using HttpClientLibrary.HttpClientService.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HttpClientLibrary.HttpClientService
{
    public class HttpRestBuilder : HttpClientBase, IHttpBuilder
    {
        public HttpRestBuilder(string baseAddress) : base(new Uri(baseAddress), "application/json")
        {
        }

        public HttpRestBuilder(Uri baseAddress) : base(baseAddress, "application/json")
        {
        }

        public HttpRestBuilder(HttpClientConfig httpClientConfig) : base(httpClientConfig)
        {
        }
    }
}
