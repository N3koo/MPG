using MpgWebService.Presentation.Request;
using MpgWebService.DTO;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MpgWebService.Repository.Interface {

    public interface IReportRepository {

        Task<List<ReportCommand>> GetReport(Period period);

        Task<IList<ReportMaterial>> GetMaterialsForCommand(string POID);

        Task<IList<ReportMaterial>> GetMaterialsForPail(string POID, int pail);
    }
}
