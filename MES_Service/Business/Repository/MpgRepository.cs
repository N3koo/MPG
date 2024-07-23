using MpgWebService.Presentation.Request.MPG;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Repository.Clients;
using MpgWebService.Repository.Interface;
using System.Threading.Tasks;

namespace MpgWebService.Repository.Command {

    public class MpgRepository : IMpgRepository {

        public async Task<ServiceResponse> ChangeStatus(string POID, string indexPail, string status) {
            var mpgResponse = await MpgClient.Client.ChangeStatus(POID, indexPail, status);
            var mesReponse = await MesClient.Client.ChangeStatus(POID, indexPail, status);

            return ServiceResponse.CombineResponses(mpgResponse, mesReponse);
        }

        public async Task<ServiceResponse> SaveCorrection(POConsumption materials) {
            var mesResponse = await MesClient.Client.SaveCorrection(materials);
            var mpgResponse = await MpgClient.Client.SaveCorrection(materials, mesResponse);

            return ServiceResponse.CombineResponses(mesResponse, mpgResponse);
        }

        public async Task<ServiceResponse> SaveDosageMaterials(POConsumption materials) {
            var mpgResponse = await MpgClient.Client.SaveDosageMaterials(materials);
            var mesResponse = await MesClient.Client.SaveDosageMaterials(mpgResponse);

            return ServiceResponse.CombineResponses(mpgResponse, mesResponse);
        }

        public async Task<ServiceResponse> GetQcLabel(string POID, int pailNumber) {
            var mesResult = await MesClient.Client.SetQcStatus(POID, pailNumber);
            var mpgResult = await MpgClient.Client.SetQC(mesResult);

            return ServiceResponse.CombineResponses(mesResult, mpgResult);
        }

        public async Task<ServiceResponse> GetQCPail() =>
           await MpgClient.Client.GetFirstPail();

        public async Task<ServiceResponse> GetAvailablePail(string POID) =>
            await MpgClient.Client.GetAvailablePail(POID);

        public async Task<ServiceResponse> GetCorrections(string POID, int pailNumber, string opNo) =>
            await MesClient.Client.GetCorrections(POID, pailNumber, opNo);

        public async Task<ServiceResponse> GetLabel(string POID) =>
            await MpgClient.Client.GetLabelData(POID);

        public async Task<ServiceResponse> GetMaterials(string POID) =>
            await MpgClient.Client.GetMaterials(POID);

        public async Task<ServiceResponse> GetCoefficients() =>
            await MesClient.Client.GetCoefficients();

        public async Task<ServiceResponse> UpdateReserveQuantities(ReserveTank[] quatities) =>
            await MesClient.Client.ReserveQuantities(quatities);
    }
}
