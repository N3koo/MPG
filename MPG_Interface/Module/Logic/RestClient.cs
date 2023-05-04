using MPG_Interface.Module.Data.Output;
using MPG_Interface.Module.Data.Input;
using MPG_Interface.Module.Data;

using System.Collections.Specialized;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using System;
using MPG_Interface.Module.Visual;

namespace MPG_Interface.Module.Logic {

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

        public static readonly RestClient Client = new();

        public event OnStartCall StartCall;
        public event OnEndCall EndCall;

        private readonly HttpClient client;

        public RestClient() {
            client = FactoryData.CreateClient();
        }

        public Task<List<ReportCommand>> GetReport(Period period) => CheckException(async () => {
            StartCall?.Invoke();

            List<ReportCommand> result = new();
            NameValueCollection query = FactoryData.CreateQueryFromPeriod(period);
            string address = $"Report?{query}";

            HttpResponseMessage response = await client.GetAsync(address);
            string data = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                result.AddRange(JsonSerializer.Deserialize<List<ReportCommand>>(data));
            } else {
                string message = JsonSerializer.Deserialize<string>(data);
                Alerts.ShowMessage(message);
            }

            EndCall?.Invoke();
            return result;
        });

        public Task SendSettings(List<SettingsElement> elements) => CheckException(async () => {
            StringContent json = FactoryData.CreateBody(elements);
            string address = $"Settings";

            HttpResponseMessage response = await client.PutAsync(address, json);
            string data = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                return JsonSerializer.Deserialize<bool>(data);
            } else {
                string message = JsonSerializer.Deserialize<string>(data);
                Alerts.ShowMessage(message);
            }

            return false;
        });

        public Task<List<SettingsElement>> GetSettings() => CheckException(async () => {
            List<SettingsElement> result = new();
            string address = $"Settings";

            HttpResponseMessage response = await client.GetAsync(address);
            string data = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                result.AddRange(JsonSerializer.Deserialize<List<SettingsElement>>(data));
            } else {
                string message = JsonSerializer.Deserialize<string>(data);
                Alerts.ShowMessage(message);
            }

            return result;
        });

        public Task<List<ReportMaterial>> GetMaterialsForCommand(string POID) => CheckException(async () => {
            StartCall?.Invoke();

            List<ReportMaterial> result = new();
            string address = $"Report/materials/{POID}";

            HttpResponseMessage response = await client.GetAsync(address);
            string data = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                result.AddRange(JsonSerializer.Deserialize<List<ReportMaterial>>(data));
            } else {
                string message = JsonSerializer.Deserialize<string>(data);
                Alerts.ShowMessage(message);
            }

            EndCall?.Invoke();
            return result;
        });

        public Task<List<ReportMaterial>> GetMaterialsForPail(string POID, int pail) => CheckException(async () => {
            StartCall?.Invoke();

            List<ReportMaterial> result = new();
            string address = $"Report/materials/{POID}/{pail}";

            HttpResponseMessage response = await client.GetAsync(address);
            string data = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                result.AddRange(JsonSerializer.Deserialize<List<ReportMaterial>>(data));
            } else {
                string message = JsonSerializer.Deserialize<string>(data);
                Alerts.ShowMessage(message);
            }

            EndCall?.Invoke();
            return result;
        });

        public Task<List<StatusCommand>> GetStatusCommand(Period period) => CheckException(async () => {
            StartCall?.Invoke();

            List<StatusCommand> result = new();
            NameValueCollection query = FactoryData.CreateQueryFromPeriod(period);
            string address = $"Production?{query}";

            HttpResponseMessage response = await client.GetAsync(address);
            string data = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                result.AddRange(JsonSerializer.Deserialize<List<StatusCommand>>(data));
            } else {
                string message = JsonSerializer.Deserialize<string>(data);
                Alerts.ShowMessage(message);
            }

            EndCall?.Invoke();
            return result;
        });

        public Task<List<ProductionOrder>> GetCommands(Period period) => CheckException(async () => {
            StartCall?.Invoke();

            List<ProductionOrder> result = new();
            var query = FactoryData.CreateQueryFromPeriod(period);
            string address = $"Command?{query}";

            HttpResponseMessage response = await client.GetAsync(address);
            string data = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                result.AddRange(JsonSerializer.Deserialize<List<ProductionOrder>>(data));
            } else {
                string message = JsonSerializer.Deserialize<string>(data);
                Alerts.ShowMessage(message);
            }

            EndCall?.Invoke();
            return result;
        });


        public Task<bool> CheckPriority(string priority) => CheckException(async () => {
            string address = $"Command/Priority/{priority}";

            HttpResponseMessage response = await client.GetAsync(address);
            string data = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                return JsonSerializer.Deserialize<bool>(data);
            } else {
                string message = JsonSerializer.Deserialize<string>(data);
                Alerts.ShowMessage(message);
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
                Alerts.ShowMessage(data);
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
                Alerts.ShowMessage(data);
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
                Alerts.ShowMessage(data);
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
                Alerts.ShowMessage(data);
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
                string message = JsonSerializer.Deserialize<string>(data);
                Alerts.ShowMessage(message);
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
                string message = JsonSerializer.Deserialize<string>(data);
                Alerts.ShowMessage(message);
            }

            return result;
        });

        private async Task<T> CheckException<T>(Func<Task<T>> function) {
            try {
                return await function();
            } catch (Exception ex) {
                Alerts.ShowMessage(ex.Message);
            }

            EndCall?.Invoke();
            return default;
        }
    }
}
