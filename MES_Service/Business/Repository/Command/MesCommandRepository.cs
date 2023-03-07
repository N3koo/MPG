using MpgWebService.Repository.Interface;
using MpgWebService.Repository.Clients;

using DataEntity.Model.Input;

using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MpgWebService.Repository {

    public class MesCommandRepository : ICommandRepository {

        private readonly MesClient mesClient;

        public MesCommandRepository() {
            mesClient = new();
        }

        public Task<List<ProductionOrder>> GetCommands(DateTime startDate, DateTime endDate) {
            var orders = mesClient.GetCommands(startDate, endDate);
            orders.AddRange(MpgClient.Client.GetCommands(startDate, endDate));

            return Task.FromResult(orders);
        }

        public Task<ProductionOrder> GetCommand(string POID) {
            var result = MpgClient.Client.GetCommand(POID);
            return Task.FromResult(result);
        }

        public Task StartCommand(string POID) {
            var data = mesClient.GetCommandData(POID);
            MpgClient.Client.StartCommand(data);

            return Task.CompletedTask;
        }

        public Task<bool> CheckPriority(string Priority) {
            var result = MpgClient.Client.CheckPriority(Priority);
            return Task.FromResult(result);
        }

        public Task<string> GetQC(string POID) {
            var result = MpgClient.Client.GetQc(POID);
            return Task.FromResult(result);
        }

        public Task<bool> BlockCommand(string POID) {
            var result = MpgClient.Client.BlockCommand(POID);

            if (!result) {
                return Task.FromResult(result);
            }

            result = mesClient.BlockCommand(POID);
            return Task.FromResult(result);
        }

        public Task<bool> CloseCommand(string POID) {
            throw new NotImplementedException();
        }

        public Task<bool> PartialProduction(string POID) {
            throw new NotImplementedException();
        }

        public Task<bool> DownloadMaterials() {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateMaterials() {
            throw new NotImplementedException();
        }
    }
}
