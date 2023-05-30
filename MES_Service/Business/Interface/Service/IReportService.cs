using MpgWebService.Presentation.Request;

using System.Collections.Generic;
using System.Threading.Tasks;
using MpgWebService.Business.Data.DTO;

namespace MpgWebService.Business.Interface.Service {
    public interface IReportService {
        Task<IEnumerable<ReportCommand>> GetReport(Period period);
        Task<IEnumerable<ReportMaterial>> GetMaterialsForCommand(string POID);
        Task<IEnumerable<ReportMaterial>> GetMaterialsForPail(string POID, int pail);
    }
}
