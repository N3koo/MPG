using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Properties;
using MpgWebService.Repository.Clients;
using MpgWebService.Repository.Interface;
using System;
using System.Threading.Tasks;


namespace MpgWebService.Repository.Command {

    public class MesCommandRepository : ICommandRepository {

        public MesCommandRepository() {
        }

        public async Task<ServiceResponse> BlockCommand(string POID) {
            var mpgResponse = await MpgClient.Client.BlockCommand(POID);
            var mesResponse = await MesClient.Client.BlockCommand(POID);

            return ServiceResponse.CombineResponses(mpgResponse, mesResponse);
        }

        public async Task<ServiceResponse> GetCommands(Period period) {
            var mesOrders = await MesClient.Client.GetCommands(period);
            var mpgOrders = await MpgClient.Client.GetCommands(period);
            return ServiceResponse.CombineResponses(mesOrders, mpgOrders);
        }

        public async Task<ServiceResponse> GetCommand(string POID) =>
            await MpgClient.Client.GetCommand(POID);

        public async Task<ServiceResponse> StartCommand(StartCommand qc) {
            var mesResponse = await MesClient.Client.GetCommandData(qc);
            var mpgResponse = await MpgClient.Client.StartCommand(mesResponse);

            return mpgResponse;
        }

        public async Task<ServiceResponse> CheckPriority(string Priority) =>
            await MpgClient.Client.CheckPriority(Priority);

        public async Task<ServiceResponse> GetQC(string POID) =>
            await MesClient.Client.GetQc(POID);

        public async Task<ServiceResponse> CloseCommand(string POID) {
            var mesResponse = await MesClient.Client.CloseCommand(POID);
            var mpgResponse = await MpgClient.Client.CloseCommand(POID);

            throw new NotImplementedException();
            return mpgResponse;
        }

        public async Task<ServiceResponse> PartialProduction(string POID) =>
            await MesClient.Client.SendPartialProduction(POID);

        public async Task<ServiceResponse> DownloadMaterials() {
            var result = await MesClient.Client.GetMaterials();
            var phrases = await MesClient.Client.GetRiskPhrases();
            var vessels = await MesClient.Client.GetStockVessels();

            var materialUpdate = await MpgClient.Client.SaveOrUpdateMaterials(result);
            var phrasesUpdate = await MpgClient.Client.SaveOrUpdateRiskPhrases(phrases);
            var stockVessel = await MpgClient.Client.SaveOrUpdateStockVesel(vessels);

            UpdateDate();
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse> UpdateMaterials() {
            var result = await MesClient.Client.GetMaterials();
            var phrases = await MesClient.Client.GetRiskPhrases();
            var vessels = await MesClient.Client.GetStockVessels();

            var materialsResponse = await MpgClient.Client.SaveOrUpdateMaterials(result);
            var phrasesResponse = await MpgClient.Client.SaveOrUpdateRiskPhrases(phrases);
            var vesselsResponse = await MpgClient.Client.SaveOrUpdateStockVesel(vessels);

            UpdateDate();
            throw new NotImplementedException();
        }

        private void UpdateDate() {
            Settings.Default.Update = DateTime.Now.ToString();
            Settings.Default.Save();
        }
    }
}
