using MpgWebService.Presentation.Request;
using MpgWebService.Repository.Interface;
using MpgWebService.Repository.Clients;
using MpgWebService.Business.Data.DTO;
using MpgWebService.Properties;

using DataEntity.Model.Input;

using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MpgWebService.Repository.Command {

    public class SapCommandRepository : ICommandRepository {

        private readonly SapClient SapClient;

        public SapCommandRepository() {
            SapClient = new SapClient();
        }

        public async Task<Response> BlockCommand(string POID) {
            var status = MpgClient.Client.BlockCommand(POID);
            ProductionOrder po;

            switch (status) {
                case 0:
                    var result = SapClient.BlockCommand(POID);
                    if (result == null) {
                        return Response.CreateErrorSap($"Nu s-a putut actualiza statusul pentru comanda {POID}");
                    }
                    break;
                case 1:
                    return Response.CreateOkResponse("Nu se poate bloca comanda deoarece s-a inceput procesul de productie");
                case 2:
                    po = await SapClient.BlockCommand(POID);
                    MpgClient.Client.CreateCommand(po);
                    break;
            }

            return Response.CreateOkResponse("Comanda a fost blocata cu succes");
        }

        public async Task<List<ProductionOrder>> GetCommands(Period period) {
            var list = await SapClient.GetCommandsAsync(period);
            list.AddRange(MpgClient.Client.GetCommands(period));

            return list;
        }

        public Task<bool> CheckPriority(string Priority) {
            var result = MpgClient.Client.CheckPriority(Priority);
            return Task.FromResult(result);
        }

        public async Task<Response> CloseCommand(string POID) {
            var result = MpgClient.Client.PartialMaterials(POID);
            var response = await SapClient.SendPartialProductionAsync(result);

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

        public async Task<Response> DownloadMaterials() {
            var materials = await SapClient.GetInitialMaterialsAsync();
            var phrases = await SapClient.GetRiskPhrasesAsync();

            MpgClient.Client.SaveOrUpdateMaterials(materials);
            MpgClient.Client.SaveOrUpdateRiskPhrases(phrases);

            UpdateDate();

            return Response.CreateOkResponse("Materialele au fost descarcate");
        }

        public async Task<Response> UpdateMaterials() {
            var materails = await SapClient.CheckSapMaterialsAsync();
            var phrases = await SapClient.GetRiskPhrasesAsync();

            MpgClient.Client.SaveOrUpdateMaterials(materails);
            MpgClient.Client.SaveOrUpdateRiskPhrases(phrases);

            UpdateDate();

            return Response.CreateOkResponse("Materialele au fost actulizate");
        }

        private void UpdateDate() {
            Settings.Default.Update = DateTime.Now.ToString();
            Settings.Default.Save();
        }

        public Task<ProductionOrder> GetCommand(string POID) {
            return Task.FromResult(MpgClient.Client.GetCommand(POID));
        }

        public Task<string> GetQC(string POID) {
            var result = SapClient.GetQC(POID);

            if (result == null) {
                result = MpgClient.Client.GetQc(POID);
            }

            return Task.FromResult(result);
        }

        public async Task<Response> PartialProduction(string POID) {
            var result = MpgClient.Client.PartialMaterials(POID);
            var response = await SapClient.SendPartialProductionAsync(result);

            if (!response.Status) {
                return response;
            }

            MpgClient.Client.UpdateTickets(result);
            return response;
        }

        public async Task<Response> StartCommand(StartCommand qc) {
            var list = SapClient.StartCommand(qc);

            if (list.Count == 0) {
                return Response.CreateErrorMpg($"Nu au fost inserate datele in MPG pentru comanda {qc.POID}");
            }

            var result = await SapClient.SetCommandStatusAsync(qc.POID, Resources.CMD_STARTED);

            if (!result) {
                return Response.CreateErrorSap($"Nu s-a putut actualiza statusul pentru comanda {qc.POID}");
            }

            return Response.CreateOkResponse("Comanda a fost transmisa");
        }
    }
}
