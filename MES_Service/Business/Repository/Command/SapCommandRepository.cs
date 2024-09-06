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

    public class SapCommandRepository : ICommandRepository {

        public SapCommandRepository() {
        }

        public async Task<ServiceResponse<bool>> BlockCommand(string POID) {
            var mpgResponse = await MpgClient.Client.BlockCommand(POID);
            var sapResponse = await SapClient.Client.BlockCommand(POID);

            return ServiceResponse<bool>.CombineResponses(mpgResponse, sapResponse);
        }

        public async Task<ServiceResponse<IList<ProductionOrder>>> GetCommands(Period period) {
            var sapResponse = await SapClient.Client.GetCommandsAsync(period);
            var mpgResponse = await MpgClient.Client.GetCommands(period);

            var data = sapResponse.Data.Concat(mpgResponse.Data).ToList();
            return ServiceResponse<IList<ProductionOrder>>.Ok(data);
        }

        public async Task<ServiceResponse<bool>> CloseCommand(string POID) {
            var result = await MpgClient.Client.GetPartialMaterials(POID);
            var partialResponse = await SapClient.Client.SendPartialProductionAsync(result.Data);

            if (partialResponse.Errors.Count > 0) {
                return partialResponse;
            }

            var updateResponse = await MpgClient.Client.UpdateTickets(result.Data.Pails);
            var closeResponse = await MpgClient.Client.CloseCommand(POID);

            return ServiceResponse<bool>.CombineResponses(partialResponse, updateResponse, closeResponse);
        }

        public async Task<ServiceResponse<bool>> DownloadMaterials() {
            var materials = await SapClient.Client.GetInitialMaterialsAsync();
            var phrases = await SapClient.Client.GetRiskPhrasesAsync();

            var materialResponse = await MpgClient.Client.SaveOrUpdateMaterials(materials.Data);
            var phrasesResponse = await MpgClient.Client.SaveOrUpdateRiskPhrases(phrases.Data);

            UpdateDate();

            return ServiceResponse<bool>.CombineResponses(materialResponse, phrasesResponse);
        }

        public async Task<ServiceResponse<bool>> UpdateMaterials() {
            var materails = await SapClient.Client.CheckSapMaterialsAsync();
            var phrases = await SapClient.Client.GetRiskPhrasesAsync();

            var materialResponse = await MpgClient.Client.SaveOrUpdateMaterials(materails.Data);
            var phrasesResponse = await MpgClient.Client.SaveOrUpdateRiskPhrases(phrases.Data);

            UpdateDate();

            return ServiceResponse<bool>.CombineResponses(materialResponse, phrasesResponse);
        }

        private void UpdateDate() {
            Settings.Default.Update = DateTime.Now.ToString();
            Settings.Default.Save();
        }

        public async Task<ServiceResponse<string>> GetQC(string POID) {
            var result = SapClient.Client.GetQC(POID);
            if (result != null) {
                return ServiceResponse<string>.Ok(result);
            }

            var mpgResponse = await MpgClient.Client.GetQc(POID);
            return mpgResponse;
        }

        public async Task<ServiceResponse<bool>> PartialProduction(string POID) {
            var result = await MpgClient.Client.GetPartialMaterials(POID);

            var response = await SapClient.Client.SendPartialProductionAsync(result.Data);
            var updateResponse = await MpgClient.Client.UpdateTickets(result.Data.Pails);

            return ServiceResponse<bool>.CombineResponses(response, updateResponse);
        }

        public async Task<ServiceResponse<bool>> StartCommand(StartCommand qc) {
            var list = SapClient.Client.StartCommand(qc);

            if (list.Count == 0) {
                return ServiceResponse<bool>.CreateErrorSap($"Nu au fost inserate datele in MPG pentru comanda {qc.POID}");
            }

            var result = await SapClient.Client.SetCommandStatusAsync(qc.POID, Settings.Default.CMD_STARTED);

            if (!result) {
                return ServiceResponse<bool>.CreateErrorSap($"Nu s-a putut actualiza statusul pentru comanda {qc.POID}");
            }

            return ServiceResponse<bool>.Ok(true);
        }

        public async Task<ServiceResponse<ProductionOrder>> GetCommand(string POID) =>
            await MpgClient.Client.GetCommand(POID);

        public async Task<ServiceResponse<bool>> CheckPriority(string Priority) =>
            await MpgClient.Client.CheckPriority(Priority);
    }
}
