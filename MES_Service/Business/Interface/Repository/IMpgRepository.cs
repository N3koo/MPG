using MpgWebService.Presentation.Response;
using MpgWebService.Presentation.Request;
using MpgWebService.Business.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Repository.Interface {
    public interface IMpgRepository {
        Task<ServiceResponse> ChangeStatus(string POID, string indexPail, string status);
        Task<object> GetAvailablePail();
        Task<List<CorrectionDto>> GetCorrections(QcDetails details);
        Task<List<Materials>> GetMaterials(string POID);
        Task<List<LotDetails>> GetOperationsList(string POID);
        Task<ServiceResponse> SaveCorrection(POCorrection correction);
        Task<ServiceResponse> SaveDosageMaterials(List<POConsumption> materials);
        Task<QcLabel> SetQcStatus(QcDetails details);
    }
}
