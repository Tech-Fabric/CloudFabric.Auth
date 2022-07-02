using System;
using System.Net.Http;

namespace CloudFabric.Auth.Middleware
{
    public class IntrospectionHttpClientProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Func<HttpClient> _httpClientFactoryFunc;

        public IntrospectionHttpClientProvider(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IntrospectionHttpClientProvider(Func<HttpClient> httpClientFactoryFunc)
        {
            _httpClientFactoryFunc = httpClientFactoryFunc;
        }

        public HttpClient GetHttpClient()
        {
            if (_httpClientFactory != null)
            {
                return _httpClientFactory.CreateClient("CloudFabricJwtValidationMiddleware-IntrospectionClient");
            }
            
            if (_httpClientFactoryFunc != null)
            {
                return _httpClientFactoryFunc.Invoke();
            }

            throw new Exception("Can't create a client since both httpClientFactory and httpClientFactoryFunc are null");
        }
    }
}