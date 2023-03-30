using MPG_Interface.Module.Data.Output;

using System.Collections.Specialized;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Net.Http;
using System.Web;
using System;

namespace MPG_Interface.Module.Data {
    public class FactoryData {

        private FactoryData() {
        }

        public static HttpClient CreateClient() {
            HttpClient client = new();
            client.BaseAddress = new Uri(Properties.Resources.Service_URL);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = TimeSpan.FromMinutes(10);

            return client;
        }

        public static NameValueCollection CreateQueryFromPeriod(Period period) {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            Array.ForEach(period.GetType().GetProperties(),
                item => query.Add(item.Name, item.GetValue(period, null).ToString()));

            return query;
        }

        public static Period CreatePeriod(DateTime start, DateTime end) {
            return new() {
                StartDate = start,
                EndDate = end,
                PageSize = 10,
                PageNumber = 1
            };
        }

        public static StringContent CreateBody<T>(List<T> list) {
            StringContent json = new(JsonSerializer.Serialize(list));
            json.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return json;
        }

        public static StringContent CreateBody(StartCommand command) {
            StringContent json = new(JsonSerializer.Serialize(command));
            json.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return json;
        }
    }
}
