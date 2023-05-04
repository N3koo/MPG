using MpgWebService.Presentation.Request;
using MpgWebService.Repository.Interface;
using MpgWebService.Business.Data.DTO;
using MpgWebService.Repository.Clients;
using MpgWebService.Properties;

using DataEntity.Model.Input;

using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MpgWebService.Repository.Command {

    public class MesCommandRepository : ICommandRepository {

        public MesCommandRepository() {
        }

        public Task<Response> BlockCommand(string POID) {
            var result = MpgClient.Client.BlockCommand(POID);
            ProductionOrder po;

            switch (result) {
                case 0:
                    _ = MesClient.Client.BlockCommand(POID);
                    break;
                case 1:
                    return Task.FromResult(Response.CreateErrorMpg("Nu se poate bloca comanda deoarece s-a inceput procesul de productie"));
                case 2:
                    po = MesClient.Client.BlockCommand(POID);
                    MpgClient.Client.CreateCommand(po);
                    break;
            }

            return Task.FromResult(Response.CreateOkResponse("Comanda a fost blocata cu succes"));
        }

        public Task<List<ProductionOrder>> GetCommands(Period period) {
            var orders = MesClient.Client.GetCommands(period);
            orders.AddRange(MpgClient.Client.GetCommands(period));

            return Task.FromResult(orders);
        }

        public Task<ProductionOrder> GetCommand(string POID) {
            var result = MpgClient.Client.GetCommand(POID);
            return Task.FromResult(result);
        }

        public Task<Response> StartCommand(StartCommand qc) {
            var data = MesClient.Client.GetCommandData(qc);

            MpgClient.Client.StartCommand(data);

            return Task.FromResult(Response.CreateOkResponse("Comanda a fost transmisa"));
        }

        public Task<bool> CheckPriority(string Priority) {
            var result = MpgClient.Client.CheckPriority(Priority);
            return Task.FromResult(result);
        }

        public Task<string> GetQC(string POID) {
            var result = MesClient.Client.GetQc(POID);
            return Task.FromResult(result);
        }

        public Task<Response> CloseCommand(string POID) {
            MesClient.Client.CloseCommand(POID);
            MpgClient.Client.CloseCommand(POID);

            return Task.FromResult(Response.CreateOkResponse("Comanda a fost inchisa"));
        }

        public Task<Response> PartialProduction(string POID) {
            MesClient.Client.SendPartialProduction(POID);

            return Task.FromResult(Response.CreateOkResponse("Materialele au fost predate"));
        }

        public Task<Response> DownloadMaterials() {
            var result = MesClient.Client.GetMaterials();
            var phrases = MesClient.Client.GetRiskPhrases();
            var vessels = MesClient.Client.GetStockVessels();

            MpgClient.Client.SaveOrUpdateMaterials(result);
            MpgClient.Client.SaveOrUpdateRiskPhrases(phrases);
            MpgClient.Client.SaveOrUpdateStockVesel(vessels);

            UpdateDate();

            return Task.FromResult(Response.CreateOkResponse("Materialele au fost descarcate"));
        }

        public Task<Response> UpdateMaterials() {
            var result = MesClient.Client.GetMaterials();
            var phrases = MesClient.Client.GetRiskPhrases();
            var vessels = MesClient.Client.GetStockVessels();

            MpgClient.Client.SaveOrUpdateMaterials(result);
            MpgClient.Client.SaveOrUpdateRiskPhrases(phrases);
            MpgClient.Client.SaveOrUpdateStockVesel(vessels);

            UpdateDate();

            return Task.FromResult(Response.CreateOkResponse("Materialele au fost actualizate"));
        }

        private void UpdateDate() {
            Settings.Default.Update = DateTime.Now.ToString();
            Settings.Default.Save();
        }
    }
}
