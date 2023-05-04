﻿using MpgWebService.Presentation.Response;
using MpgWebService.Presentation.Request;
using MpgWebService.Business.Data.DTO;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Business.Interface.Service {

    public interface IMpgService {
        Task<List<Materials>> GetMaterials(string POID);
        Task<object> GetAvailablePail();
        Task<List<LotDetails>> GetOperationsList(string POID);
        Task<QcLabel> SetQcStatus(QcDetails details);
        Task<List<CorrectionDto>> GetCorrections(QcDetails details);
        Task<Response> SaveCorrection(POCorrection correction);
        Task<Response> SaveDosageMaterials(List<POConsumption> materials);
        Task<Response> ChangeStatus(string POID, string pail, string status);
    }
}
