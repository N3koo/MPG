using MpgWebService.Presentation.Response;
using MpgWebService.Presentation.Request;

using DataEntity.Model.Input;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Repository.Interface {

    public interface ICommandRepository {

        Task<List<ProductionOrder>> GetCommands(Period period);

        Task<ProductionOrder> GetCommand(string POID);

        Task<bool> CheckPriority(string Priority);

        Task<string> GetQC(string POID);

        Task<ServiceResponse> StartCommand(StartCommand qc);

        Task<ServiceResponse> BlockCommand(string POID);

        Task<ServiceResponse> CloseCommand(string POID);

        Task<ServiceResponse> PartialProduction(string POID);

        Task<ServiceResponse> DownloadMaterials();

        Task<ServiceResponse> UpdateMaterials();
    }
}
