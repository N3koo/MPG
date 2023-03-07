using System.Net.Http.Headers;
using System.Net.Http;
using System;

namespace MPG_Interface.Module.Data {
    public class FactoryData {

        private FactoryData() {
        }

        public static HttpClient CreateClient() {
            HttpClient client = new();
            client.BaseAddress = new Uri(Properties.Resources.Service_URL);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

    }
}
