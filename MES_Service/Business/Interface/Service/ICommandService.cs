using MpgWebService.Presentation.Request;
using MpgWebService.Business.Data.DTO;

using System.Collections.Generic;
using System.Threading.Tasks;
using MpgWebService.Presentation.Response;

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
