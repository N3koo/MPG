﻿using MpgWebService.Presentation.Response.Command;
using MpgWebService.Presentation.Request.Command;
using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Response;
using MpgWebService.Repository.Interface;
using MpgWebService.Repository.Command;
using MpgWebService.Data.Extension;
using MpgWebService.Properties;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;


namespace MpgWebService.Business.Service {

    public class CommandService : ICommandService {

        private readonly ICommandRepository repository;

        public CommandService() {
            repository = new MesCommandRepository();
        }

        public CommandService(ICommandRepository repository) {
            this.repository = repository;
        }

        public async Task<ServiceResponse> BlockCommand(string POID) {
            var result = await repository.BlockCommand(POID);
            result.CheckErrors();
            return result;
        }

        public async Task<bool> CheckPriority(string priority) =>
            await repository.CheckPriority(priority);

        public async Task<ServiceResponse> CloseCommand(string POID) {
            var response = await repository.CloseCommand(POID);
            response.CheckErrors();
            return response;
        }

        public async Task<ServiceResponse> DownloadMaterials() {
            var data = Settings.Default.Update;
            ServiceResponse response;

            if (string.IsNullOrEmpty(data)) {
                response = await repository.DownloadMaterials();
            } else {
                response = await repository.UpdateMaterials();
            }

            response.CheckErrors();
            return response;
        }

        public async Task<ProductionOrderDto> GetCommand(string POID) =>
            (await repository.GetCommand(POID)).AsDto();

        public async Task<IEnumerable<ProductionOrderDto>> GetCommands(Period period) =>
            (await repository.GetCommands(period)).Select(item => item.AsDto());

        public async Task<string> GetQC(string POID) =>
            await repository.GetQC(POID);

        public async Task<ServiceResponse> StartCommand(StartCommand qc) {
            var result = await repository.StartCommand(qc);
            result.CheckErrors();
            return result;
        }

        public async Task<ServiceResponse> StartPartialProduction(string POID) {
            var result = await repository.PartialProduction(POID);
            result.CheckErrors();
            return result;
        }
    }
}
