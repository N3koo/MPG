using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response;

using DataEntity.Model.Input;

using System.Threading.Tasks;

namespace MpgWebService.Repository.Interface {

    public interface ICommandRepository {

        Task<ServiceResponse> GetCommands(Period period);

        Task<ProductionOrder> GetCommand(string POID);

        Task<ServiceResponse> CheckPriority(string Priority);

        Task<ServiceResponse> GetQC(string POID);

        Task<ServiceResponse> StartCommand(StartCommand qc);

        Task<ServiceResponse> BlockCommand(string POID);

        Task<ServiceResponse> CloseCommand(string POID);

        Task<ServiceResponse> PartialProduction(string POID);

        Task<ServiceResponse> DownloadMaterials();

        Task<ServiceResponse> UpdateMaterials();
    }
}
