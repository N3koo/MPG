﻿using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Report;
using MpgWebService.Presentation.Response.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Business.Interface.Service {

    public interface IReportService {

        Task<ServiceResponse<IList<ReportCommandDto>>> GetReport(Period period);

        Task<ServiceResponse<IList<ReportMaterialDto>>> GetMaterialsForCommand(string POID);

        Task<ServiceResponse<IList<ReportMaterialDto>>> GetMaterialsForPail(string POID, int pail);

    }
}
