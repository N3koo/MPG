using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Request.MPG;
using MpgWebService.Presentation.Response.Mpg;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Repository.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Business.Service {

    public class MpgService : IMpgService {

        private readonly IMpgRepository repository;

        public MpgService(IMpgRepository repository) {
            this.repository = repository;
        }

        public async Task<ServiceResponse<PailDto>> GetAvailablePail(string POID) =>
            await repository.GetAvailablePail(POID);

        public async Task<ServiceResponse<PailQCDto>> GetQCPail() =>
            await repository.GetQCPail();

        public async Task<ServiceResponse<LabelDto>> GetLabel(string POID) =>
            await repository.GetLabel(POID);

        public async Task<ServiceResponse<IList<MaterialDto>>> GetMaterials(string POID) =>
            await repository.GetMaterials(POID);

        public async Task<ServiceResponse<QcLabelDto>> GetQcLabel(string POID, int pailNumber) =>
            await repository.GetQcLabel(POID, pailNumber);

        public async Task<ServiceResponse<IList<MaterialDto>>> GetCorrections(string POID, int pailNumber, string opNo) =>
            await repository.GetCorrections(POID, pailNumber, opNo);

        public async Task<ServiceResponse<bool>> SaveCorrection(POConsumption correction) =>
            await repository.SaveCorrection(correction);

        public async Task<ServiceResponse<bool>> SaveDosageMaterials(POConsumption materials) =>
            await repository.SaveDosageMaterials(materials);

        public async Task<ServiceResponse<bool>> ChangeStatus(string POID, string pail, string status) =>
            await repository.ChangeStatus(POID, pail, status);

        public async Task<ServiceResponse<IList<CoefficientDto>>> GetCoefficients() =>
            await repository.GetCoefficients();

        public async Task<ServiceResponse<bool>> UpdateReserveQuantities(ReserveTank[] quantities) =>
            await repository.UpdateReserveQuantities(quantities);
    }
}
