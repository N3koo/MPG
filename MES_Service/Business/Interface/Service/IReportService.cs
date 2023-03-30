using MpgWebService.Presentation.Request;
using MpgWebService.DTO;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Business.Interface.Service {
    public interface IReportService {
        Task<IEnumerable<ReportCommand>> GetReport(Period period);
        Task<IEnumerable<ReportMaterial>> GetMaterialsForCommand(string POID);
        Task<IEnumerable<ReportMaterial>> GetMaterialsForPail(string POID, int pail);
    }
}
