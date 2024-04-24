using MpgWebService.Presentation.Response;
using MpgWebService.Presentation.Request;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Repository.Interface {
    public interface IMpgRepository {
        Task<PailDto> GetAvailablePail();
        Task<LabelDto> GetLabel(string POID);
        Task<List<MaterialDto>> GetMaterials(string POID);
        Task<QcLabelDto> GetQcLabel(string POID, int pailNumber);
        Task<List<MaterialDto>> GetCorrections(string POID, int pailNumber, string opNo);
        Task<ServiceResponse> SaveCorrection(POConsumption correction);
        Task<ServiceResponse> SaveDosageMaterials(POConsumption materials);
        Task<ServiceResponse> ChangeStatus(string POID, string pailNumber, string status);
    }
}