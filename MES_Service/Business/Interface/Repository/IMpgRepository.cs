using MpgWebService.Presentation.Request.MPG;
using MpgWebService.Presentation.Response.Mpg;
using MpgWebService.Presentation.Response.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Repository.Interface {

    public interface IMpgRepository {

        Task<ServiceResponse<PailQCDto>> GetQCPail();

        Task<ServiceResponse<PailDto>> GetAvailablePail(string POID);

        Task<ServiceResponse<LabelDto>> GetLabel(string POID);

        Task<ServiceResponse<IList<MaterialDto>>> GetMaterials(string POID);

        Task<ServiceResponse<QcLabelDto>> GetQcLabel(string POID, int pailNumber);

        Task<ServiceResponse<IList<MaterialDto>>> GetCorrections(string POID, int pailNumber, string opNo);

        Task<ServiceResponse<bool>> SaveCorrection(POConsumption correction);

        Task<ServiceResponse<bool>> SaveDosageMaterials(POConsumption materials);

        Task<ServiceResponse<bool>> ChangeStatus(string POID, string pailNumber, string status);

        Task<ServiceResponse<IList<CoefficientDto>>> GetCoefficients();

        Task<ServiceResponse<bool>> UpdateReserveQuantities(ReserveTank[] quatities);

    }
}