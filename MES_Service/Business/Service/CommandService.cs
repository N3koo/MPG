﻿using MpgWebService.Business.Interface.Service;
using MpgWebService.Presentation.Request;
using MpgWebService.Repository.Interface;
using MpgWebService.Repository.Command;
using MpgWebService.Business.Data.DTO;
using MpgWebService.Data.Extension;
using MpgWebService.Properties;
using MpgWebService.DTO;

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

        public async Task<Response> BlockCommand(string POID) {
            var result = await repository.BlockCommand(POID);
            result.CheckErrors();
            return result;
        }

        public async Task<bool> CheckPriority(string priority) {
            return await repository.CheckPriority(priority);
        }

        public async Task<Response> CloseCommand(string POID) {
            var response = await repository.CloseCommand(POID);
            response.CheckErrors();
            return response;
        }

        public async Task<Response> DownloadMaterials() {
            var data = Settings.Default.Update;
            Response response;

            if (string.IsNullOrEmpty(data)) {
                response = await repository.DownloadMaterials();
            } else {
                response = await repository.UpdateMaterials();
            }

            response.CheckErrors();
            return response;
        }

        public async Task<ProductionOrderDto> GetCommand(string POID) {
            var result = await repository.GetCommand(POID);
            return result.AsDto();
        }

        public async Task<IEnumerable<ProductionOrderDto>> GetCommands(Period period) {
            var result = await repository.GetCommands(period);
            return result.Select(item => item.AsDto());
        }

        public async Task<string> GetQC(string POID) {
            return await repository.GetQC(POID);
        }

        public async Task<Response> StartCommand(StartCommand qc) {
            var result = await repository.StartCommand(qc);
            result.CheckErrors();
            return result;
        }

        public async Task<Response> StartPartialProduction(string POID) {
            var result = await repository.PartialProduction(POID);
            result.CheckErrors();
            return result;
        }
    }
}
