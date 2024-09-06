using DataEntity.Model.Input;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Repository.Interface {

    public interface ICommandRepository {

        Task<ServiceResponse<IList<ProductionOrder>>> GetCommands(Period period);

        Task<ServiceResponse<ProductionOrder>> GetCommand(string POID);

        Task<ServiceResponse<bool>> CheckPriority(string Priority);

        Task<ServiceResponse<string>> GetQC(string POID);

        Task<ServiceResponse<bool>> StartCommand(StartCommand qc);

        Task<ServiceResponse<bool>> BlockCommand(string POID);

        Task<ServiceResponse<bool>> CloseCommand(string POID);

        Task<ServiceResponse<bool>> PartialProduction(string POID);

        Task<ServiceResponse<bool>> DownloadMaterials();

        Task<ServiceResponse<bool>> UpdateMaterials();

    }
}
