using DataEntity.Model.Input;

using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MpgWebService.Repository.Interface {

    public interface ICommandRepository {

        Task<List<ProductionOrder>> GetCommands(DateTime startDate, DateTime endDate);

        Task<ProductionOrder> GetCommand(string POID);

        Task StartCommand(string POID);

        Task<bool> CheckPriority(string Priority);

        Task<string> GetQC(string POID);

        Task<bool> BlockCommand(string POID);

        Task<bool> CloseCommand(string POID);

        Task<bool> PartialProduction(string POID);

        Task<bool> DownloadMaterials();

        Task<bool> UpdateMaterials();
    }
}
