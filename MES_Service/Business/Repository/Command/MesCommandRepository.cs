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

    public class MesCommandRepository : ICommandRepository {

        public MesCommandRepository() {
        }

        public Task<ServiceResponse> BlockCommand(string POID) {
            var result = (int)MpgClient.Client.BlockCommand(POID).Data;
            ProductionOrder po;

            switch (result) {
                case 0:
                    _ = MesClient.Client.BlockCommand(POID);
                    break;
                case 1:
                    return Task.FromResult(ServiceResponse.CreateErrorMpg("Nu se poate bloca comanda deoarece s-a inceput procesul de productie"));
                case 2:
                    po = MesClient.Client.BlockCommand(POID);
                    MpgClient.Client.CreateCommand(po);
                    break;
            }

            return Task.FromResult(ServiceResponse.CreateOkResponse("Comanda a fost blocata cu succes"));
        }

        public Task<ServiceResponse> GetCommands(Period period) {
            var orders = MesClient.Client.GetCommands(period);
            orders.AddRange(MpgClient.Client.GetCommands(period));

            return Task.FromResult(orders);
        }

        public Task<ServiceResponse> GetCommand(string POID) {
            var result = MpgClient.Client.GetCommand(POID);
            return Task.FromResult(result);
        }

        public Task<ServiceResponse> StartCommand(StartCommand qc) {
            var data = MesClient.Client.GetCommandData(qc);

            MpgClient.Client.StartCommand(data);

            return Task.FromResult(ServiceResponse.CreateOkResponse("Comanda a fost transmisa"));
        }

        public Task<ServiceResponse> CheckPriority(string Priority) {
            var result = MpgClient.Client.CheckPriority(Priority);
            return Task.FromResult(result);
        }

        public Task<ServiceResponse> GetQC(string POID) {
            var result = MesClient.Client.GetQc(POID);
            return Task.FromResult(result);
        }

        public Task<ServiceResponse> CloseCommand(string POID) {
            MesClient.Client.CloseCommand(POID);
            MpgClient.Client.CloseCommand(POID);

            return Task.FromResult(ServiceResponse.CreateOkResponse("Comanda a fost inchisa"));
        }

        public Task<ServiceResponse> PartialProduction(string POID) {
            MesClient.Client.SendPartialProduction(POID);

            return Task.FromResult(ServiceResponse.CreateOkResponse("Materialele au fost predate"));
        }

        public Task<ServiceResponse> DownloadMaterials() {
            var result = MesClient.Client.GetMaterials();
            var phrases = MesClient.Client.GetRiskPhrases();
            var vessels = MesClient.Client.GetStockVessels();

            MpgClient.Client.SaveOrUpdateMaterials(result);
            MpgClient.Client.SaveOrUpdateRiskPhrases(phrases);
            MpgClient.Client.SaveOrUpdateStockVesel(vessels);

            UpdateDate();

            return Task.FromResult(ServiceResponse.CreateOkResponse("Materialele au fost descarcate"));
        }

        public Task<ServiceResponse> UpdateMaterials() {
            var result = MesClient.Client.GetMaterials();
            var phrases = MesClient.Client.GetRiskPhrases();
            var vessels = MesClient.Client.GetStockVessels();

            MpgClient.Client.SaveOrUpdateMaterials(result);
            MpgClient.Client.SaveOrUpdateRiskPhrases(phrases);
            MpgClient.Client.SaveOrUpdateStockVesel(vessels);

            UpdateDate();

            return Task.FromResult(ServiceResponse.CreateOkResponse("Materialele au fost actualizate"));
        }

        private void UpdateDate() {
            Settings.Default.Update = DateTime.Now.ToString();
            Settings.Default.Save();
        }
    }
}
