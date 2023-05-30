using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Response;
using MpgWebService.Presentation.Request;
using MpgWebService.Repository.Interface;
using MpgWebService.Repository.Command;
using MpgWebService.Business.Data.DTO;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Business.Service {

    public class MpgService : IMpgService {

        private readonly IMpgRepository repository;

        public MpgService() {
            repository = new MpgRepository();
        }

        public async Task<ServiceResponse> ChangeStatus(string POID, string pail, string status) {
            var result = await repository.ChangeStatus(POID, pail, status);
            result.CheckErrors();
            return result;
        }

        public async Task<object> GetAvailablePail() {
            return await repository.GetAvailablePail();
        }

        public async Task<List<CorrectionDto>> GetCorrections(QcDetails details) {
            var result = await repository.GetCorrections(details);
            if (result.Count == 0) {
                ServiceResponse.CreateErrorMes("Nu exista corectii").CheckErrors();
            }

            return result;
        }

        public async Task<List<Materials>> GetMaterials(string POID) {
            var result = await repository.GetMaterials(POID);
            if (result.Count == 0) {
                ServiceResponse.CreateErrorMes($"Nu exista materiale pentru comanda {POID}").CheckErrors();
            }

            return result;
        }

        public async Task<List<LotDetails>> GetOperationsList(string POID) {
            var result = await repository.GetOperationsList(POID);

            if (result.Count == 0) {
                ServiceResponse.CreateErrorMes("Nu exisa pasi de QC").CheckErrors();
            }

            return result;
        }

        public async Task<ServiceResponse> SaveCorrection(POCorrection correction) {
            var result = await repository.SaveCorrection(correction);
            result.CheckErrors();
            return result;
        }

        public async Task<ServiceResponse> SaveDosageMaterials(List<POConsumption> materials) {
            var result = await repository.SaveDosageMaterials(materials);
            result.CheckErrors();
            return result;
        }

        public async Task<QcLabel> SetQcStatus(QcDetails details) {
            var result = await repository.SetQcStatus(details);

            if (result == null) {
                ServiceResponse.CreateErrorMes("Nu exista corectie").CheckErrors();
            }

            return result;
        }


    }
}
