using MpgWebService.Presentation.Response.Command;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Business.Interface.Service {

    public interface ICommandService {

        Task<IEnumerable<ProductionOrderDto>> GetCommands(Period period);
        Task<ProductionOrderDto> GetCommand(string POID);
        Task<ServiceResponse> StartCommand(StartCommand qc);
        Task<string> GetQC(string POID);
        Task<bool> CheckPriority(string priority);
        Task<ServiceResponse> BlockCommand(string POID);
        Task<ServiceResponse> CloseCommand(string POID);
        Task<ServiceResponse> StartPartialProduction(string POID);
        Task<ServiceResponse> DownloadMaterials();

    }
}
