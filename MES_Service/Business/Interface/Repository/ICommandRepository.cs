using MpgWebService.Presentation.Request;
using DataEntity.Model.Input;

using System.Collections.Generic;
using System.Threading.Tasks;
using MpgWebService.Business.Data.DTO;

namespace MpgWebService.Repository.Interface {

    public interface ICommandRepository {

        Task<List<ProductionOrder>> GetCommands(Period period);

        Task<ProductionOrder> GetCommand(string POID);

        Task<bool> CheckPriority(string Priority);

        Task<string> GetQC(string POID);

        Task<Response> StartCommand(StartCommand qc);

        Task<Response> BlockCommand(string POID);

        Task<Response> CloseCommand(string POID);

        Task<Response> PartialProduction(string POID);

        Task<Response> DownloadMaterials();

        Task<Response> UpdateMaterials();
    }
}
