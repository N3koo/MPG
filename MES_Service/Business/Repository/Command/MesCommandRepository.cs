using MpgWebService.Presentation.Request;
using MpgWebService.Repository.Interface;
using MpgWebService.Business.Data.DTO;
using MpgWebService.Repository.Clients;

using DataEntity.Model.Input;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Repository {

    public class MesCommandRepository : ICommandRepository {

        private readonly MesClient mesClient;

        public MesCommandRepository() {
            mesClient = new();
        }

        public Task<Response> BlockCommand(string POID) {
            var result = MpgClient.Client.BlockCommand(POID);
            ProductionOrder po;

            switch (result) {
                case 0:
                    _ = mesClient.BlockCommand(POID);
                    break;
                case 1:
                    return Task.FromResult(Response.CreateOkResponse("Nu se poate bloca comanda deoarece s-a inceput procesul de productie"));
                case 2:
                    po = mesClient.BlockCommand(POID);
                    MpgClient.Client.CreateCommand(po);
                    break;
            }

            return Task.FromResult(Response.CreateOkResponse("Comanda a fost blocata cu succes"));
        }

        public Task<List<ProductionOrder>> GetCommands(Period period) {
            var orders = mesClient.GetCommands(period);
            orders.AddRange(MpgClient.Client.GetCommands(period));

            return Task.FromResult(orders);
        }

        public Task<ProductionOrder> GetCommand(string POID) {
            var result = MpgClient.Client.GetCommand(POID);
            return Task.FromResult(result);
        }

        public Task<Response> StartCommand(StartCommand qc) {
            var data = mesClient.GetCommandData(qc);

            MpgClient.Client.StartCommand(data);

            return Task.FromResult(Response.CreateOkResponse("Comanda a fost transmisa"));
        }

        public Task<bool> CheckPriority(string Priority) {
            var result = MpgClient.Client.CheckPriority(Priority);
            return Task.FromResult(result);
        }

        public Task<string> GetQC(string POID) {
            var result = MpgClient.Client.GetQc(POID);
            return Task.FromResult(result);
        }

        public Task<Response> CloseCommand(string POID) {
            var result = MpgClient.Client.CloseCommand(POID);
            return Task.FromResult(Response.CreateOkResponse(""));
        }

        public Task<Response> PartialProduction(string POID) {
            var result = mesClient.SendPartialProduction(POID);
            return Task.FromResult(Response.CreateOkResponse(""));
        }

        public Task<Response> DownloadMaterials() {
            var result = mesClient.GetMaterials();
            MpgClient.Client.SaveOrUpdateMaterials(result);

            var phrases = mesClient.GetRiskPhrases();
            MpgClient.Client.SaveOrUpdateRiskPhrases(phrases);

            return Task.FromResult(Response.CreateOkResponse(""));
        }

        public Task<Response> UpdateMaterials() {
            var result = mesClient.GetMaterials();
            MpgClient.Client.SaveOrUpdateMaterials(result);

            var phrases = mesClient.GetRiskPhrases();
            MpgClient.Client.SaveOrUpdateRiskPhrases(phrases);

            return Task.FromResult(Response.CreateOkResponse(""));
        }
    }
}
