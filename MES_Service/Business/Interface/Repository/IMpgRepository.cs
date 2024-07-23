using MpgWebService.Presentation.Request.MPG;
using MpgWebService.Presentation.Response.Wrapper;
using System.Threading.Tasks;


namespace MpgWebService.Repository.Interface
{

    public interface IMpgRepository {
        Task<ServiceResponse> GetQCPail();

        Task<ServiceResponse> GetAvailablePail(string POID);

        Task<ServiceResponse> GetLabel(string POID);

        Task<ServiceResponse> GetMaterials(string POID);

        Task<ServiceResponse> GetQcLabel(string POID, int pailNumber);

        Task<ServiceResponse> GetCorrections(string POID, int pailNumber, string opNo);

        Task<ServiceResponse> SaveCorrection(POConsumption correction);

        Task<ServiceResponse> SaveDosageMaterials(POConsumption materials);

        Task<ServiceResponse> ChangeStatus(string POID, string pailNumber, string status);

        Task<ServiceResponse> GetCoefficients();

        Task<ServiceResponse> UpdateReserveQuantities(ReserveTank[] quatities);
    }
}