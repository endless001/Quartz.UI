using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Quartz.Api.Infrastructure
{
    public class StandardHttpClient
    {
        private HttpClient _client;
        private ILogger<StandardHttpClient> _logger;

        public StandardHttpClient(ILogger<StandardHttpClient>  logger)
        {
            _logger = logger;
            _client = new HttpClient();
        }

        public async Task<HttpResponseMessage> GetStringAsync(string uri, Dictionary<string,string> headers)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    requestMessage.Headers.Add(item.Key, item.Value);
                }
               
            }

            var response = await _client.SendAsync(requestMessage);
            // raise exception if HttpResponseCode 500 
            // needed for circuit breaker to track fails

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }

            return response;
        }
        public async Task<HttpResponseMessage> DeleteAsync(string uri, Dictionary<string, string> headers)
        {


            // a new StringContent must be created for each retry
            // as it is disposed after each call


            var requestMessage = new HttpRequestMessage();

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    requestMessage.Headers.Add(item.Key, item.Value);
                }
            }

            var response = await _client.SendAsync(requestMessage);

            // raise exception if HttpResponseCode 500
            // needed for circuit breaker to track fails

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }

            return response;
        }
        public async Task<HttpResponseMessage> PostAsync<T>(string uri, T item, Dictionary<string, string> headers)
        {
            Func<HttpRequestMessage> func = () =>
            {
                return RequestMessage<T>(item, HttpMethod.Post, uri);
            };
            return await DoPostPutAsync(HttpMethod.Post, uri, func,headers);
        }

        public async Task<HttpResponseMessage> PostAsync(string uri, Dictionary<string, string> keys, Dictionary<string, string> headers)
        {

            Func<HttpRequestMessage> func = () =>
            {
                return RequestMessage(keys, HttpMethod.Post, uri);
            };
            return await DoPostPutAsync(HttpMethod.Post, uri, func,headers);
        }

        private HttpRequestMessage RequestMessage<T>(T item, HttpMethod method, string uri)
        {
            var requestMessage = new HttpRequestMessage(method, uri);
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json");
            return requestMessage;
        }
        private HttpRequestMessage RequestMessage(Dictionary<string, string> keys, HttpMethod method, string uri)
        {
            var requestMessage = new HttpRequestMessage(method, uri);
            requestMessage.Content = new FormUrlEncodedContent(keys);
            return requestMessage;
        }
        private async Task<HttpResponseMessage> DoPostPutAsync(HttpMethod method, string uri, Func<HttpRequestMessage> func, Dictionary<string, string> headers)
        {
            if (method != HttpMethod.Post && method != HttpMethod.Put)
            {
                throw new ArgumentException("Value must be either post or put.", nameof(method));
            }

            // a new StringContent must be created for each retry
            // as it is disposed after each call

 
             var requestMessage = func();
         
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    requestMessage.Headers.Add(item.Key, item.Value);
                }
            }

            var response = await _client.SendAsync(requestMessage);

            // raise exception if HttpResponseCode 500
            // needed for circuit breaker to track fails

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }

            return response;
        }
       

    }
}
