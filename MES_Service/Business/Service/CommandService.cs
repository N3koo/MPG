using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Wrapper;
using MpgWebService.Properties;
using MpgWebService.Repository.Command;
using MpgWebService.Repository.Interface;
using System.Threading.Tasks;


namespace MpgWebService.Business.Service {

    public class CommandService : ICommandService {

        private readonly ICommandRepository repository;

        public CommandService() {
            repository = new MesCommandRepository();
        }

        public CommandService(ICommandRepository repository) {
            this.repository = repository;
        }

        public async Task<ServiceResponse> BlockCommand(string POID) =>
            await repository.BlockCommand(POID);

        public async Task<ServiceResponse> CheckPriority(string priority) =>
            await repository.CheckPriority(priority);

        public async Task<ServiceResponse> CloseCommand(string POID) =>
            await repository.CloseCommand(POID);

        public async Task<ServiceResponse> DownloadMaterials() {
            var data = Settings.Default.Update;
            ServiceResponse response;

            if (string.IsNullOrEmpty(data)) {
                response = await repository.DownloadMaterials();
            } else {
                response = await repository.UpdateMaterials();
            }

            return response;
        }

        public async Task<ServiceResponse> GetCommand(string POID) =>
            await repository.GetCommand(POID);

        public async Task<ServiceResponse> GetCommands(Period period) =>
            await repository.GetCommands(period);

        public async Task<ServiceResponse> GetQC(string POID) =>
            await repository.GetQC(POID);

        public async Task<ServiceResponse> StartCommand(StartCommand qc) =>
            await repository.StartCommand(qc);

        public async Task<ServiceResponse> StartPartialProduction(string POID) =>
            await repository.PartialProduction(POID);
    }
}
