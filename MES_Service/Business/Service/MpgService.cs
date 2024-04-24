using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Response;
using MpgWebService.Presentation.Request;
using MpgWebService.Repository.Interface;
using MpgWebService.Repository.Command;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Business.Service { 

    public class MpgService : IMpgService {

        private readonly IMpgRepository repository;

        public MpgService() {
            repository = new MpgRepository();
        }

        public async Task<PailDto> GetAvailablePail() => 
            await repository.GetAvailablePail();

        public async Task<LabelDto> GetLabel(string POID) =>
            await repository.GetLabel(POID);

        public async Task<List<MaterialDto>> GetMaterials(string POID) {
            var result = await repository.GetMaterials(POID);
            if (result.Count == 0) {
                ServiceResponse.CreateErrorMes($"Nu exista materiale pentru comanda {POID}").CheckErrors();
            }

            return result;
        }

        public async Task<QcLabelDto> GetQcLabel(string POID, int pailNumber) =>
            await repository.GetQcLabel(POID, pailNumber);

        public async Task<List<MaterialDto>> GetCorrections(string POID, int pailNumber, string opNo) {
            var result = await repository.GetCorrections(POID, pailNumber, opNo);
            if (result.Count == 0) {
                ServiceResponse.CreateErrorMes("Nu exista corectii").CheckErrors();
            }

            return result;
        }

        public async Task<ServiceResponse> SaveCorrection(POConsumption correction) {
            var result = await repository.SaveCorrection(correction);
            result.CheckErrors();
            return result;
        }

        public async Task<ServiceResponse> SaveDosageMaterials(POConsumption materials) {
            var result = await repository.SaveDosageMaterials(materials);
            result.CheckErrors();
            return result;
        }

        public async Task<ServiceResponse> ChangeStatus(string POID, string pail, string status) {
            var result = await repository.ChangeStatus(POID, pail, status);
            result.CheckErrors();
            return result;
        }
    }
}
