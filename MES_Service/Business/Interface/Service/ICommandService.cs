using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Wrapper;
using System.Threading.Tasks;

namespace MpgWebService.Business.Interface.Service
{

    public interface ICommandService {

        Task<ServiceResponse> GetCommands(Period period);
        Task<ServiceResponse> GetCommand(string POID);
        Task<ServiceResponse> StartCommand(StartCommand qc);
        Task<ServiceResponse> GetQC(string POID);
        Task<ServiceResponse> CheckPriority(string priority);
        Task<ServiceResponse> BlockCommand(string POID);
        Task<ServiceResponse> CloseCommand(string POID);
        Task<ServiceResponse> StartPartialProduction(string POID);
        Task<ServiceResponse> DownloadMaterials();

    }
}
