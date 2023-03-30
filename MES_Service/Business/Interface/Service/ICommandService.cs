using MpgWebService.Presentation.Request;
using MpgWebService.Business.Data.DTO;
using MpgWebService.DTO;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Business.Interface.Service {
    public interface ICommandService {
        Task<IEnumerable<ProductionOrderDto>> GetCommands(Period period);
        Task<ProductionOrderDto> GetCommand(string POID);
        Task<Response> StartCommand(StartCommand qc);
        Task<string> GetQC(string POID);
        Task<bool> CheckPriority(string priority);
        Task<Response> BlockCommand(string POID);
        Task<Response> CloseCommand(string POID);
        Task<Response> StartPartialProduction(string POID);
        Task<Response> DownloadMaterials();
    }
}
