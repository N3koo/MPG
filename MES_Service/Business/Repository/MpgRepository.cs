using MpgWebService.Presentation.Response;
using MpgWebService.Presentation.Request;
using MpgWebService.Repository.Interface;
using MpgWebService.Repository.Clients;
using MpgWebService.Business.Data.DTO;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace MpgWebService.Repository.Command {

    public class MpgRepository : IMpgRepository {

        public Task<ServiceResponse> ChangeStatus(string POID, string indexPail, string status) {
            MpgClient.Client.ChangeStatus(POID, indexPail, status);
            MesClient.Client.ChangeStatus(POID, indexPail, status);

            return Task.FromResult(ServiceResponse.CreateOkResponse("Ok"));
        }

        public Task<List<CorrectionDto>> GetCorrections(QcDetails details) {
            var result = MesClient.Client.GetCorrections(details).Select(p => CorrectionDto.FromCorrection(p));
            return Task.FromResult(result.ToList());
        }



        public Task<ServiceResponse> SaveCorrection(POCorrection correction) {
            var po = POCorrection.CreatePOCorrection(correction);

            MesClient.Client.SaveCorrection(po);
            MpgClient.Client.SaveCorrection(po);

            return Task.FromResult(ServiceResponse.CreateOkResponse("Ok"));
        }

        public Task<ServiceResponse> SaveDosageMaterials(List<POConsumption> materials) {
            var consumption = materials.Select(p => POConsumption.CreateConsumption(p)).ToList();

            MesClient.Client.SaveDosageMaterials(consumption);
            MpgClient.Client.SaveDosageMaterials(consumption);

            return Task.FromResult(ServiceResponse.CreateOkResponse("Materialele au fost salvate"));
        }

        public Task<object> GetAvailablePail() {
            throw new NotImplementedException();
            //Task.FromResult(MpgClient.Client.GetAvailablePail());

        }

        public Task<List<Materials>> GetMaterials(string POID) =>
            Task.FromResult(MpgClient.Client.GetMaterials(POID).Select(p => Materials.FromBom(p)).ToList());

        public Task<QcLabel> SetQcStatus(QcDetails details) =>
            Task.FromResult(MesClient.Client.SetQcStatus(details));

        public Task<List<LotDetails>> GetOperationsList(string POID) =>
            Task.FromResult(MpgClient.Client.GetOperations(POID));
    }
}
