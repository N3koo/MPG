using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Command;
using MpgWebService.Presentation.Response.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Business.Interface.Service {

    public interface ICommandService {

        Task<ServiceResponse<IList<ProductionOrderDto>>> GetCommands(Period period);
        Task<ServiceResponse<ProductionOrderDto>> GetCommand(string POID);
        Task<ServiceResponse<string>> GetQC(string POID);
        Task<ServiceResponse<bool>> StartCommand(StartCommand qc);
        Task<ServiceResponse<bool>> CheckPriority(string priority);
        Task<ServiceResponse<bool>> BlockCommand(string POID);
        Task<ServiceResponse<bool>> CloseCommand(string POID);
        Task<ServiceResponse<bool>> StartPartialProduction(string POID);
        Task<ServiceResponse<bool>> DownloadMaterials();

    }
}
