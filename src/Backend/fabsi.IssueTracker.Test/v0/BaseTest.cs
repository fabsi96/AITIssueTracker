using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AITIssueTracker.Test.v0
{
    public class BaseTest
    {
        protected HttpClient ApiClient
        {
            get
            {
                return client;
            }

            set
            {
                HttpClientHandler clientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
                };
                client = new HttpClient(clientHandler) { BaseAddress = new Uri($"{ServerIp}:{ServerPort}"), Timeout = TimeSpan.FromSeconds(15) };
            }
        }

        protected string Version
        {
            set
            {
                client.DefaultRequestHeaders.Remove("api-version");
                client.DefaultRequestHeaders.Add("api-version", value);
            }
        }


        private HttpClient client;

        private string ServerIp { get; }

        private int ServerPort { get; }


        public BaseTest(string ip, int port)
        {
            ServerIp = ip;
            ServerPort = port;
            ApiClient = new HttpClient();
        }
    }
}
