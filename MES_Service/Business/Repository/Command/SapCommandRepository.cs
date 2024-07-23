using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Properties;
using MpgWebService.Repository.Clients;
using MpgWebService.Repository.Interface;
using System;
using System.Threading.Tasks;


namespace MpgWebService.Repository.Command
{

    public class SapCommandRepository : ICommandRepository {

        public SapCommandRepository() {
        }

        public async Task<ServiceResponse> BlockCommand(string POID) {
            var mpgResponse = await MpgClient.Client.BlockCommand(POID);
            var sapResponse = await SapClient.Client.BlockCommand(POID);

            return ServiceResponse.CombineResponses(mpgResponse, sapResponse);
        }

        public async Task<ServiceResponse> GetCommands(Period period) {
            var sapResponse = await SapClient.Client.GetCommandsAsync(period);
            var mpgResponse = await MpgClient.Client.GetCommands(period);

            return ServiceResponse.CombineResponses(sapResponse, mpgResponse);
        }

        public async Task<ServiceResponse> CheckPriority(string Priority) =>
            await MpgClient.Client.CheckPriority(Priority);

        public async Task<ServiceResponse> CloseCommand(string POID) {
            var result = await MpgClient.Client.PartialMaterials(POID);
            var response = await SapClient.Client.SendPartialProductionAsync(result);
            var updateResponse = await MpgClient.Client.UpdateTickets(result);
            var closeResponse = MpgClient.Client.CloseCommand(POID);

            throw new NotImplementedException();
        }

        public async Task<ServiceResponse> DownloadMaterials() {
            var materials = await SapClient.Client.GetInitialMaterialsAsync();
            var phrases = await SapClient.Client.GetRiskPhrasesAsync();

            var materialResponse = await MpgClient.Client.SaveOrUpdateMaterials(materials);
            var phrasesResponse = await MpgClient.Client.SaveOrUpdateRiskPhrases(phrases);

            UpdateDate();

            return ServiceResponse.CombineResponses(materialResponse, phrasesResponse);
        }

        public async Task<ServiceResponse> UpdateMaterials() {
            var materails = await SapClient.Client.CheckSapMaterialsAsync();
            var phrases = await SapClient.Client.GetRiskPhrasesAsync();

            var materialResponse = await MpgClient.Client.SaveOrUpdateMaterials(materails);
            var phrasesResponse = await MpgClient.Client.SaveOrUpdateRiskPhrases(phrases);

            UpdateDate();

            return ServiceResponse.CombineResponses(materialResponse, phrasesResponse);
        }

        private void UpdateDate() {
            Settings.Default.Update = DateTime.Now.ToString();
            Settings.Default.Save();
        }

        public async Task<ServiceResponse> GetCommand(string POID) =>
            await MpgClient.Client.GetCommand(POID);

        public async Task<ServiceResponse> GetQC(string POID) {
            var result = SapClient.Client.GetQC(POID);
            if (result != null) {
                return ServiceResponse.Ok(result);
            }

            var mpgResponse = await MpgClient.Client.GetQc(POID);
            return mpgResponse;
        }

        public async Task<ServiceResponse> PartialProduction(string POID) {
            var result = await MpgClient.Client.PartialMaterials(POID);
            var response = await SapClient.Client.SendPartialProductionAsync(result);
            var updateResponse = await MpgClient.Client.UpdateTickets(response);

            throw new NotImplementedException();
            return response;
        }

        public async Task<ServiceResponse> StartCommand(StartCommand qc) {
            var list = SapClient.Client.StartCommand(qc);

            if (list.Count == 0) {
                return ServiceResponse.CreateErrorMpg($"Nu au fost inserate datele in MPG pentru comanda {qc.POID}");
            }

            var result = await SapClient.Client.SetCommandStatusAsync(qc.POID, Settings.Default.CMD_STARTED);

            if (!result) {
                return ServiceResponse.CreateErrorSap($"Nu s-a putut actualiza statusul pentru comanda {qc.POID}");
            }

            return ServiceResponse.Ok("Comanda a fost transmisa");
        }
    }
}
