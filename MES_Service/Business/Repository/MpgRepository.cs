﻿using MpgWebService.Business.Data.Exceptions;
using MpgWebService.Presentation.Response;
using MpgWebService.Presentation.Request;
using MpgWebService.Repository.Interface;
using MpgWebService.Repository.Clients;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Repository.Command {

    public class MpgRepository : IMpgRepository {

        public Task<PailDto> GetAvailablePail() {
            var pail = MpgClient.Client.GetAvailablePail() ?? throw new MpgException("No pail available");

            MesClient.Client.ChangeStatus(pail.POID, pail.PailNumber, pail.PailStatus);

            return Task.FromResult(pail);
        }

        public Task<ServiceResponse> ChangeStatus(string POID, string indexPail, string status) {
            MpgClient.Client.ChangeStatus(POID, indexPail, status);
            MesClient.Client.ChangeStatus(POID, indexPail, status);

            return Task.FromResult(ServiceResponse.CreateOkResponse("Ok"));
        }
        
        public Task<ServiceResponse> SaveCorrection(POConsumption materials) {
            var result = MesClient.Client.SaveCorrection(materials);
            MpgClient.Client.SaveCorrection(materials, result);

            return Task.FromResult(ServiceResponse.CreateOkResponse("Ok"));
        }

        public Task<ServiceResponse> SaveDosageMaterials(POConsumption materials) {
            var result = MpgClient.Client.SaveDosageMaterials(materials);
            MesClient.Client.SaveDosageMaterials(result);

            return Task.FromResult(ServiceResponse.CreateOkResponse("Materialele au fost salvate"));
        }

        public Task<QcLabelDto> GetQcLabel(string POID, int pailNumber) {
            var result = MesClient.Client.SetQcStatus(POID, pailNumber) ?? throw new MesException("Could not set QC");

            MpgClient.Client.SetQC(result);
            return Task.FromResult(result);
        }

        public Task<List<MaterialDto>> GetCorrections(string POID, int pailNumber, string opNo) =>
            Task.FromResult(MesClient.Client.GetCorrections(POID, pailNumber, opNo));

        public Task<LabelDto> GetLabel(string POID) =>
            Task.FromResult(MpgClient.Client.GetLabelData(POID));

        public Task<List<MaterialDto>> GetMaterials(string POID) =>
            Task.FromResult(MpgClient.Client.GetMaterials(POID));
    }
}
