using MPG_Interface.Module.Data;
using MPG_Interface.Module.Data.Input;
using MPG_Interface.Module.Data.Output;
using MPG_Interface.Module.Visual;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace MPG_Interface.Module.Logic
{

    /// <summary>
    /// 
    /// </summary>
    public class RestClient {

        /// <summary>
        /// 
        /// </summary>
        public delegate void OnStartCall();

        /// <summary>
        /// 
        /// </summary>
        public delegate void OnEndCall();

        public event OnStartCall StartCall;

        public event OnEndCall EndCall;

        private readonly HttpClient client;

        public static readonly RestClient Client = new();

        public RestClient() {
            client = FactoryData.CreateClient();
        }

        public Task<ServiceResponse<List<ReportCommand>>> GetReport(Period period) => CheckException(async () => {
            StartCall?.Invoke();

            NameValueCollection query = FactoryData.CreateQueryFromPeriod(period);
            string address = $"Report?{query}";
            var response = await client.GetFromJsonAsync<ServiceResponse<List<ReportCommand>>>(address);

            EndCall?.Invoke();
            return response;
        });

        public Task SendSettings(List<SettingsElement> elements) => CheckException(async () => {
            StringContent json = FactoryData.CreateBody(elements);
            string address = $"Settings";

            HttpResponseMessage response = await client.PutAsync(address, json);
            string data = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                var settingsResponse = JsonSerializer.Deserialize<ServiceResponse<bool>>(data);
                if (settingsResponse.Errors == null)
                    return settingsResponse.Data;
            } else {
                Alerts.ShowError(data);
            }

            return false;
        });

        public Task<ServiceResponse<List<SettingsElement>>> GetSettings() => CheckException(async () => {
            string address = $"Settings";

            var response = await client.GetFromJsonAsync<ServiceResponse<List<SettingsElement>>>(address);

            return response;
        });

        public Task<ServiceResponse<List<ReportMaterial>>> GetMaterialsForCommand(string POID) => CheckException(async () => {
            StartCall?.Invoke();

            string address = $"Report/materials/{POID}";
            var response = await client.GetFromJsonAsync<ServiceResponse<List<ReportMaterial>>>(address);

            EndCall?.Invoke();
            return response;
        });

        public Task<ServiceResponse<List<ReportMaterial>>> GetMaterialsForPail(string POID, int pail) => CheckException(async () => {
            StartCall?.Invoke();

            string address = $"Report/materials/{POID}/{pail}";
            var response = await client.GetFromJsonAsync<ServiceResponse<List<ReportMaterial>>>(address);

            EndCall?.Invoke();
            return response;
        });

        public Task<ServiceResponse<List<StatusCommand>>> GetStatusCommand(Period period) => CheckException(async () => {
            StartCall?.Invoke();

            NameValueCollection query = FactoryData.CreateQueryFromPeriod(period);
            string address = $"Production?{query}";
            var response = await client.GetFromJsonAsync<ServiceResponse<List<StatusCommand>>>(address);

            EndCall?.Invoke();
            return response;
        });

        public Task<ServiceResponse<List<ProductionOrder>>> GetCommands(Period period) => CheckException(async () => {
            StartCall?.Invoke();

            var query = FactoryData.CreateQueryFromPeriod(period);
            string address = $"Command?{query}";
            var response = await client.GetFromJsonAsync<ServiceResponse<List<ProductionOrder>>>(address);

            EndCall?.Invoke();
            return response;
        });

        public Task<bool> CheckPriority(string priority) => CheckException(async () => {
            string address = $"Command/Priority/{priority}";

            HttpResponseMessage response = await client.GetAsync(address);
            string data = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                return JsonSerializer.Deserialize<bool>(data);
            } else {
                Alerts.ShowError(data);
            }

            return false;
        });

        public Task<string> BlockCommand(string POID) => CheckException(async () => {
            StartCall?.Invoke();

            string result = string.Empty;
            string address = $"Command/Block/{POID}";

            HttpResponseMessage response = await client.DeleteAsync(address);
            string data = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                result = JsonSerializer.Deserialize<string>(data);
            } else {
                Alerts.ShowError(data);
            }

            EndCall?.Invoke();
            return result;
        });

        public Task<string> CloseCommand(string POID) => CheckException(async () => {
            StartCall?.Invoke();

            string result = string.Empty;
            string address = $"Command/Close/{POID}";

            HttpResponseMessage response = await client.PutAsync(address, null);
            string data = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                result = JsonSerializer.Deserialize<string>(data);
            } else {
                Alerts.ShowError(data);
            }

            EndCall?.Invoke();
            return result;
        });

        public Task<string> PartialProduction(string POID) => CheckException(async () => {
            StartCall?.Invoke();

            string result = string.Empty;
            string address = $"Command/Partial/{POID}";

            HttpResponseMessage response = await client.PutAsync(address, null);
            string data = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                result = JsonSerializer.Deserialize<string>(data);
            } else {
                Alerts.ShowError(data);
            }

            EndCall?.Invoke();
            return result;
        });

        public Task<string> DownloadMaterials() => CheckException(async () => {
            StartCall?.Invoke();

            string result = string.Empty;
            string address = $"Command/Materials";

            HttpResponseMessage response = await client.PostAsync(address, null);
            string data = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                result = JsonSerializer.Deserialize<string>(data);
            } else {
                Alerts.ShowError(data);
            }

            EndCall?.Invoke();
            return result;
        });

        public Task<string> StartCommand(StartCommand command) => CheckException(async () => {
            StartCall?.Invoke();

            string result = string.Empty;
            StringContent body = FactoryData.CreateBody(command);
            string address = $"Command/Start";

            HttpResponseMessage response = await client.PutAsync(address, body);
            string data = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                result = JsonSerializer.Deserialize<string>(data);
            } else {
                Alerts.ShowError(data);
            }

            EndCall?.Invoke();
            return result;
        });

        public Task<string> GetQC(string POID) => CheckException(async () => {
            string result = null;
            string address = $"Command/QC/{POID}";

            HttpResponseMessage response = await client.GetAsync(address);
            string data = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                result = JsonSerializer.Deserialize<string>(data);
            } else {
                Alerts.ShowError(data);
            }

            return result;
        });

        private async Task<T> CheckException<T>(Func<Task<T>> function) {
            try {
                return await function();
            } catch (Exception ex) {
                Alerts.ShowError(ex.Message);
            }

            EndCall?.Invoke();
            return default;
        }
    }
}
