using MpgWebService.Business.Interface.Service;
using MpgWebService.Business.Interface.Settings;
using MpgWebService.Data.Extension;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Command;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Repository.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MpgWebService.Business.Service {

    public class CommandService : ICommandService {

        private readonly ICommandRepository repository;

        private readonly ISettings settings;

        public CommandService(ICommandRepository repository, ISettings settings) {
            this.repository = repository;
            this.settings = settings;
        }

        public async Task<ServiceResponse<bool>> DownloadMaterials() {
            var data = settings.Update;

            return string.IsNullOrEmpty(data) ? await repository.DownloadMaterials() :
                                                await repository.UpdateMaterials();
        }

        public async Task<ServiceResponse<ProductionOrderDto>> GetCommand(string POID) {
            var result = await repository.GetCommand(POID);
            if (result.Errors != null) {
                return ServiceResponse<ProductionOrderDto>.GetErrors(result.Errors);
            }

            return ServiceResponse<ProductionOrderDto>.Ok(result.Data.AsDto());
        }

        public async Task<ServiceResponse<IList<ProductionOrderDto>>> GetCommands(Period period) {
            var result = await repository.GetCommands(period);
            if (result.Errors != null) {
                return ServiceResponse<IList<ProductionOrderDto>>.GetErrors(result.Errors);
            }

            var data = result.Data.Select(x => x.AsDto()).ToList();
            return ServiceResponse<IList<ProductionOrderDto>>.Ok(data);
        }

        public async Task<ServiceResponse<bool>> BlockCommand(string POID) =>
           await repository.BlockCommand(POID);

        public async Task<ServiceResponse<bool>> CheckPriority(string priority) =>
            await repository.CheckPriority(priority);

        public async Task<ServiceResponse<bool>> CloseCommand(string POID) =>
            await repository.CloseCommand(POID);

        public async Task<ServiceResponse<string>> GetQC(string POID) =>
            await repository.GetQC(POID);

        public async Task<ServiceResponse<bool>> StartCommand(StartCommand qc) =>
            await repository.StartCommand(qc);

        public async Task<ServiceResponse<bool>> StartPartialProduction(string POID) =>
            await repository.PartialProduction(POID);

    }
}
