using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Request.MPG;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Repository.Command;
using MpgWebService.Repository.Interface;
using System.Threading.Tasks;

namespace MpgWebService.Business.Service
{

    public class MpgService : IMpgService {

        private readonly IMpgRepository repository;

        public MpgService() {
            repository = new MpgRepository();
        }

        public async Task<ServiceResponse> GetAvailablePail(string POID) =>
            await repository.GetAvailablePail(POID);

        public async Task<ServiceResponse> GetQCPail() =>
            await repository.GetQCPail();

        public async Task<ServiceResponse> GetLabel(string POID) =>
            await repository.GetLabel(POID);

        public async Task<ServiceResponse> GetMaterials(string POID) =>
            await repository.GetMaterials(POID);

        public async Task<ServiceResponse> GetQcLabel(string POID, int pailNumber) =>
            await repository.GetQcLabel(POID, pailNumber);

        public async Task<ServiceResponse> GetCorrections(string POID, int pailNumber, string opNo) =>
            await repository.GetCorrections(POID, pailNumber, opNo);

        public async Task<ServiceResponse> SaveCorrection(POConsumption correction) =>
            await repository.SaveCorrection(correction);

        public async Task<ServiceResponse> SaveDosageMaterials(POConsumption materials) =>
            await repository.SaveDosageMaterials(materials);

        public async Task<ServiceResponse> ChangeStatus(string POID, string pail, string status) =>
            await repository.ChangeStatus(POID, pail, status);

        public async Task<ServiceResponse> GetCoefficients() =>
            await repository.GetCoefficients();

        public async Task<ServiceResponse> UpdateReserveQuantities(ReserveTank[] quantities) =>
            await repository.UpdateReserveQuantities(quantities);
    }
}
