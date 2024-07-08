using MpgWebService.Presentation.Response.Mpg;
using MpgWebService.Presentation.Request.MPG;
using MpgWebService.Presentation.Response;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Business.Interface.Service {

    public interface IMpgService {

        Task<PailQCDto> GetQCPail();
        Task<PailDto> GetAvailablePail(string POID);
        Task<LabelDto> GetLabel(string POID);
        Task<List<MaterialDto>> GetMaterials(string POID);
        Task<QcLabelDto> GetQcLabel(string POID, int pailNumber);
        Task<List<MaterialDto>> GetCorrections(string POID, int pailNumber, string opNo);
        Task<ServiceResponse> SaveCorrection(POConsumption correction);
        Task<ServiceResponse> SaveDosageMaterials(POConsumption materials);
        Task<ServiceResponse> ChangeStatus(string POID, string pailNumber, string status);
        Task<List<CoefficientDto>> GetCoefficients();
        Task<ServiceResponse> UpdateReserveQuantities(ReserveTank[] quantities);
    }
}
