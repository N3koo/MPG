using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response;
using MpgWebService.Repository.Interface;
using MpgWebService.Repository.Clients;
using MpgWebService.Properties;

using DataEntity.Model.Input;

using System.Collections.Generic;
using System.Threading.Tasks;
using System;


namespace MpgWebService.Repository.Command {

    public class SapCommandRepository : ICommandRepository {

        public SapCommandRepository() {
        }

        public async Task<ServiceResponse> BlockCommand(string POID) {
            var status = MpgClient.Client.BlockCommand(POID);
            ProductionOrder po;

            switch (status) {
                case 0:
                    var result = SapClient.Client.BlockCommand(POID);
                    if (result == null) {
                        return ServiceResponse.CreateErrorSap($"Nu s-a putut actualiza statusul pentru comanda {POID}");
                    }
                    break;
                case 1:
                    return ServiceResponse.CreateErrorMpg($"Comanda {POID} nu se poate bloca deoarece s-a inceput procesul de productie");
                case 2:
                    po = await SapClient.Client.BlockCommand(POID);
                    MpgClient.Client.CreateCommand(po);
                    break;
            }

            return ServiceResponse.CreateOkResponse($"Comanda {POID} a fost blocata cu succes");
        }

        public async Task<List<ProductionOrder>> GetCommands(Period period) {
            var list = await SapClient.Client.GetCommandsAsync(period);
            list.AddRange(MpgClient.Client.GetCommands(period));

            return list;
        }

        public Task<bool> CheckPriority(string Priority) {
            var result = MpgClient.Client.CheckPriority(Priority);
            return Task.FromResult(result);
        }

        public async Task<ServiceResponse> CloseCommand(string POID) {
            var result = MpgClient.Client.PartialMaterials(POID);
            var response = await SapClient.Client.SendPartialProductionAsync(result);

            if (!response.Status) {
                return response;
            }

            response = MpgClient.Client.UpdateTickets(result);

            if (!response.Status) {
                return response;
            }

            response = MpgClient.Client.CloseCommand(POID);
            return response;
        }

        public async Task<ServiceResponse> DownloadMaterials() {
            var materials = await SapClient.Client.GetInitialMaterialsAsync();
            var phrases = await SapClient.Client.GetRiskPhrasesAsync();

            MpgClient.Client.SaveOrUpdateMaterials(materials);
            MpgClient.Client.SaveOrUpdateRiskPhrases(phrases);

            UpdateDate();

            return ServiceResponse.CreateOkResponse("Materialele au fost descarcate");
        }

        public async Task<ServiceResponse> UpdateMaterials() {
            var materails = await SapClient.Client.CheckSapMaterialsAsync();
            var phrases = await SapClient.Client.GetRiskPhrasesAsync();

            MpgClient.Client.SaveOrUpdateMaterials(materails);
            MpgClient.Client.SaveOrUpdateRiskPhrases(phrases);

            UpdateDate();

            return ServiceResponse.CreateOkResponse("Materialele au fost actulizate");
        }

        private void UpdateDate() {
            Settings.Default.Update = DateTime.Now.ToString();
            Settings.Default.Save();
        }

        public Task<ProductionOrder> GetCommand(string POID) {
            return Task.FromResult(MpgClient.Client.GetCommand(POID));
        }

        public Task<string> GetQC(string POID) {
            var result = SapClient.Client.GetQC(POID);

            result ??= MpgClient.Client.GetQc(POID);

            return Task.FromResult(result);
        }

        public async Task<ServiceResponse> PartialProduction(string POID) {
            var result = MpgClient.Client.PartialMaterials(POID);
            var response = await SapClient.Client.SendPartialProductionAsync(result);

            if (!response.Status) {
                return response;
            }

            MpgClient.Client.UpdateTickets(result);
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

            return ServiceResponse.CreateOkResponse("Comanda a fost transmisa");
        }
    }
}
