using MpgWebService.Repository.Interface;
using MpgWebService.Repository.Clients;
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

        public async Task<bool> BlockCommand(string POID) {
            var result = MpgClient.Client.BlockCommand(POID);

            if (!result) {
                return result;
            }

            return await SapClient.SetCommandStatusAsync(POID, "BLOC");
        }

        public Task<bool> CheckPriority(string Priority) {
            var result = MpgClient.Client.CheckPriority(Priority);
            return Task.FromResult(result);
        }

        public async Task<bool> CloseCommand(string POID) {
            var result = MpgClient.Client.PartialMaterials(POID);
            await SapClient.SendPartialProductionAsync(result);
            await SapClient.SetCommandStatusAsync(POID, "PRLT");

            MpgClient.Client.UpdateTickets(result);
            return MpgClient.Client.CloseCommand(POID);
        }

        public async Task<bool> DownloadMaterials() {
            var materials = await SapClient.GetInitialMaterialsAsync();
            var phrases = await SapClient.GetRiskPhrasesAsync();

            MpgClient.Client.SaveOrUpdateMaterials(materials);
            MpgClient.Client.SaveOrUpdateRiskPhrases(phrases);

            return true;
        }

        public async Task<bool> UpdateMaterials() {
            var materails = await SapClient.CheckSapMaterialsAsync();
            var phrases = await SapClient.GetRiskPhrasesAsync();

            MpgClient.Client.SaveOrUpdateMaterials(materails);
            MpgClient.Client.SaveOrUpdateRiskPhrases(phrases);

            return true;
        }

        public async Task<List<ProductionOrder>> GetCommands(DateTime startDate, DateTime endDate) {
            var list = await SapClient.GetCommandsAsync(startDate, endDate);
            list.AddRange(MpgClient.Client.GetCommands(startDate, endDate));

            return list;
        }

        public Task<ProductionOrder> GetCommand(string POID) {
            return Task.FromResult(MpgClient.Client.GetCommand(POID));
        }

        public Task<string> GetQC(string POID) {
            var result = SapClient.GetQC(POID);
            return Task.FromResult(result);
        }

        public async Task<bool> PartialProduction(string POID) {
            var result = MpgClient.Client.PartialMaterials(POID);
            await SapClient.SendPartialProductionAsync(result);
            MpgClient.Client.UpdateTickets(result);
            return false;
        }

        public Task StartCommand(string POID) {
            throw new NotImplementedException();
        }
    }
}
