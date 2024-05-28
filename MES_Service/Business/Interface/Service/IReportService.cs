using MpgWebService.Presentation.Request.Command;
using MpgWebService.Presentation.Response.Report;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Business.Interface.Service {

    public interface IReportService {
    
        Task<IEnumerable<ReportCommandDto>> GetReport(Period period);
        Task<IEnumerable<ReportMaterialDto>> GetMaterialsForCommand(string POID);
        Task<IEnumerable<ReportMaterialDto>> GetMaterialsForPail(string POID, int pail);

    }
}
