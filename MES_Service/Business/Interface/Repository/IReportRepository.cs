using MpgWebService.Presentation.Response.Report;
using MpgWebService.Presentation.Request.Command;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Repository.Interface {

    public interface IReportRepository {

        Task<List<ReportCommandDto>> GetReport(Period period);

        Task<IList<ReportMaterialDto>> GetMaterialsForCommand(string POID);

        Task<IList<ReportMaterialDto>> GetMaterialsForPail(string POID, int pail);
    }
}
