using DataEntity.Model.Input;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Properties;
using MpgWebService.Repository.Clients;
using MpgWebService.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MpgWebService.Repository.Command {

    public class MesCommandRepository : ICommandRepository {

        public MesCommandRepository() {
        }

        public async Task<ServiceResponse<bool>> BlockCommand(string POID) {
            var mpgResponse = await MpgClient.Client.BlockCommand(POID);
            var mesResponse = await MesClient.Client.BlockCommand(POID);

            return ServiceResponse<bool>.CombineResponses(mpgResponse, mesResponse);
        }

        public async Task<ServiceResponse<IList<ProductionOrder>>> GetCommands(Period period) {
            var mesOrders = await MesClient.Client.GetCommands(period);
            var mpgOrders = await MpgClient.Client.GetCommands(period);

            var data = mesOrders.Data.Concat(mpgOrders.Data).ToList();
            return ServiceResponse<IList<ProductionOrder>>.Ok(data);
        }

        public async Task<ServiceResponse<bool>> StartCommand(StartCommand qc) {
            var mesResponse = await MesClient.Client.GetCommandData(qc);
            return await MpgClient.Client.StartCommand(mesResponse.Data);
        }

        public async Task<ServiceResponse<bool>> CloseCommand(string POID) {
            var mesResponse = await MesClient.Client.CloseCommand(POID);
            var mpgResponse = await MpgClient.Client.CloseCommand(POID);

            return ServiceResponse<bool>.CombineResponses(mesResponse, mpgResponse);
        }

        public async Task<ServiceResponse<bool>> DownloadMaterials() {
            var result = await MesClient.Client.GetMaterials();
            var phrases = await MesClient.Client.GetRiskPhrases();
            var vessels = await MesClient.Client.GetStockVessels();

            var materialUpdate = await MpgClient.Client.SaveOrUpdateMaterials(result.Data);
            var phrasesUpdate = await MpgClient.Client.SaveOrUpdateRiskPhrases(phrases.Data);
            var stockVessel = await MpgClient.Client.SaveOrUpdateStockVesel(vessels.Data);

            UpdateDate();

            return ServiceResponse<bool>.CombineResponses(materialUpdate, phrasesUpdate, stockVessel);
        }

        public async Task<ServiceResponse<bool>> UpdateMaterials() {
            var result = await MesClient.Client.GetMaterials();
            var phrases = await MesClient.Client.GetRiskPhrases();
            var vessels = await MesClient.Client.GetStockVessels();

            var materialsResponse = await MpgClient.Client.SaveOrUpdateMaterials(result.Data);
            var phrasesResponse = await MpgClient.Client.SaveOrUpdateRiskPhrases(phrases.Data);
            var vesselsResponse = await MpgClient.Client.SaveOrUpdateStockVesel(vessels.Data);

            UpdateDate();

            return ServiceResponse<bool>.CombineResponses(materialsResponse, phrasesResponse, vesselsResponse);
        }

        public async Task<ServiceResponse<bool>> PartialProduction(string POID) =>
            await MesClient.Client.SendPartialProduction(POID);

        //TODO: Check MES first
        public async Task<ServiceResponse<ProductionOrder>> GetCommand(string POID) =>
         await MpgClient.Client.GetCommand(POID);

        public async Task<ServiceResponse<bool>> CheckPriority(string Priority) =>
            await MpgClient.Client.CheckPriority(Priority);

        public async Task<ServiceResponse<string>> GetQC(string POID) =>
            await MesClient.Client.GetQc(POID);

        private void UpdateDate() {
            Settings.Default.Update = DateTime.Now.ToString();
            Settings.Default.Save();
        }
    }
}
