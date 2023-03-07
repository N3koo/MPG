using MPG_Interface.Module.Data;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.Json;
using System.Net.Http;
using System.Web;
using System;
using MPG_Interface.Module.Visual.ViewModel;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace MPG_Interface.Module.Logic {

    /// <summary>
    /// 
    /// </summary>
    public delegate void OnStartCall();

    /// <summary>
    /// 
    /// </summary>
    public delegate void OnEndCall();

    /// <summary>
    /// 
    /// </summary>
    public class RestClient {

        public static readonly RestClient Client = new();

        public event OnStartCall StartCall;
        public event OnEndCall EndCall;

        private readonly HttpClient client;

        public RestClient() {
            client = FactoryData.CreateClient();
        }

        public async Task<List<ReportCommand>> GetReport(DateTime start, DateTime end) {
            StartCall?.Invoke();

            List<ReportCommand> result = new();
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["start"] = start.ToString(CultureInfo.InvariantCulture);
            query["end"] = end.ToString(CultureInfo.InvariantCulture);
            string address = $"Report?{query}";

            HttpResponseMessage response = await client.GetAsync(address);

            if (response.IsSuccessStatusCode) {
                string data = await response.Content.ReadAsStringAsync();
                result.AddRange(JsonSerializer.Deserialize<List<ReportCommand>>(data));
            }

            EndCall?.Invoke();
            return result;
        }

        public async Task SendSettings(List<SettingsElement> elements) {
            var json = new StringContent(JsonSerializer.Serialize(elements));
            json.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            string address = $"Settings";

            HttpResponseMessage response = await client.PutAsync(address, json);

            if (response.IsSuccessStatusCode) {
                Debug.WriteLine("A mers");
            }

            Debug.WriteLine("Nu a mers");
        }

        public async Task<List<SettingsElement>> GetSettings() {
            List<SettingsElement> result = new();
            string address = $"Settings";

            HttpResponseMessage response = await client.GetAsync(address);

            if (response.IsSuccessStatusCode) {
                string data = await response.Content.ReadAsStringAsync();
                result.AddRange(JsonSerializer.Deserialize<List<SettingsElement>>(data));
            }

            return result;
        }

        public async Task<List<ReportMaterial>> GetMaterialsForCommand(string POID) {
            StartCall?.Invoke();

            List<ReportMaterial> result = new();
            string address = $"Report/materials/{POID}";
            HttpResponseMessage response = await client.GetAsync(address);

            if (response.IsSuccessStatusCode) {
                string data = await response.Content.ReadAsStringAsync();
                result.AddRange(JsonSerializer.Deserialize<List<ReportMaterial>>(data));
            }

            EndCall?.Invoke();
            return result;
        }

        public async Task<List<ReportMaterial>> GetMaterialsForPail(string POID, int pail) {
            StartCall?.Invoke();

            List<ReportMaterial> result = new();
            string address = $"Report/materials/{POID}/{pail}";

            HttpResponseMessage response = await client.GetAsync(address);

            if (response.IsSuccessStatusCode) {
                string data = await response.Content.ReadAsStringAsync();
                result.AddRange(JsonSerializer.Deserialize<List<ReportMaterial>>(data));
            }

            EndCall?.Invoke();
            return result;
        }

        public async Task<List<StatusCommand>> GetStatusCommand(DateTime start, DateTime end) {
            StartCall?.Invoke();

            List<StatusCommand> result = new();
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["start"] = start.ToString(CultureInfo.InvariantCulture);
            query["end"] = end.ToString(CultureInfo.InvariantCulture);
            string address = $"Production?{query}";

            HttpResponseMessage response = await client.GetAsync(address);

            if (response.IsSuccessStatusCode) {
                string data = await response.Content.ReadAsStringAsync();
                result.AddRange(JsonSerializer.Deserialize<List<StatusCommand>>(data));
            }

            EndCall?.Invoke();
            return new List<StatusCommand>();
        }

        public async Task<List<ProductionOrder>> GetCommands(DateTime start, DateTime end) {
            StartCall?.Invoke();

            List<ProductionOrder> result = new();
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["start"] = start.ToString(CultureInfo.InvariantCulture);
            query["end"] = end.ToString(CultureInfo.InvariantCulture);
            query["PageNumber"] = "2";
            query["PageSize"] = "10";

            string address = $"Command?{query}";

            Debug.WriteLine(address);

            HttpResponseMessage response = await client.GetAsync(address);

            if (response.IsSuccessStatusCode) {
                string data = await response.Content.ReadAsStringAsync();
                result.AddRange(JsonSerializer.Deserialize<List<ProductionOrder>>(data));
            }

            EndCall?.Invoke();
            return result;
        }

        public async Task<bool> CheckPriority(string priority) {
            string address = $"Command/Priority/{priority}";

            HttpResponseMessage response = await client.GetAsync(address);

            if (response.IsSuccessStatusCode) {
                string data = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<bool>(data);
            }

            return false;
        }

        public async Task<bool> BlockCommand(string POID) {
            StartCall?.Invoke();

            bool result = false;
            string address = $"Command/Block/{POID}";

            HttpResponseMessage response = await client.DeleteAsync(address);

            if (response.IsSuccessStatusCode) {
                string data = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<bool>(data);
            }

            EndCall?.Invoke();
            return result;
        }

        public async Task<bool> CloseCommand(string POID) {
            StartCall?.Invoke();

            bool result = false;
            string address = $"Command/Close/{POID}";

            HttpResponseMessage response = await client.PutAsync(address, null);

            if (response.IsSuccessStatusCode) {
                string data = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<bool>(data);
            }

            EndCall?.Invoke();
            return result;
        }

        public async Task<bool> PartialProduction(string POID) {
            StartCall?.Invoke();

            bool result = false;
            string address = $"Command/Partial/{POID}";

            HttpResponseMessage response = await client.PutAsync(address, null);

            if (response.IsSuccessStatusCode) {
                string data = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<bool>(data);
            }

            EndCall?.Invoke();
            return result;
        }

        public async Task<bool> DownloadMaterials() {
            StartCall?.Invoke();

            bool result = false;
            string address = $"Command/Materials";

            HttpResponseMessage response = await client.PostAsync(address, null);

            if (response.IsSuccessStatusCode) {
                string data = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<bool>(data);
            }

            EndCall?.Invoke();
            return result;
        }

        public async Task<bool> StartCommand(string POID) {
            StartCall?.Invoke();

            bool result = false;
            string address = $"Command/Start/{POID}";

            HttpResponseMessage response = await client.PutAsync(address, null);

            if (response.IsSuccessStatusCode) {
                string data = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<bool>(data);
            }

            EndCall?.Invoke();
            return result;
        }

        public async Task<string> GetQC(string POID) {
            string result = null;
            string address = $"Command/QC/{POID}";

            HttpResponseMessage response = await client.GetAsync(address);

            if (response.IsSuccessStatusCode) {
                string data = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<string>(data);
            }

            return result;
        }
    }
}
